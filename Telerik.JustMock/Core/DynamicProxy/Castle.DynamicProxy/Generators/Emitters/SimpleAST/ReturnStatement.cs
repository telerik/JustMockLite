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
    using System.Reflection.Emit;

    internal class ReturnStatement : IStatement
    {
        private readonly IExpression expression;
        private readonly Reference reference;

        public ReturnStatement()
        {
        }

        public ReturnStatement(Reference reference)
        {
            this.reference = reference;
        }

        public ReturnStatement(IExpression expression)
        {
            this.expression = expression;
        }

        public void Emit(ILGenerator gen)
        {
            if (reference != null)
            {
                ArgumentsUtil.EmitLoadOwnerAndReference(reference, gen);
            }
            else if (expression != null)
            {
                expression.Emit(gen);
            }

            gen.Emit(OpCodes.Ret);
        }
    }
}