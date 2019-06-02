/*
 JustMock Lite
 Copyright © 2010-2015 Progress Software Corporation

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
using System.Text;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Expectations.Abstraction;
using Telerik.JustMock.Expectations.DynaMock;


namespace Telerik.JustMock.Expectations
{
	internal sealed class NonPublicExpectation : INonPublicExpectation
	{

        private MethodInfo GetGenericMethodInstance(object target, Type returnType, string memberName, Type[] typeArguments, params object[] args)
        {
            var type = target.GetType();
            return GetGenericMethodInstance(type, returnType, memberName, typeArguments, args);
        }

        private MethodInfo GetGenericMethodInstance(Type targetType, Type returnType, string memberName, Type[] typeArguments, params object[] args)
        {
            var type = targetType;
            var mixin = MocksRepository.GetMockMixin(targetType, null);
            if (mixin != null)
                type = mixin.DeclaringType;

            var method = MockingUtil.GetGenericMethodInstanceByName(type, returnType, memberName, typeArguments, ref args);

            return method;
        }


        public ActionExpectation Arrange(object target, string memberName, Type[] typeArguments, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(target, typeof(void), memberName, typeArguments, args);
                return MockingContext.CurrentRepository.Arrange(target, method, args, () => new ActionExpectation());
            });
        }

        public FuncExpectation<TReturn> Arrange<TReturn>(object target, string memberName, Type[] typeArguments, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(target, typeof(TReturn), memberName, typeArguments, args);
                return MockingContext.CurrentRepository.Arrange(target, method, args, () => new FuncExpectation<TReturn>());
            });
        }

        public ActionExpectation Arrange<T>(string memberName, Type[] typeArguments, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return Arrange(typeof(T), memberName, typeArguments, args);
            });
        }

        public ActionExpectation Arrange(Type targetType, string memberName, Type[] typeArguments, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(targetType, typeof(void), memberName, typeArguments, args);

                return MockingContext.CurrentRepository.Arrange(targetType, method, args, () => new ActionExpectation());
            });
        }

        public FuncExpectation<TReturn> Arrange<T, TReturn>(string memberName, Type[] typeArguments, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return Arrange<TReturn>(typeof(T), memberName, typeArguments, args);
            });
        }

        public FuncExpectation<TReturn> Arrange<TReturn>(Type targetType, string memberName, Type[] typeArguments, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(targetType, typeof(TReturn), memberName, typeArguments, args);

                return MockingContext.CurrentRepository.Arrange(targetType, method, args, () => new FuncExpectation<TReturn>());
            });
        }

        public void Assert(object target, string memberName, Type[] typeArguments, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(target, typeof(void), memberName, typeArguments, args);
                this.Assert(target, method, args);
            });
        }
        public void Assert<TReturn>(object target, string memberName, Type[] typeArguments, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(target, typeof(TReturn), memberName, typeArguments, args);
                this.Assert(target, method, args);
            });
        }

        public void Assert(object target, string memberName, Type[] typeArguments, Occurs occurs, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(target, typeof(void), memberName, typeArguments, args);
                this.Assert(target, method, occurs, args);
            });
        }

        public void Assert<TReturn>(object target, string memberName, Type[] typeArguments, Occurs occurs, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(target, typeof(TReturn), memberName, typeArguments, args);
                this.Assert(target, method, occurs, args);
            });
        }

        public void Assert<T>(string memberName, Type[] typeArguments, Occurs occurs, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(typeof(T), typeof(void), memberName, typeArguments, args);
                this.Assert(method, occurs, args);
            });
        }

        public void Assert(Type targetType, string memberName, Type[] typeArguments, Occurs occurs, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(targetType, typeof(void), memberName, typeArguments, args);
                this.Assert(method, occurs, args);
            });
        }

        public void Assert<T>(string memberName, Type[] typeArguments, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(typeof(T), typeof(void), memberName, typeArguments, args);
                this.Assert(method, args);
            });
        }

        public void Assert(Type targetType, string memberName, Type[] typeArguments, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(targetType, typeof(void), memberName, typeArguments, args);
                this.Assert(method, args);
            });
        }

        public void Assert<TReturn>(Type targetType, string memberName, Type[] typeArguments, Occurs occurs, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(targetType, typeof(TReturn), memberName, typeArguments, args);
                this.Assert(method, occurs, args);
            });
        }

        public void Assert<TReturn>(Type targetType, string memberName, Type[] typeArguments, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(targetType, typeof(TReturn), memberName, typeArguments, args);
                this.Assert(method, args);
            });
        }

        public void Assert<T, TReturn>(string memberName, Type[] typeArguments, Occurs occurs, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(typeof(T), typeof(TReturn), memberName, typeArguments);
                this.Assert(method, occurs, args);
            });
        }

        public void Assert<T, TReturn>(string memberName, Type[] typeArguments, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(typeof(T), typeof(TReturn), memberName, typeArguments, args);
                this.Assert(method, args);
            });
        }


        public int GetTimesCalled(object target, string memberName, Type[] typeArguments, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(target, typeof(void), memberName, typeArguments, args);
                return this.GetTimesCalled(method, args);
            });
        }

        public int GetTimesCalled(Type type, string memberName, Type[] typeArguments, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var method = GetGenericMethodInstance(type, typeof(void), memberName, typeArguments, args);
                return this.GetTimesCalled(method, args);
            });
        }




        public ActionExpectation Arrange(object target, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var method = MockingUtil.GetMethodByName(type, typeof(void), memberName, ref args);
					return MockingContext.CurrentRepository.Arrange(target, method, args, () => new ActionExpectation());
				});
		}

		public ActionExpectation Arrange(object target, MethodInfo method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(target, method, args, () => new ActionExpectation()));
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(object target, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var method = MockingUtil.GetMethodByName(type, typeof(TReturn), memberName, ref args);
					return MockingContext.CurrentRepository.Arrange(target, method, args, () => new FuncExpectation<TReturn>());
				});
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(object target, MethodInfo method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(target, method, args, () => new FuncExpectation<TReturn>()));
		}
		
		Type GetTypeFromInstance(object instance)
		{
			Type type = instance.GetType();
			if (type.IsProxy())
				type = type.BaseType;
			return type;
		}

		PropertyInfo GetNonPublicProperty(Type type, string propertyName)
		{
			var mockedProperty = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
				.FirstOrDefault(property => property.Name == propertyName);
			return mockedProperty;
		}

		private static string BuildMissingPropertyMessage(Type type, string propertyName)
		{
			return String.Format("Property '{0}' was not found on type '{1}'.", propertyName, type);
		}

		public ActionExpectation ArrangeSet(object target, string propertyName, object value)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = GetTypeFromInstance(target);
				if (GetNonPublicProperty(type, propertyName) == null)
				{
					throw new MissingMemberException(BuildMissingPropertyMessage(type, propertyName));
				}
				return Arrange(target, propertyName, value);
			});
		}

		public ActionExpectation ArrangeSet<T>(string propertyName, object value)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return ArrangeSet(typeof(T), propertyName, value);
			});
		}

		public ActionExpectation ArrangeSet(Type type, string propertyName, object value)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				if (GetNonPublicProperty(type, propertyName) == null)
				{
					throw new MissingMemberException(BuildMissingPropertyMessage(type, propertyName));
				}
				return Arrange(type, propertyName, value);
			});
		}

		public void Assert<TReturn>(object target, string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var message = MockingUtil.GetAssertionMessage(args);
					var method = MockingUtil.GetMethodByName(type, typeof(TReturn), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, null);
				});
		}

		public void Assert(object target, MethodInfo method, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = MockingUtil.GetAssertionMessage(args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, null);
				});
		}

		public void Assert(object target, string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var message = MockingUtil.GetAssertionMessage(args);
					var method = MockingUtil.GetMethodByName(type, typeof(void), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, null);
				});
		}

		public void Assert<TReturn>(object target, string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var message = MockingUtil.GetAssertionMessage(args);
					var method = MockingUtil.GetMethodByName(type, typeof(TReturn), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, occurs);
				});
		}

		public void Assert(object target, MethodInfo method, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = MockingUtil.GetAssertionMessage(args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, occurs);
				});
		}

		public void Assert(object target, string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var message = MockingUtil.GetAssertionMessage(args);
					var method = MockingUtil.GetMethodByName(type, typeof(void), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, occurs);
				});
		}

		public int GetTimesCalled(object target, MethodInfo method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				MockingContext.CurrentRepository.GetTimesCalledFromMethodInfo(target, method, args));
		}

		public int GetTimesCalled(object target, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var type = target.GetType();
				var mixin = MocksRepository.GetMockMixin(target, null);
				if (mixin != null)
					type = mixin.DeclaringType;

				var method = MockingUtil.GetMethodByName(type, typeof(void), memberName, ref args);
				return MockingContext.CurrentRepository.GetTimesCalledFromMethodInfo(target, method, args);
			});
		}

#if !LITE_EDITION

		public FuncExpectation<TReturn> Arrange<T, TReturn>(string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = MockingUtil.GetMethodByName(typeof(T), typeof(TReturn), memberName, ref args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TReturn>());
				});
		}

		public ActionExpectation Arrange<T>(string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = MockingUtil.GetMethodByName(typeof(T), typeof(void), memberName, ref args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new ActionExpectation());
				});
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(Type targetType, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = MockingUtil.GetMethodByName(targetType, typeof(TReturn), memberName, ref args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TReturn>());
				});
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(MethodBase method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TReturn>()));
		}

		public ActionExpectation Arrange(Type targetType, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = MockingUtil.GetMethodByName(targetType, typeof(void), memberName, ref args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new ActionExpectation());
				});
		}

		public ActionExpectation Arrange(MethodBase method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(null, method, args, () => new ActionExpectation()));
		}

		public void Assert<T>(string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = MockingUtil.GetAssertionMessage(args);
					var method = MockingUtil.GetMethodByName(typeof(T), typeof(void), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, occurs);
				});
		}

		public void Assert(MethodBase method, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = MockingUtil.GetAssertionMessage(args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, occurs);
				});
		}

		public void Assert<T, TReturn>(string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = MockingUtil.GetAssertionMessage(args);
					var method = MockingUtil.GetMethodByName(typeof(T), typeof(TReturn), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, occurs);
				});
		}

		public void Assert<T>(string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = MockingUtil.GetAssertionMessage(args);
					var method = MockingUtil.GetMethodByName(typeof(T), typeof(void), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, null);
				});
		}

		public void Assert(MethodBase method, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = MockingUtil.GetAssertionMessage(args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, null);
				});
		}

		public void Assert<T, TReturn>(string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = MockingUtil.GetAssertionMessage(args);
					var method = MockingUtil.GetMethodByName(typeof(T), typeof(TReturn), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, null);
				});
		}

		public void Assert(Type targetType, string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = MockingUtil.GetAssertionMessage(args);
					var method = MockingUtil.GetMethodByName(targetType, typeof(void), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, occurs);
				});
		}

		public void Assert<TReturn>(Type targetType, string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = MockingUtil.GetAssertionMessage(args);
					var method = MockingUtil.GetMethodByName(targetType, typeof(TReturn), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, occurs);
				});
		}

		public void Assert(Type targetType, string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = MockingUtil.GetAssertionMessage(args);
					var method = MockingUtil.GetMethodByName(targetType, typeof(void), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, null);
				});
		}

		void INonPublicExpectation.Assert<TReturn>(Type targetType, string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = MockingUtil.GetAssertionMessage(args);
					var method = MockingUtil.GetMethodByName(targetType, typeof(TReturn), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, null);
				});
		}

		public int GetTimesCalled(MethodBase method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				MockingContext.CurrentRepository.GetTimesCalledFromMethodInfo(null, method, args));

		}

		public int GetTimesCalled(Type type, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var method = MockingUtil.GetMethodByName(type, typeof(void), memberName, ref args);
				return MockingContext.CurrentRepository.GetTimesCalledFromMethodInfo(null, method, args);
			});
		}
#endif

		#region Raise event

		public void Raise(object instance, EventInfo eventInfo, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => RaiseEventBehavior.RaiseEventImpl(instance, eventInfo, args));
		}

		public void Raise(EventInfo eventInfo, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => RaiseEventBehavior.RaiseEventImpl(null, eventInfo, args));
		}

		public void Raise(object instance, string eventName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				RaiseEventBehavior.RaiseEventImpl(instance, MockingUtil.GetUnproxiedType(instance).GetEvent(eventName,
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), args);
			});
		}

		public void Raise(Type type, string eventName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				RaiseEventBehavior.RaiseEventImpl(null, type.GetEvent(eventName,
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static), args);
			});
		}
		#endregion

		public PrivateAccessor MakePrivateAccessor(object instance)
		{
			return new PrivateAccessor(instance);
		}

		public PrivateAccessor MakeStaticPrivateAccessor(Type type)
		{
			return PrivateAccessor.ForType(type);
		}

		public dynamic Wrap(object instance)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
#if !LITE_EDITION
					Mock.Intercept(instance.GetType());
#endif
					return new ExpressionContainer(Expression.Constant(instance, MockingUtil.GetUnproxiedType(instance)));
				}
			);
		}

		public dynamic WrapType(Type type)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
#if !LITE_EDITION
					Mock.Intercept(type);
#endif
					return new ExpressionContainer(Expression.Constant(type.GetDefaultValue(), type)) { IsStatic = true };
				}
			);
		}

		public ActionExpectation Arrange(dynamic dynamicExpression)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				MockingContext.CurrentRepository.Arrange(((IExpressionContainer)dynamicExpression).ToLambda(), () => new ActionExpectation())
			);
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(dynamic dynamicExpression)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				MockingContext.CurrentRepository.Arrange(((IExpressionContainer)dynamicExpression).ToLambda(), () => new FuncExpectation<TReturn>())
			);
		}

		public void Assert(dynamic dynamicExpression, Occurs occurs, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() =>
				MockingContext.CurrentRepository.Assert(message, null, ((IExpressionContainer)dynamicExpression).ToLambda(), null, occurs)
			);
		}

		public void Assert(dynamic dynamicExpression, Args args, Occurs occurs, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() =>
				MockingContext.CurrentRepository.Assert(message, null, ((IExpressionContainer)dynamicExpression).ToLambda(), args, occurs)
			);
		}

#if !PORTABLE
		public INonPublicRefReturnExpectation RefReturn
		{
			get { return ProfilerInterceptor.GuardInternal(() => new NonPublicRefReturnExpectation()); }
		}
#endif
	}
}
