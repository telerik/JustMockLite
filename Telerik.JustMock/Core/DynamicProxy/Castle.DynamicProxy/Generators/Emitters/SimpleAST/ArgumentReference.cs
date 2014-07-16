// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Telerik.JustMock.Core.Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	[DebuggerDisplay("argument {Type}")]
	internal class ArgumentReference : TypeReference
	{
		public ArgumentReference(Type argumentType)
			: this(argumentType, DBNull.Value)
		{ }

	    public ArgumentReference(Type argumentType, object defaultValue)
			: base(argumentType)
		{
            this.DefaultValue = defaultValue;
            ParameterAttributes = ParameterAttributes.None;
            Position = -1;
		}

	    public ArgumentReference(Type argumentType, int position, ParameterAttributes parameterAttributes)
			: base(argumentType)
		{
			Position = position;
		}

        internal object DefaultValue { get; private set; }
        internal int Position { get; set; }
        internal ParameterAttributes ParameterAttributes { get; set; }

		public override void LoadAddressOfReference(ILGenerator gen)
		{
			throw new NotSupportedException();
		}

		public override void LoadReference(ILGenerator gen)
		{
			if (Position == -1)
			{
				throw new ProxyGenerationException("ArgumentReference unitialized");
			}
			switch (Position)
			{
				case 0:
					gen.Emit(OpCodes.Ldarg_0);
					break;
				case 1:
					gen.Emit(OpCodes.Ldarg_1);
					break;
				case 2:
					gen.Emit(OpCodes.Ldarg_2);
					break;
				case 3:
					gen.Emit(OpCodes.Ldarg_3);
					break;
				default:
                    gen.Emit(OpCodes.Ldarg, Position);
					break;
			}
		}

		public override void StoreReference(ILGenerator gen)
		{
			if (Position == -1)
			{
				throw new ProxyGenerationException("ArgumentReference unitialized");
			}
			gen.Emit(OpCodes.Starg, Position);
		}
	}
}