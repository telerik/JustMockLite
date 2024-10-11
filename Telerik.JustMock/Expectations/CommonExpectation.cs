/*
 JustMock Lite
 Copyright © 2010-2015,2019,2021,2023 Progress Software Corporation

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Core.MatcherTree;
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock.Expectations
{
    /// <summary>
    /// Implements common expectations.
    /// </summary>
    public partial class CommonExpectation<TContainer> : IAction<TContainer>, IInstanceScope<TContainer>, IMethodMock
    {
        private readonly List<IBehavior> behaviors = new List<IBehavior>();
        private readonly InvocationOccurrenceBehavior occurences;
        private ImplementationOverrideBehavior acceptCondition;

        MocksRepository IMethodMock.Repository { get; set; }

        IMockMixin IMethodMock.Mock { get; set; }

        bool IMethodMock.IsSequential { get; set; }
        bool IMethodMock.IsInOrder { get; set; }
        bool IMethodMock.IsUsed { get; set; }
        CallPattern IMethodMock.CallPattern { get; set; }
        ICollection<IBehavior> IMethodMock.Behaviors { get { return this.behaviors; } }
        InvocationOccurrenceBehavior IMethodMock.OccurencesBehavior { get { return this.occurences; } }
        string IMethodMock.ArrangementExpression { get; set; }

        ImplementationOverrideBehavior IMethodMock.AcceptCondition
        {
            get { return this.acceptCondition; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (this.acceptCondition != null)
                    throw new MockException("Condition already specified.");
                this.acceptCondition = value;
            }
        }

        private MocksRepository Repository
        {
            get { return ((IMethodMock)this).Repository; }
        }

        private CallPattern CallPattern
        {
            get { return ((IMethodMock)this).CallPattern; }
        }

        internal IMockMixin Mock
        {
            get { return ((IMethodMock)this).Mock; }
        }

        internal CommonExpectation()
        {
            this.occurences = new InvocationOccurrenceBehavior(this);
            this.behaviors.Add(this.occurences);
        }

        #region Implementation from ICommon<TContainer>

        /// <summary>
        /// Implementation detail.
        /// </summary>
        /// <param name="delg"></param>
        /// <param name="ignoreDelegateReturnValue"></param>
        protected void ProcessDoInstead(Delegate delg, bool ignoreDelegateReturnValue)
        {

#if !PORTABLE
            // force interception of construcor declaring type used in each newobj IL instruction
            // in elevated mode, this will allow those types to be successfully resolved by the
            // profiler while instrumenting newobj instructions on the JIT phase
            if (ProfilerInterceptor.IsProfilerAttached
                && ProfilerInterceptor.NewObjInterceptionOnOverwriteEnabled
                && !ProfilerInterceptor.IsReJitEnabled)
            {
                MockingUtil.MethodBodyDisassembler.DisassembleMethodInfo(this.CallPattern.Method)
                    .Where(instr => instr.OpCode == System.Reflection.Emit.OpCodes.Newobj)
                    .ToList()
                    .ForEach(instr =>
                        {
                            try
                            {
                                var objCtor = this.CallPattern.Method.DeclaringType.Module.ResolveMethod(instr.Operand.Int);
                                MockingContext.CurrentRepository.EnableInterception(objCtor.DeclaringType);
                            }
                            catch (ArgumentException)
                            {
                                // as a last shot, try to load referenced assemblies by the constructor declaring type
                                try
                                {
                                    var notLoadedReferencedAssemblyNames = this.CallPattern.Method.DeclaringType.Assembly.GetReferencedAssemblies()
                                        .Where(assemblyName =>
                                            !AppDomain.CurrentDomain.GetAssemblies()
                                                .Select(assembly => assembly.GetName())
                                                .Contains(assemblyName));

                                    foreach (var notLoadedReferencedAssemblyName in notLoadedReferencedAssemblyNames)
                                    {
                                        var manuallyLoadedReferencedAssembly = Assembly.Load(notLoadedReferencedAssemblyName.FullName);
                                        if (manuallyLoadedReferencedAssembly.GetExportedTypes()
                                                .SelectMany(type => type.GetConstructors())
                                                .Select(ctor => ctor.MetadataToken)
                                                .Contains(instr.Operand.Int))
                                        {
                                            break;
                                        }
                                    }
                                }
                                catch (Exception) { }
                            }
                            catch (Exception) { }
                        });
            }
#endif

            if (delg == null)
            {
                var returnType = CallPattern.Method.GetReturnType();
                if (returnType == typeof(void))
                    returnType = typeof(object);
                if (returnType.IsValueType && Nullable.GetUnderlyingType(returnType) == null)
                    throw new MockException(String.Format("Cannot return null value for non-nullable return type '{0}'.", returnType));

                delg = Expression.Lambda(typeof(Func<>).MakeGenericType(returnType),
                    Expression.Constant(null, returnType)).Compile();
            }

            this.behaviors.Add(new ImplementationOverrideBehavior(delg, ignoreDelegateReturnValue));
        }

        /// <summary>
        /// Defines the body of the expected method that will be executed instead of the orginal method body.
        /// </summary>
        /// <param name="action">delegate the method body</param>
        /// <returns></returns>
        public TContainer DoInstead(Action action)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    ProcessDoInstead(action, true);
                    return (TContainer)(object)this;
                });
        }

        /// <summary>
        /// Specifies the delegate that will execute for the expected method.
        /// </summary>
        /// <param name="delegate">Target delegate to evaluate.</param>
        public TContainer DoInstead(Delegate @delegate)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    ProcessDoInstead(@delegate, true);
                    return (TContainer)(object)this;
                });
        }

        private void ProcessRaises(Action eventExpression, Func<object, Delegate> eventArgumentsFactoryFactory)
        {
            object instance = null;
            var evt = Repository.ParseAddHandlerAction(eventExpression, out instance);
            this.behaviors.Add(new RaiseEventBehavior(instance, evt, eventArgumentsFactoryFactory(instance)));
        }

        ///<summary>
        /// Raises the expected with sepecic arguments
        ///</summary>
        public TContainer Raises(Action eventExpression, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
#if !PORTABLE
                    IWaitDuration wait = args.OfType<IWaitDuration>().FirstOrDefault();
                    if (wait != null)
                    {
                        args = args.Where(obj => obj != wait).ToArray();
                    }
#endif

                    this.ProcessRaises(eventExpression, instance => new Func<object[]>(() =>
                        {
#if !PORTABLE
                            if (wait != null)
                                Thread.Sleep(wait.Miliseconds);
#endif
                            return args;
                        }));
                    return (TContainer)(object)this;
                });
        }

        ///<summary>
        /// Raises the expected event with provided <see cref="EventArgs"/>.
        ///</summary>
        ///<param name="eventExpression"></param>
        ///<param name="args">Event arguments</param>
        ///<returns></returns>
        ///<exception cref="InvalidOperationException"></exception>
        public TContainer Raises(Action eventExpression, EventArgs args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    this.ProcessRaises(eventExpression, instance => new Func<object[]>(() => new object[] { instance, args }));
                    return (TContainer)(object)this;
                });
        }

        ///<summary>
        /// Raises the expected event for the setup.
        ///</summary>
        ///<param name="eventExpression"></param>
        ///<param name="func">Function that will be used to construct event arguments</param>
        ///<returns></returns>
        ///<exception cref="InvalidOperationException"></exception>
        public TContainer Raises<T1>(Action eventExpression, Func<T1, EventArgs> func)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    this.ProcessRaises(eventExpression, instance => new Func<T1, object[]>(t1 => new object[] { instance, func(t1) }));
                    return (TContainer)(object)this;
                });
        }

        ///<summary>
        /// Raises the expected event for the setup.
        ///</summary>
        ///<param name="eventExpression"></param>
        ///<param name="func">An function that will be used to construct event arguments</param>
        ///<returns></returns>
        ///<exception cref="InvalidOperationException"></exception>
        public TContainer Raises<T1, T2>(Action eventExpression, Func<T1, T2, EventArgs> func)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    this.ProcessRaises(eventExpression, instance => new Func<T1, T2, object[]>((t1, t2) => new object[] { instance, func(t1, t2) }));
                    return (TContainer)(object)this;
                });
        }

        ///<summary>
        /// Raises the expected event for the setup.
        ///</summary>
        ///<param name="eventExpression"></param>
        ///<param name="func">An function that will be used to construct event arguments</param>
        ///<returns></returns>
        ///<exception cref="InvalidOperationException"></exception>
        public TContainer Raises<T1, T2, T3>(Action eventExpression, Func<T1, T2, T3, EventArgs> func)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    this.ProcessRaises(eventExpression, instance => new Func<T1, T2, T3, object[]>((t1, t2, t3) => new object[] { instance, func(t1, t2, t3) }));
                    return (TContainer)(object)this;
                });
        }

        ///<summary>
        /// Raises the expected event for the setup.
        ///</summary>
        ///<param name="eventExpression"></param>
        ///<param name="func">An function that will be used to construct event arguments</param>
        ///<returns></returns>
        ///<exception cref="InvalidOperationException"></exception>
        public TContainer Raises<T1, T2, T3, T4>(Action eventExpression, Func<T1, T2, T3, T4, EventArgs> func)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    this.ProcessRaises(eventExpression, instance => new Func<T1, T2, T3, T4, object[]>((t1, t2, t3, t4) => new object[] { instance, func(t1, t2, t3, t4) }));
                    return (TContainer)(object)this;
                });
        }

        /// <summary>
        /// Throws a the specified expection for target call.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public IAssertable Throws(Exception exception)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    this.behaviors.Add(new ThrowExceptionBehavior(exception));
                    return this;
                });
        }

        /// <summary>
        /// Throws a the specified expection for target call.
        /// </summary>
        /// <returns></returns>
        public IAssertable Throws<T>() where T : Exception
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    this.behaviors.Add(new ThrowExceptionBehavior((T)MockingUtil.CreateInstance(typeof(T))));
                    return this;
                });
        }

        /// <summary>
        /// Throws a the specified expection for target call.
        /// </summary>
        /// <returns></returns>
        public IAssertable Throws<T>(params object[] args) where T : Exception
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    this.behaviors.Add(new ThrowExceptionBehavior((T)MockingUtil.CreateInstance(typeof(T), args)));
                    return this;
                });
        }

#if !LITE_EDITION
        /// <summary>
        /// Throws a the specified exception for the target async call causing returned task to fail.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public IAssertable ThrowsAsync(Exception exception)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                this.behaviors.Add(new ThrowAsyncExceptionBehavior(exception));
                return this;
            });
        }

        /// <summary>
        /// Throws a the specified exception for the target async call causing returned task to fail.
        /// </summary>
        /// <returns></returns>
        public IAssertable ThrowsAsync<T>() where T : Exception
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                this.behaviors.Add(new ThrowAsyncExceptionBehavior((T)MockingUtil.CreateInstance(typeof(T))));
                return this;
            });
        }

        /// <summary>
        /// Throws a the specified exception for the target async call causing returned task to fail.
        /// </summary>
        /// <returns></returns>
        public IAssertable ThrowsAsync<T>(params object[] args) where T : Exception
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                this.behaviors.Add(new ThrowAsyncExceptionBehavior((T)MockingUtil.CreateInstance(typeof(T), args)));
                return this;
            });
        }
#endif

        /// <summary>
        /// Use it to call the real implementation.
        /// </summary>
        /// <returns></returns>
        public IAssertable CallOriginal()
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    this.behaviors.Add(new CallOriginalBehavior());
                    return this;
                });
        }

        /// <summary>
        ///  Specfies call a to step over.
        /// </summary>
        /// <remarks>
        /// For loose mocks by default the behavior is step over.
        /// </remarks>
        /// <returns>Refarence to <see cref="IAssertable"/></returns>
        public IAssertable DoNothing()
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    ProcessDoInstead(new Action(() => { }), true);
                    return this;
                });
        }

        #endregion

        #region Implementation of IAssertable

        /// <summary>
        /// Specifies that the arranged member must be called. Asserting the mock will throw if the expectation is not fulfilled.
        /// </summary>
        public IDisposable MustBeCalled(string message = null)
        {
            return ProfilerInterceptor.GuardInternal(() => SetOccurrenceBounds(1, null, message));
        }

        #endregion

        #region Occurrence

        /// <summary>
        /// Specifies how many times the call should occur.
        /// </summary>
        /// <param name="numberOfTimes">Specified number of times</param>
        public IDisposable Occurs(int numberOfTimes, string message = null)
        {
            return ProfilerInterceptor.GuardInternal(() => SetOccurrenceBounds(numberOfTimes, numberOfTimes, message));
        }

        /// <summary>
        /// Specifies how many times atleast the call should occur.
        /// </summary>
        /// <param name="numberOfTimes">Specified number of times</param>
        public IDisposable OccursAtLeast(int numberOfTimes, string message = null)
        {
            return ProfilerInterceptor.GuardInternal(() => SetOccurrenceBounds(numberOfTimes, null, message));
        }

        /// <summary>
        /// Specifies how many times maximum the call can occur.
        /// </summary>
        /// <param name="numberOfTimes">Specified number of times</param>
        public IDisposable OccursAtMost(int numberOfTimes, string message = null)
        {
            return ProfilerInterceptor.GuardInternal(() => SetOccurrenceBounds(null, numberOfTimes, message));
        }

        /// <summary>
        /// Specifies that the call must occur once.
        /// </summary>
        public IDisposable OccursOnce(string message = null)
        {
            return ProfilerInterceptor.GuardInternal(() => SetOccurrenceBounds(1, 1, message));
        }

        /// <summary>
        /// Specifies that the call must never occur.
        /// </summary>
        public IDisposable OccursNever(string message = null)
        {
            return ProfilerInterceptor.GuardInternal(() => SetOccurrenceBounds(0, 0, message));
        }

        #endregion

        /// <summary>
        /// Specifies that JustMock should invoke different mock instance for each setup.
        /// </summary>
        /// <remarks>
        /// When this modifier is applied
        /// for similar type call, the flow of setups will be maintained.
        /// </remarks>
        public IAssertable InSequence()
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    (this as IMethodMock).IsSequential = true;
                    return this;
                });
        }

        /// <summary>
        /// Specifies a call should occur in a specific order.
        /// </summary>
        public IOccurrence InOrder(string message = null)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    (this as IMethodMock).IsInOrder = true;
                    this.behaviors.Add(new InOrderBehavior(this.Repository, this.Mock, message));
                    return this;
                });
        }

        /// <summary>
        /// Determines whether prerequisite is met
        /// </summary>
        public bool IsMet
        {
            get
            {
                return ProfilerInterceptor.GuardInternal(() => ((IMethodMock)this).IsUsed);
            }
        }

        /// <summary>
        /// Specifies that a call should occur only after all of the given prerequisites have been met.
        /// </summary>
        public IAssertable AfterAll(params IPrerequisite[] prerequisites)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                this.behaviors.Add(new AfterAllBehavior(prerequisites));
                return this;
            });
        }

        /// <summary>
        /// Defines that the expectation should occur for all instance.
        /// </summary>
        public TContainer IgnoreInstance()
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    this.CallPattern.InstanceMatcher = new AnyMatcher();
                    this.CallPattern.MethodMockNode.ReattachMethodMock();
                    return (TContainer)(object)this;
                });
        }

        /// <summary>
        /// Specifies that the arrangement will be respected regardless of the thread
        /// on which the call to the arranged member happens.
        /// </summary>
        /// <remarks>
        /// This is only needed for arrangements of static members. Arrangements on
        /// instance members are always respected, regardless of the current thread.
        /// 
        /// Cross-thread arrangements are active as long as the current context
        /// (test method) is on the call stack. Be careful when arranging
        /// static members cross-thread because the effects of the arrangement may
        /// affect and even crash the testing framework.
        /// </remarks>
        public IAssertable OnAllThreads()
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    this.Repository.InterceptGlobally(this.CallPattern.Method);
                    return this;
                });
        }

        /// <summary>
        /// Specifies an additional condition that must be true for this arrangement to be
        /// considered when the arranged member is called. This condition is evaluated in addition
        /// to the conditions imposed by any argument matchers in the arrangement.
        /// 
        /// This method allows a more general way of matching arrangements than argument matchers do.
        /// </summary>
        /// <param name="condition">A function that should return 'true' when this
        /// arrangement should be considered and 'false' if this arrangement doesn't match the user criteria.</param>
        public TContainer When(Func<bool> condition)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                this.SetAcceptCondition(condition);
                return (TContainer)(object)this;
            });
        }

        private void SetAcceptCondition(Delegate condition)
        {
            ((IMethodMock)this).AcceptCondition = new ImplementationOverrideBehavior(condition, false);
        }

        /// <summary>
        /// Specifies to ignore any argument for the target call.
        /// </summary>
        /// <returns>Func or Action Container</returns>
        public TContainer IgnoreArguments()
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var callPattern = this.CallPattern;
                for (int i = 0; i < callPattern.ArgumentMatchers.Count; i++)
                {
                    callPattern.ArgumentMatchers[i] = new AnyMatcher();
                }

#if !PORTABLE
                this.Repository.UpdateMockDebugView(callPattern.Method, callPattern.ArgumentMatchers.ToArray());
#endif

                callPattern.MethodMockNode.ReattachMethodMock();

                return (TContainer)(object)this;
            });
        }

        /// <summary>
        /// Removes this arrangement. Its side effects will no longer be executed and its expectations will not be asserted.
        /// </summary>
        public void Dispose()
        {
            ProfilerInterceptor.GuardInternal(() => CallPattern.MethodMockNode.DetachMethodMock());
        }

        private IDisposable SetOccurrenceBounds(int? lowerBound, int? upperBound, string message)
        {
            this.occurences.SetBounds(lowerBound, upperBound, message);
            return this;
        }
    }
}
