using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.JustMock
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class MockedTypeAttribute : Attribute
	{
		public Type[] Implements { get; set; }

		public MockedTypeAttribute(Type mockedType)
		{
		}

		public MockedTypeAttribute(string mockedTypeName, string assemblyName)
		{
		}
	}
}
