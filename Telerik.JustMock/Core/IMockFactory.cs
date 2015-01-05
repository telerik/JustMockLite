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
		ProxyTypeInfo CreateClassProxyType(Type classToProxy, MocksRepository repository, MockCreationSettings settings);
	}
}
