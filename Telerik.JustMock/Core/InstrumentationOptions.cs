using System;
using System.Linq;

namespace Telerik.JustMock.Core
{
	internal class OptionNameAttribute : Attribute
	{
		public string Name { get; set; }

		public OptionNameAttribute(string name)
		{
			Name = name;
		}

		public static string NameFromOption(InstrumentationOptions option)
		{
			var optionField = typeof(InstrumentationOptions).GetFields()
				.Where(f => f.IsStatic && ((InstrumentationOptions)f.GetValue(null) & option) != 0)
				.FirstOrDefault();
			if (optionField == null)
				return "???";

			var attr = (OptionNameAttribute)optionField.GetCustomAttributes(typeof(OptionNameAttribute), false)[0];
			return attr.Name;
		}
	}

	[Flags]
	internal enum InstrumentationOptions
	{
		ioDefault = 0,

		[OptionName("fields")]
		ioFieldMocking = 0x01,
	}
}
