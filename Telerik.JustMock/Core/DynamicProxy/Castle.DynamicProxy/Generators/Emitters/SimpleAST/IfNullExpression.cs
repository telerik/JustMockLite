﻿// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
    using System.Reflection.Emit;

    internal class IfNullExpression : IExpression, IStatement
    {
        private readonly IExpressionOrStatement ifNotNull;
        private readonly IExpressionOrStatement ifNull;
        private readonly Reference reference;
        private readonly IExpression expression;

        public IfNullExpression(Reference reference, IExpressionOrStatement ifNull, IExpressionOrStatement ifNotNull = null)
        {
            this.reference = reference ?? throw new ArgumentNullException(nameof(reference));
            this.ifNull = ifNull;
            this.ifNotNull = ifNotNull;
        }

        public IfNullExpression(IExpression expression, IExpressionOrStatement ifNull, IExpressionOrStatement ifNotNull = null)
        {
            this.expression = expression ?? throw new ArgumentNullException(nameof(expression));
            this.ifNull = ifNull;
            this.ifNotNull = ifNotNull;
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

            var notNull = gen.DefineLabel();
            gen.Emit(OpCodes.Brtrue_S, notNull);
            ifNull.Emit(gen);
            gen.MarkLabel(notNull);
            if (ifNotNull != null) // yeah, I know that reads funny :)
            {
                ifNotNull.Emit(gen);
            }
        }
    }
}