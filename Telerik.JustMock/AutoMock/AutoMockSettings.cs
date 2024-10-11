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
using Telerik.JustMock.AutoMock.Ninject;
using Telerik.JustMock.Core;

namespace Telerik.JustMock.AutoMock
{
    /// <summary>
    /// Contains settings that modify the auto-mocking container behavior. Create an instance of this class, set all relevant properties
    /// and pass it to <see cref="Telerik.JustMock.AutoMock.MockingContainer..ctor"/>
    /// </summary>
    public class AutoMockSettings : NinjectSettings
    {
        /// <summary>
        /// The behavior of the mocks created by the container.
        /// </summary>
        public Behavior MockBehavior
        {
            get { return ProfilerInterceptor.GuardInternal(() => this.Get<Behavior>("MockBehavior", Behavior.RecursiveLoose)); }
            set { ProfilerInterceptor.GuardInternal(() => this.Set("MockBehavior", value)); }
        }

        /// <summary>
        /// Specifies the constructor overload which should be injected. The constructor
        /// to inject will be the one that has the exact same parameter types as this setting's value.
        /// If this setting is not specified, the default constructor scoring algorithm will be used
        /// to select the most appropriate constructor.
        /// </summary>
        public Type[] ConstructorArgTypes
        {
            get { return ProfilerInterceptor.GuardInternal(() => this.Get<Type[]>("ConstructorArgTypes", null)); }
            set { ProfilerInterceptor.GuardInternal(() => this.Set("ConstructorArgTypes", value)); }
        }
    }
}
