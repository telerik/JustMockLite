// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;
	using System.Runtime.CompilerServices;

	using Telerik.JustMock.Core.Castle.DynamicProxy.Generators;
	using Telerik.JustMock.Core.Castle.DynamicProxy.Generators.Emitters;
	using Telerik.JustMock.Core.Castle.DynamicProxy.Internal;

	internal class WrappedClassMembersCollector : ClassMembersCollector
	{
		public WrappedClassMembersCollector(Type type, ModuleScope scope) : base(type, scope)
		{
		}

		public override void CollectMembersToProxy(IProxyGenerationHook hook)
		{
			base.CollectMembersToProxy(hook);
			CollectFields(hook);
			// TODO: perhaps we should also look for nested classes...
		}

		protected override MetaMethod GetMethodToGenerate(MethodInfo method, IProxyGenerationHook hook, bool isStandalone)
		{
#if SILVERLIGHT
			if(method.IsFamily)
			{
				// we can't proxy protected methods like this on Silverlight
				return null;
			}
#endif
			if (scope.Internals.IsAccessible(method) == false)
			{
				return null;
			}

			var accepted = AcceptMethod(method, true, hook);
			if (!accepted && !method.IsAbstract)
			{
				//we don't need to do anything...
				return null;
			}

			return new MetaMethod(method, scope, method, isStandalone, accepted, hasTarget: true);
		}

		protected bool IsGeneratedByTheCompiler(FieldInfo field)
		{
			// for example fields backing autoproperties
			return Attribute.IsDefined(field, typeof(CompilerGeneratedAttribute));
		}

		protected virtual bool IsOKToBeOnProxy(FieldInfo field)
		{
			return IsGeneratedByTheCompiler(field);
		}

		private void CollectFields(IProxyGenerationHook hook)
		{
			var fields = type.GetAllFields();
			foreach (var field in fields)
			{
				if (IsOKToBeOnProxy(field))
				{
					continue;
				}

				hook.NonProxyableMemberNotification(type, field);
			}
		}
	}
}