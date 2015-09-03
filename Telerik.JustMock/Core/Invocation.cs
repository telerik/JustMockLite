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
using System.Reflection;
using System.Text;

namespace Telerik.JustMock.Core
{
	/// <summary>
	/// An implementation detail interface. Not intended for external usage.
	/// </summary>
	public sealed class Invocation
	{
		private MemberInfo member;

		private object returnValue;
		private bool isReturnValueSet;

		internal object Instance { get; private set; }

		internal object[] Args { get; private set; }

		#region Output properties

		internal object ReturnValue
		{
			get { return this.returnValue; }
			set
			{
				this.returnValue = value;
				isReturnValueSet = true;
			}
		}

		internal bool IsReturnValueSet
		{
			get { return this.isReturnValueSet; }
		}

		internal bool CallOriginal { get; set; }
		internal bool UserProvidedImplementation { get; set; }
		internal Action ExceptionThrower { get; set; }

		#endregion

		internal bool InArrange { get; set; }
		internal bool Recording { get; set; }
		internal bool RetainBehaviorDuringRecording { get; set; }
		internal MocksRepository Repository { get; set; }

		internal IMockMixin MockMixin { get; private set; }

		internal Type ReturnType
		{
			get
			{
				var asField = member as FieldInfo;
				if (asField != null)
					return Args.Length == 0 ? asField.FieldType : typeof(void);

				var asMethod = member as MethodInfo;
				return asMethod != null ? asMethod.ReturnType : typeof(void);
			}
		}

		internal Invocation(object instance, MemberInfo member, object[] args)
		{
			this.Instance = instance;
			this.Member = member;
			this.Args = args;

			this.MockMixin = member.IsExtensionMethod() && args.Length >= 1
				? MocksRepository.GetMockMixin(args[0], null)
				: MocksRepository.GetMockMixin(instance, member.DeclaringType);
		}

		internal void ThrowExceptionIfNecessary()
		{
			if (ExceptionThrower != null)
				ExceptionThrower();
		}

		internal MemberInfo Member
		{
			get
			{
				return this.member;
			}
			private set
			{
				var asMethod = value as MethodBase;
				if (asMethod != null)
				{
					if (asMethod.ContainsGenericParameters)
						throw new ArgumentException("Invocation method must be a concrete method");
				}

				var asMethodInfo = value as MethodInfo;
				if (asMethodInfo != null)
					value = asMethodInfo.NormalizeComInterfaceMethod();

				this.member = value;
			}
		}

		internal string InputToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("{0}.{1}(", Instance != null ? MockingUtil.GetUnproxiedType(Instance) : member.DeclaringType, member.Name);
			sb.Append(", ".Join(Args));
			sb.Append(")");
			return sb.ToString();
		}
	}
}
