/*
 JustMock Lite
 Copyright © 2010-2014 Telerik AD

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
using System.Threading;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Core.MatcherTree;
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock.Expectations
{
	/// <summary>
	/// Implements common expecations.
	/// </summary>
	public partial class CommonExpectation<TContainer> : IAction<TContainer>, IInstanceScope<TContainer>, IMethodMock
	{
		private readonly List<IBehavior> behaviors = new List<IBehavior>();
		private readonly InvocationOccurrenceBehavior occurences;
		private ImplementationOverrideBehavior acceptCondition;

		MocksRepository IMethodMock.Repository { get; set; }

		IMockMixin IMethodMock.Mock { get; set; }

		bool IMethodMock.IsSequential { get; set; }
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

		protected void ProcessDoInstead(Delegate delg, bool ignoreDelegateReturnValue)
		{
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
					IWaitDuration wait = (IWaitDuration)args.FirstOrDefault(obj => obj != null && typeof(IWaitDuration).IsAssignableFrom(obj.GetType()));
					if (wait != null)
					{
						args = args.Where(obj => obj != wait).ToArray();
					}

					this.ProcessRaises(eventExpression, instance => new Func<object[]>(() =>
						{
							if (wait != null)
								Thread.Sleep(wait.Miliseconds);
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
		/// Specifies that the mock call should be invoked to pass <see cref="Mock.Assert{T}(T)"/>
		/// </summary>
		public void MustBeCalled()
		{
			ProfilerInterceptor.GuardInternal(() => occurences.SetBounds(1, null));
		}

		#endregion

		#region Occurrence

		/// <summary>
		/// Specifies how many times the call should occur.
		/// </summary>
		/// <param name="numberOfTimes">Specified number of times</param>
		public void Occurs(int numberOfTimes)
		{
			ProfilerInterceptor.GuardInternal(() => occurences.SetBounds(numberOfTimes, numberOfTimes));
		}

		/// <summary>
		/// Specifies how many times atleast the call should occur.
		/// </summary>
		/// <param name="numberOfTimes">Specified number of times</param>
		public void OccursAtLeast(int numberOfTimes)
		{
			ProfilerInterceptor.GuardInternal(() => occurences.SetBounds(numberOfTimes, null));
		}

		/// <summary>
		/// Specifies how many times maximum the call can occur.
		/// </summary>
		/// <param name="numberOfTimes">Specified number of times</param>
		public void OccursAtMost(int numberOfTimes)
		{
			ProfilerInterceptor.GuardInternal(() => occurences.SetBounds(null, numberOfTimes));
		}

		/// <summary>
		/// Specifies that the call must occur once.
		/// </summary>
		public void OccursOnce()
		{
			ProfilerInterceptor.GuardInternal(() => occurences.SetBounds(1, 1));
		}

		/// <summary>
		/// Specifies that the call must never occur.
		/// </summary>
		public void OccursNever()
		{
			ProfilerInterceptor.GuardInternal(() => occurences.SetBounds(0, 0));
		} 

		#endregion

		/// <summary>
		/// Specifies that justmock should invoke different mock instance for each setup.
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
		public IOccurrence InOrder()
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.behaviors.Add(new InOrderBehavior(this.Mock));
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
				var callPattern = ((IMethodMock)this).CallPattern;
				for (int i = 0; i < callPattern.ArgumentMatchers.Count; i++)
				{
					callPattern.ArgumentMatchers[i] = new AnyMatcher();
				}

				callPattern.MethodMockNode.ReattachMethodMock();

				return (TContainer)(object)this;
			});
		}
	}
}
