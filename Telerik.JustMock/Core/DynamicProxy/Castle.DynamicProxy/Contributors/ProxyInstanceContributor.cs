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

namespace Telerik.JustMock.Core.Castle.DynamicProxy.Contributors
{
	using System;
	using Telerik.JustMock.Core.Castle.DynamicProxy.Generators;
	using Telerik.JustMock.Core.Castle.DynamicProxy.Generators.Emitters;
	using Telerik.JustMock.Core.Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Telerik.JustMock.Core.Castle.DynamicProxy.Internal;

	internal abstract class ProxyInstanceContributor : ITypeContributor
	{
		// TODO: this whole type (and its descendants) should be #if !SILVERLIGHT... and empty type should be used instead for SL

		protected readonly Type targetType;
		private readonly Type[] interfaces;

		protected ProxyInstanceContributor(Type targetType, Type[] interfaces)
		{
			this.targetType = targetType;
			this.interfaces = interfaces ?? Type.EmptyTypes;
		}

		protected abstract Expression GetTargetReferenceExpression(ClassEmitter emitter);

		public virtual void Generate(ClassEmitter @class, ProxyGenerationOptions options)
		{
			var interceptors = @class.GetField("__interceptors");
			ImplementProxyTargetAccessor(@class, interceptors);
			foreach (var attribute in targetType.GetNonInheritableAttributes())
			{
				@class.DefineCustomAttribute(attribute);
			}
		}

		protected void ImplementProxyTargetAccessor(ClassEmitter emitter, FieldReference interceptorsField)
		{
			var dynProxyGetTarget = emitter.CreateMethod("DynProxyGetTarget", typeof(object));

			dynProxyGetTarget.CodeBuilder.AddStatement(
				new ReturnStatement(new ConvertExpression(typeof(object), targetType, GetTargetReferenceExpression(emitter))));

			var getInterceptors = emitter.CreateMethod("GetInterceptors", typeof(IInterceptor[]));

			getInterceptors.CodeBuilder.AddStatement(
				new ReturnStatement(interceptorsField));
		}

		public void CollectElementsToProxy(IProxyGenerationHook hook, MetaType model)
		{
		}
	}
}