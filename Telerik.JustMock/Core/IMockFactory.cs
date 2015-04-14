/*
 JustMock Lite
 Copyright © 2010-2015 Telerik AD

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
using System.Text;

namespace Telerik.JustMock.Core
{
	internal interface IMockFactory
	{
		bool IsAccessible(Type type);
		object Create(Type type, MocksRepository repository, IMockMixin mockMixinImpl, MockCreationSettings settings, bool createTransparentProxy);
		Type CreateDelegateBackend(Type delegateType);
		IMockMixin CreateExternalMockMixin(IMockMixin mockMixin, IEnumerable<object> mixins);
		ProxyTypeInfo CreateClassProxyType(Type classToProxy, MocksRepository repository, MockCreationSettings settings, MockMixin mockMixinImpl);
	}
}
