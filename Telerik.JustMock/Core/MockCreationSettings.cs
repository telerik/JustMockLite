/*
 JustMock Lite
 Copyright © 2010-2015,2108 Progress Software Corporation

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
using System.Reflection.Emit;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Setup;

namespace Telerik.JustMock.Core
{
    internal class MockCreationSettings
    {
        private static readonly Behavior DefaultBehavior = Behavior.RecursiveLoose;

        internal object[] Args { get; set; }

        internal IEnumerable<object> Mixins { get; set; }

        internal IEnumerable<IBehavior> SupplementaryBehaviors { get; set; }

        internal IEnumerable<IBehavior> FallbackBehaviors { get; set; }

        internal Type[] AdditionalMockedInterfaces { get; set; }

        internal bool MockConstructorCall { get; set; }

        internal bool MustCreateProxy { get; set; }

        internal IEnumerable<CustomAttributeBuilder> AdditionalProxyTypeAttributes { get; set; }

        internal Expression<Predicate<MethodInfo>> InterceptorFilter { get; set; }

        internal static MockCreationSettings GetSettings(object[] constructorArgs, Behavior? behavior,
            Type[] additionalMockedInterfaces, bool? mockConstructorCall, IEnumerable<CustomAttributeBuilder> additionalProxyTypeAttributes = null,
            List<IBehavior> supplementaryBehaviors = null, List<IBehavior> fallbackBehaviors = null, List<object> mixins = null, Expression<Predicate<MethodInfo>> interceptorFilter = null)
        {
            if (behavior == null)
                behavior = DefaultBehavior;

            MockCreationSettings settings = MockCreationSettings.DissectBehavior(behavior.Value, constructorArgs, mockConstructorCall);
            settings.AdditionalMockedInterfaces = additionalMockedInterfaces;
            settings.AdditionalProxyTypeAttributes = additionalProxyTypeAttributes;
            settings.InterceptorFilter = interceptorFilter;

            if (supplementaryBehaviors != null)
            {
                settings.SupplementaryBehaviors = new List<IBehavior>(settings.SupplementaryBehaviors.Concat(supplementaryBehaviors));
            }

            if (fallbackBehaviors != null)
            {
                settings.FallbackBehaviors = new List<IBehavior>(settings.FallbackBehaviors.Concat(fallbackBehaviors));
            }

            if (mixins != null)
            {
                settings.Mixins = new List<object>(settings.Mixins.Concat(mixins));
            }

            return settings;
        }

        /// <summary>
        /// Creates instance <see cref="MockCreationSettings"/> for the default behavior <see cref="Behavior.RecursiveLoose"/>.
        /// </summary>
        /// <returns><see cref="MockCreationSettings"/> instance</returns>
        internal static MockCreationSettings GetSettings()
        {
            MockCreationSettings settings = MockCreationSettings.DissectBehavior(MockCreationSettings.DefaultBehavior, constructorArgs: null, mockConstructorCall: null);
            return settings;
        }

        /// <summary>
        /// Creates instance <see cref="MockCreationSettings"/> for the specified behavior.
        /// </summary>
        /// <param name="behavior">Behavior for which to create the settings</param>
        /// <returns><see cref="MockCreationSettings"/> instance</returns>
        internal static MockCreationSettings GetSettings(Behavior behavior)
        {
            MockCreationSettings settings = MockCreationSettings.DissectBehavior(behavior, constructorArgs: null, mockConstructorCall: null);
            return settings;
        }

        private static MockCreationSettings DissectBehavior(Behavior behavior, object[] constructorArgs, bool? mockConstructorCall)
        {
            var supplementaryBehaviors = new List<IBehavior>();
            var fallbackBehaviors = new List<IBehavior>();
            var mixins = new List<object>();

            mixins.Add(new MockingBehaviorConfiguration { Behavior = behavior });

            var eventStubs = new EventStubsBehavior();
            mixins.Add(eventStubs.CreateMixin());

            switch (behavior)
            {
                case Behavior.RecursiveLoose:
                case Behavior.Loose:
                    fallbackBehaviors.Add(eventStubs);
                    fallbackBehaviors.Add(new PropertyStubsBehavior());
                    fallbackBehaviors.Add(new CallOriginalObjectMethodsBehavior());
                    fallbackBehaviors.Add(new RecursiveMockingBehavior(behavior == Behavior.RecursiveLoose
                        ? RecursiveMockingBehaviorType.ReturnMock : RecursiveMockingBehaviorType.ReturnDefault));
                    fallbackBehaviors.Add(new StaticConstructorMockBehavior());
                    fallbackBehaviors.Add(new ExecuteConstructorBehavior());
                    break;
                case Behavior.Strict:
                    fallbackBehaviors.Add(eventStubs);
                    fallbackBehaviors.Add(new RecursiveMockingBehavior(RecursiveMockingBehaviorType.OnlyDuringAnalysis));
                    fallbackBehaviors.Add(new StaticConstructorMockBehavior());
                    fallbackBehaviors.Add(new ExecuteConstructorBehavior());
                    fallbackBehaviors.Add(new StrictBehavior(throwOnlyOnValueReturningMethods: false));
                    supplementaryBehaviors.Add(new StrictBehavior(throwOnlyOnValueReturningMethods: true));
                    break;
                case Behavior.CallOriginal:
                    fallbackBehaviors.Add(new CallOriginalBehavior());
                    fallbackBehaviors.Add(eventStubs);
                    fallbackBehaviors.Add(new RecursiveMockingBehavior(RecursiveMockingBehaviorType.OnlyDuringAnalysis));
                    fallbackBehaviors.Add(new StaticConstructorMockBehavior());
                    fallbackBehaviors.Add(new ExecuteConstructorBehavior());
                    break;
            }

            if (!mockConstructorCall.HasValue)
            {
                switch (behavior)
                {
                    case Behavior.RecursiveLoose:
                    case Behavior.Loose:
                    case Behavior.Strict:
                        mockConstructorCall = constructorArgs == null;
                        break;
                    case Behavior.CallOriginal:
                        mockConstructorCall = false;
                        break;
                }
            }

            return new MockCreationSettings
            {
                Args = constructorArgs,
                Mixins = mixins,
                SupplementaryBehaviors = supplementaryBehaviors,
                FallbackBehaviors = fallbackBehaviors,
                MockConstructorCall = mockConstructorCall.Value,
            };
        }
    }
}
