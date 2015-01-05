using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.JustMock
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class MockedTypeAttribute : Attribute
	{
		public MockedTypeAttribute(Type mockedType)
		{
		}

		public MockedTypeAttribute(string mockedTypeName, string assemblyName)
		{
		}
	}
}
