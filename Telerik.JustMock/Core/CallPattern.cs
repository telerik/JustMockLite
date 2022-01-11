/*
 JustMock Lite
 Copyright © 2010-2015,2021 Progress Software Corporation

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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telerik.JustMock.Core.MatcherTree;

namespace Telerik.JustMock.Core
{
	internal class CallPattern
	{
		internal static readonly MethodInfo ObjectEqualsMethod = typeof(object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance);

		internal static readonly HashSet<MethodBase> uninterceptedMethods = new HashSet<MethodBase>
		{
			typeof(object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static),
			typeof(object).GetMethod("ReferenceEquals", BindingFlags.Public | BindingFlags.Static),
			typeof(object).GetMethod("Finalize", BindingFlags.NonPublic | BindingFlags.Instance),
		};

		public MethodMockMatcherTreeNode MethodMockNode;
		public IMatcher InstanceMatcher;
		public List<IMatcher> ArgumentMatchers = new List<IMatcher>();

		public Delegate Filter;

		private MethodBase method;
		public MethodBase Method
		{
			get
			{
				return this.method;
			}
		}

		public void SetMethod(MethodBase method, bool checkCompatibility)
		{
			if (checkCompatibility)
			{
				if (method == typeof(object).GetConstructor(MockingUtil.EmptyTypes))
					DebugView.TraceEvent(Diagnostics.IndentLevel.Warning, () => "System.Object constructor will be intercepted only in 'new' expressions, i.e. 'new object()'.");

				CheckMethodCompatibility(method);
				CheckInstrumentationAvailability(method);
			}
			this.method = method;
		}

		public static void CheckMethodCompatibility(MethodBase method)
		{
			var sigTypes = method.GetParameters().Select(p => p.ParameterType).Concat(new[] { method.GetReturnType() });
			if (sigTypes.Any(sigType =>
			{
				while (sigType.IsByRef || sigType.IsArray)
					sigType = sigType.GetElementType();
				return sigType == typeof(TypedReference);
			}))
				throw new MockException("Mocking methods with TypedReference in their signature is not supported.");
#if PORTABLE
			if (method.GetReturnType().IsByRef)
				throw new MockException("Cannot mock method with by-ref return value.");
#endif

			if (method.CallingConvention == CallingConventions.VarArgs)
				throw new MockException("Cannot mock method with __arglist.");
		}

		private static bool IsInterceptedAtTheCallSite(MethodBase method)
		{
			return method.IsConstructor;
		}

		public static void CheckInstrumentationAvailability(MethodBase value)
		{
			if (IsInterceptedAtTheCallSite(value))
			{
				return;
			}

#if !PORTABLE
			var methodImpl = value.GetMethodImplementationFlags();
			if ((((methodImpl & MethodImplAttributes.InternalCall) != 0
				   || (methodImpl & MethodImplAttributes.CodeTypeMask) != MethodImplAttributes.IL)
				  && value.Module.Assembly == typeof(object).Assembly)
				  && !value.IsInheritable())
				throw new MockException("Cannot mock a method that is implemented internally by the CLR.");
#endif

			if (!value.IsInheritable() && !ProfilerInterceptor.TypeSupportsInstrumentation(value.DeclaringType))
				throw new MockException(String.Format("Cannot mock non-inheritable member '{0}' on type '{1}' due to CLR limitations.", value, value.DeclaringType));

			if ((value.Attributes & MethodAttributes.PinvokeImpl) != 0)
			{
				if (!ProfilerInterceptor.IsProfilerAttached)
					throw new MockException("The profiler must be enabled to mock DllImport methods.");

				string fullName = value.DeclaringType.FullName + "." + value.Name;
				if ((value.Attributes & MethodAttributes.HasSecurity) != 0)
					throw new MockException(string.Format("DllImport method {0} cannot be mocked because it has security information attached.", fullName));

				// method could be the profiler generated shadow method or is inside an assembly which doesn't contain managed code (valid RVA)
				throw new MockException(string.Format("DllImport method {0} cannot be mocked due to internal limitation.", fullName));
			}

			if (uninterceptedMethods.Contains(value))
				throw new MockException("Cannot mock Object..ctor, Object.Equals, Object.ReferenceEquals or Finalize");

			var asMethodInfo = value as MethodInfo;
			if (asMethodInfo != null)
			{
				if (uninterceptedMethods.Contains(asMethodInfo.GetBaseDefinition()))
					throw new MockException("Cannot mock Object.Equals, Object.ReferenceEquals or Finalize");
			}
		}

		public bool IsDerivedFromObjectEquals
		{
			get
			{
				var asMethodInfo = this.Method as MethodInfo;
				return asMethodInfo != null && asMethodInfo.GetBaseDefinition() == ObjectEqualsMethod;
			}
		}

		internal CallPattern Clone()
		{
			var newCallPattern = new CallPattern();
			newCallPattern.method = this.method;
			newCallPattern.InstanceMatcher = this.InstanceMatcher;
			for (int i = 0; i < this.ArgumentMatchers.Count; i++)
			{
				newCallPattern.ArgumentMatchers.Add(this.ArgumentMatchers[i]);
			}

			return newCallPattern;
		}

		internal static CallPattern CreateUniversalCallPattern(MethodBase method)
		{
			var result = new CallPattern();
			result.SetMethod(method, checkCompatibility: true);
			result.InstanceMatcher = new AnyMatcher();
			result.ArgumentMatchers.AddRange(Enumerable.Repeat((IMatcher)new AnyMatcher(), method.GetParameters().Length));
			result.AdjustForExtensionMethod();
			return result;
		}

		internal void AdjustForExtensionMethod()
		{
			if (Method.IsExtensionMethod())
			{
				var thisMatcher = ArgumentMatchers[0];
				var valueMatcher = thisMatcher as IValueMatcher;
				if (valueMatcher != null)
					thisMatcher = new ReferenceMatcher(valueMatcher.Value);

				InstanceMatcher = thisMatcher;
				ArgumentMatchers.RemoveAt(0);
			}
		}
	}
}
