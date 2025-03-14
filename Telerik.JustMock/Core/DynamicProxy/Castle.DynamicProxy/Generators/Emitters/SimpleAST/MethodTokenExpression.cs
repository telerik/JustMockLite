// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    using Castle.DynamicProxy.Tokens;

    internal class MethodTokenExpression : IExpression
    {
        private readonly MethodInfo method;

        public MethodTokenExpression(MethodInfo method)
        {
            this.method = method;
            Debug.Assert(method.DeclaringType != null);  // DynamicProxy isn't using global methods nor `DynamicMethod`
        }

        public void Emit(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldtoken, method);
            gen.Emit(OpCodes.Ldtoken, method.DeclaringType);

            var minfo = MethodBaseMethods.GetMethodFromHandle;
            gen.Emit(OpCodes.Call, minfo);
            gen.Emit(OpCodes.Castclass, typeof(MethodInfo));
        }
    }
}