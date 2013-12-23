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
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Setup;

namespace Telerik.JustMock
{
	/// <summary>
	/// Specifies the behavior of the mock.
	/// </summary>
	public enum Behavior
	{
		/// <summary>
		/// Specifies that by default mock calls will behave like a stub, unless explicitly setup.
		/// </summary>
		Loose,
		
		/// <summary>
		/// Specifies that by default mock calls will return mock objects, unless explicitly setup.
		/// </summary>
		RecursiveLoose,
		
		/// <summary>
		/// Specifies that any calls made on the mock 
		/// will throw an exception if not explictly set.
		/// </summary>
		Strict,
		
		/// <summary>
		/// Specifies that by default all calls made on mock will invoke its 
		/// corresponding original member unless some expecations are set.
		/// </summary>
		CallOriginal
	}

	internal static class MockBuilder
	{
		private static readonly Behavior DefaultBehavior = Behavior.RecursiveLoose;

		public static object Create(this MocksRepository repository, Type type, object[] constructorArgs, Behavior? behavior,
			Type[] additionalMockedInterfaces, bool? mockConstructorCall, IEnumerable<CustomAttributeBuilder> additionalProxyTypeAttributes = null,
			List<IBehavior> supplementaryBehaviors = null, List<IBehavior> fallbackBehaviors = null, List<object> mixins = null, Expression<Predicate<MethodInfo>> interceptorFilter = null)
		{
			if (behavior == null)
				behavior = DefaultBehavior;

			if (supplementaryBehaviors == null)
				supplementaryBehaviors = new List<IBehavior>();
			if (fallbackBehaviors == null)
				fallbackBehaviors = new List<IBehavior>();
			if (mixins == null)
				mixins = new List<object>();

			DissectBehavior(behavior.Value, mixins, supplementaryBehaviors, fallbackBehaviors, constructorArgs, ref mockConstructorCall);

			return repository.Create(type,
				new MockCreationSettings
				{
					Args = constructorArgs,
					Mixins = mixins,
					SupplementaryBehaviors = supplementaryBehaviors,
					FallbackBehaviors = fallbackBehaviors,
					AdditionalMockedInterfaces = additionalMockedInterfaces,
					MockConstructorCall = mockConstructorCall.Value,
					AdditionalProxyTypeAttributes = additionalProxyTypeAttributes,
					InterceptorFilter = interceptorFilter,
				});
		}

		public static IMockMixin CreateExternalMockMixin(this MocksRepository repository, Type type, object mockObject, Behavior? behavior)
		{
			if (behavior == null)
				behavior = DefaultBehavior;
			
			var supplementaryBehaviors = new List<IBehavior>();
			var fallbackBehaviors = new List<IBehavior>();
			var mixins = new List<object>();

			bool? mockCtorCall = null;
			DissectBehavior(behavior.Value, mixins, supplementaryBehaviors, fallbackBehaviors, null, ref mockCtorCall);
			return repository.CreateExternalMockMixin(type, mockObject, mixins, supplementaryBehaviors, fallbackBehaviors);
		}

		public static void InterceptStatics(this MocksRepository repository, Type type, Behavior? behavior, bool mockStaticConstructor)
		{
			if (behavior == null)
				behavior = DefaultBehavior;

			var supplementaryBehaviors = new List<IBehavior>();
			var fallbackBehaviors = new List<IBehavior>();
			var mixins = new List<object>();

			bool? mockCtorCall = null;
			DissectBehavior(behavior.Value, mixins, supplementaryBehaviors, fallbackBehaviors, null, ref mockCtorCall);
			repository.InterceptStatics(type, mixins, supplementaryBehaviors, fallbackBehaviors, mockStaticConstructor);
		}

		private static void DissectBehavior(Behavior behavior, List<object> mixins, List<IBehavior> supplementaryBehaviors, List<IBehavior> fallbackBehaviors, object[] constructorArgs, ref bool? mockConstructorCall)
		{
			mixins.Add(new MockingBehaviorConfiguration { Behavior = behavior });
			
			var eventStubs = new EventStubsBehavior();
			mixins.Add(eventStubs.CreateMixin());
			fallbackBehaviors.Add(eventStubs);

			switch (behavior)
			{
				case Behavior.RecursiveLoose:
				case Behavior.Loose:
					fallbackBehaviors.Add(new PropertyStubsBehavior());
					fallbackBehaviors.Add(new CallOriginalObjectMethodsBehavior());
					fallbackBehaviors.Add(new RecursiveMockingBehavior(behavior == Behavior.RecursiveLoose ? RecursiveMockingBehaviorType.ReturnMock : RecursiveMockingBehaviorType.ReturnDefault));
					fallbackBehaviors.Add(new StaticConstructorMockBehavior());
					fallbackBehaviors.Add(new ExecuteConstructorBehavior());
					break;
				case Behavior.Strict:
					fallbackBehaviors.Add(new RecursiveMockingBehavior(RecursiveMockingBehaviorType.OnlyDuringAnalysis));
					fallbackBehaviors.Add(new StaticConstructorMockBehavior());
					fallbackBehaviors.Add(new ExecuteConstructorBehavior());
					fallbackBehaviors.Add(new StrictBehavior(false));
					supplementaryBehaviors.Add(new StrictBehavior(true));
					break;
				case Behavior.CallOriginal:
					fallbackBehaviors.Add(new CallOriginalBehavior());
					fallbackBehaviors.Add(new RecursiveMockingBehavior(RecursiveMockingBehaviorType.OnlyDuringAnalysis));
					fallbackBehaviors.Add(new StaticConstructorMockBehavior());
					fallbackBehaviors.Add(new ExecuteConstructorBehavior());
					break;
			}

			if (!mockConstructorCall.HasValue)
			{
#if !SILVERLIGHT
				switch (behavior)
				{
					case Behavior.RecursiveLoose:
					case Behavior.Loose:
					case Behavior.Strict:
						mockConstructorCall = constructorArgs == null || constructorArgs.Length == 0;
						break;
					case Behavior.CallOriginal:
						mockConstructorCall = false;
						break;
				}
#else
				mockConstructorCall = false;
#endif
			}
		}
	}
}
