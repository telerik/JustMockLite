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

namespace Telerik.JustMock.Core.Castle.DynamicProxy.Contributors
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators;
    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators.Emitters;
    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators.Emitters.SimpleAST;
    using Telerik.JustMock.Core.Castle.DynamicProxy.Tokens;

    internal class InvocationWithDelegateContributor : IInvocationCreationContributor
    {
        private readonly Type delegateType;
        private readonly MetaMethod method;
        private readonly INamingScope namingScope;
        private readonly Type targetType;

        public InvocationWithDelegateContributor(Type delegateType, Type targetType, MetaMethod method,
                                                 INamingScope namingScope)
        {
            Debug.Assert(delegateType.IsGenericType == false, "delegateType.IsGenericType == false");
            this.delegateType = delegateType;
            this.targetType = targetType;
            this.method = method;
            this.namingScope = namingScope;
        }

        public ConstructorEmitter CreateConstructor(ArgumentReference[] baseCtorArguments, AbstractTypeEmitter invocation)
        {
            var arguments = GetArguments(baseCtorArguments);
            var constructor = invocation.CreateConstructor(arguments);

            var delegateField = invocation.CreateField("delegate", delegateType);
            constructor.CodeBuilder.AddStatement(new AssignStatement(delegateField, arguments[0]));
            return constructor;
        }

        public MethodInfo GetCallbackMethod()
        {
            return delegateType.GetMethod("Invoke");
        }

        public MethodInvocationExpression GetCallbackMethodInvocation(AbstractTypeEmitter invocation, IExpression[] args,
                                                                      Reference targetField,
                                                                      MethodEmitter invokeMethodOnTarget)
        {
            var allArgs = GetAllArgs(args, targetField);
            var @delegate = (Reference)invocation.GetField("delegate");

            return new MethodInvocationExpression(@delegate, GetCallbackMethod(), allArgs);
        }

        public IExpression[] GetConstructorInvocationArguments(IExpression[] arguments, ClassEmitter proxy)
        {
            var allArguments = new IExpression[arguments.Length + 1];
            allArguments[0] = BuildDelegateToken(proxy);
            Array.Copy(arguments, 0, allArguments, 1, arguments.Length);
            return allArguments;
        }

        private FieldReference BuildDelegateToken(ClassEmitter proxy)
        {
            var callback = proxy.CreateStaticField(namingScope.GetUniqueName("callback_" + method.Method.Name), delegateType);
            var createDelegate = new MethodInvocationExpression(
                null,
                DelegateMethods.CreateDelegate,
                new TypeTokenExpression(delegateType),
                NullExpression.Instance,
                new MethodTokenExpression(method.MethodOnTarget));
            var bindDelegate = new AssignStatement(callback, new ConvertExpression(delegateType, createDelegate));

            proxy.ClassConstructor.CodeBuilder.AddStatement(bindDelegate);
            return callback;
        }

        private IExpression[] GetAllArgs(IExpression[] args, Reference targetField)
        {
            var allArgs = new IExpression[args.Length + 1];
            args.CopyTo(allArgs, 1);
            allArgs[0] = new ConvertExpression(targetType, targetField);
            return allArgs;
        }

        private ArgumentReference[] GetArguments(ArgumentReference[] baseCtorArguments)
        {
            var arguments = new ArgumentReference[baseCtorArguments.Length + 1];
            arguments[0] = new ArgumentReference(delegateType);
            baseCtorArguments.CopyTo(arguments, 1);
            return arguments;
        }
    }
}
