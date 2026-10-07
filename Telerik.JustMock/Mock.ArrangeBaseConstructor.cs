/*
 JustMock Lite
 Copyright © 2010-2015,2018,2025 Progress Software Corporation

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

#if !LITE_EDITION
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Core.MatcherTree;
using Telerik.JustMock.Expectations;

namespace Telerik.JustMock
{
    public partial class Mock
    {
        /// <summary>
        /// Arranges the parameterless base constructor of <typeparamref name="TBase"/>
        /// so it can be suppressed, verified, or redirected when called from a derived type's constructor.
        /// </summary>
        /// <typeparam name="TBase">The base class whose constructor to arrange. Must be a non-sealed reference type.</typeparam>
        /// <returns>Reference to <see cref="ActionExpectation"/> to set up the mock behavior.</returns>
        /// <remarks>
        /// <para>This method requires the JustMock profiler (elevated mocking). Base constructors are
        /// non-virtual and can only be intercepted at the IL level.</para>
        /// <para>Base constructor interception is not supported when OnDemand optimization is enabled.</para>
        /// <para>The arrangement applies globally to all call sites of the specified constructor,
        /// regardless of which derived type triggers it.</para>
        /// <para>Constructor overload resolution prefers exact argument or typed-matcher types,
        /// then more specific compatible parameter types. Unrelated overloads remain ambiguous.</para>
        /// <para>Numeric values and <c>Arg.IsAny&lt;T&gt;()</c> support implicit numeric widening.
        /// Predicate and range matchers must use the selected constructor parameter type.</para>
        /// <para>When <see cref="ActionExpectation"/> is used with <c>DoNothing()</c> to suppress the base constructor body,
        /// any further constructors chained from within that body (e.g. grandparent constructors) are also
        /// not executed, as they are only reachable through the suppressed body.</para>
        /// </remarks>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown when <typeparamref name="TBase"/> is sealed or a value type,
        /// when OnDemand optimization is enabled, or when no matching constructor is found.</exception>
        /// <example>
        /// <code>
        /// Mock.ArrangeBaseConstructor&lt;MyBase&gt;().DoNothing().Occurs(1);
        /// var sut = new MyDerived();
        /// Mock.AssertBaseConstructor&lt;MyBase&gt;();
        /// </code>
        /// </example>
        public static ActionExpectation ArrangeBaseConstructor<TBase>() where TBase : class
        {
            return ProfilerInterceptor.GuardInternal(() => ArrangeBaseConstructor(typeof(TBase), new object[0]));
        }

        /// <summary>
        /// Arranges the base constructor of <typeparamref name="TBase"/> matching the specified argument.
        /// </summary>
        /// <typeparam name="TBase">The base class whose constructor to arrange. Must be a non-sealed reference type.</typeparam>
        /// <param name="arg1">The first argument value or argument matcher used to resolve the constructor overload.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to set up the mock behavior.</returns>
        /// <remarks>Requires the JustMock profiler. The arrangement applies globally.</remarks>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown on invalid type or unresolvable constructor overload.</exception>
        public static ActionExpectation ArrangeBaseConstructor<TBase>(object arg1) where TBase : class
        {
            return ProfilerInterceptor.GuardInternal(() => ArrangeBaseConstructor(typeof(TBase), new object[] { arg1 }));
        }

        /// <summary>
        /// Arranges the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        /// <typeparam name="TBase">The base class whose constructor to arrange. Must be a non-sealed reference type.</typeparam>
        /// <param name="arg1">First argument value or matcher.</param>
        /// <param name="arg2">Second argument value or matcher.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to set up the mock behavior.</returns>
        /// <remarks>Requires the JustMock profiler. The arrangement applies globally.</remarks>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown on invalid type or unresolvable constructor overload.</exception>
        public static ActionExpectation ArrangeBaseConstructor<TBase>(object arg1, object arg2) where TBase : class
        {
            return ProfilerInterceptor.GuardInternal(() => ArrangeBaseConstructor(typeof(TBase), new object[] { arg1, arg2 }));
        }

        /// <summary>
        /// Arranges the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        /// <typeparam name="TBase">The base class whose constructor to arrange. Must be a non-sealed reference type.</typeparam>
        /// <param name="arg1">First argument value or matcher.</param>
        /// <param name="arg2">Second argument value or matcher.</param>
        /// <param name="arg3">Third argument value or matcher.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to set up the mock behavior.</returns>
        /// <remarks>Requires the JustMock profiler. The arrangement applies globally.</remarks>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown on invalid type or unresolvable constructor overload.</exception>
        public static ActionExpectation ArrangeBaseConstructor<TBase>(object arg1, object arg2, object arg3) where TBase : class
        {
            return ProfilerInterceptor.GuardInternal(() => ArrangeBaseConstructor(typeof(TBase), new object[] { arg1, arg2, arg3 }));
        }

        /// <summary>
        /// Arranges the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        /// <typeparam name="TBase">The base class whose constructor to arrange. Must be a non-sealed reference type.</typeparam>
        /// <param name="arg1">First argument value or matcher.</param>
        /// <param name="arg2">Second argument value or matcher.</param>
        /// <param name="arg3">Third argument value or matcher.</param>
        /// <param name="arg4">Fourth argument value or matcher.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to set up the mock behavior.</returns>
        /// <remarks>Requires the JustMock profiler. The arrangement applies globally.</remarks>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown on invalid type or unresolvable constructor overload.</exception>
        public static ActionExpectation ArrangeBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4) where TBase : class
        {
            return ProfilerInterceptor.GuardInternal(() => ArrangeBaseConstructor(typeof(TBase), new object[] { arg1, arg2, arg3, arg4 }));
        }

        /// <summary>
        /// Arranges the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        /// <typeparam name="TBase">The base class whose constructor to arrange. Must be a non-sealed reference type.</typeparam>
        /// <param name="arg1">First argument value or matcher.</param>
        /// <param name="arg2">Second argument value or matcher.</param>
        /// <param name="arg3">Third argument value or matcher.</param>
        /// <param name="arg4">Fourth argument value or matcher.</param>
        /// <param name="arg5">Fifth argument value or matcher.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to set up the mock behavior.</returns>
        /// <remarks>Requires the JustMock profiler. The arrangement applies globally.</remarks>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown on invalid type or unresolvable constructor overload.</exception>
        public static ActionExpectation ArrangeBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4, object arg5) where TBase : class
        {
            return ProfilerInterceptor.GuardInternal(() => ArrangeBaseConstructor(typeof(TBase), new object[] { arg1, arg2, arg3, arg4, arg5 }));
        }

        /// <summary>
        /// Arranges the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        /// <typeparam name="TBase">The base class whose constructor to arrange. Must be a non-sealed reference type.</typeparam>
        /// <param name="arg1">First argument value or matcher.</param>
        /// <param name="arg2">Second argument value or matcher.</param>
        /// <param name="arg3">Third argument value or matcher.</param>
        /// <param name="arg4">Fourth argument value or matcher.</param>
        /// <param name="arg5">Fifth argument value or matcher.</param>
        /// <param name="arg6">Sixth argument value or matcher.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to set up the mock behavior.</returns>
        /// <remarks>Requires the JustMock profiler. The arrangement applies globally.</remarks>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown on invalid type or unresolvable constructor overload.</exception>
        public static ActionExpectation ArrangeBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6) where TBase : class
        {
            return ProfilerInterceptor.GuardInternal(() => ArrangeBaseConstructor(typeof(TBase), new object[] { arg1, arg2, arg3, arg4, arg5, arg6 }));
        }

        /// <summary>
        /// Arranges the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        /// <typeparam name="TBase">The base class whose constructor to arrange. Must be a non-sealed reference type.</typeparam>
        /// <param name="arg1">First argument value or matcher.</param>
        /// <param name="arg2">Second argument value or matcher.</param>
        /// <param name="arg3">Third argument value or matcher.</param>
        /// <param name="arg4">Fourth argument value or matcher.</param>
        /// <param name="arg5">Fifth argument value or matcher.</param>
        /// <param name="arg6">Sixth argument value or matcher.</param>
        /// <param name="arg7">Seventh argument value or matcher.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to set up the mock behavior.</returns>
        /// <remarks>Requires the JustMock profiler. The arrangement applies globally.</remarks>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown on invalid type or unresolvable constructor overload.</exception>
        public static ActionExpectation ArrangeBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7) where TBase : class
        {
            return ProfilerInterceptor.GuardInternal(() => ArrangeBaseConstructor(typeof(TBase), new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 }));
        }

        /// <summary>
        /// Arranges the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        /// <typeparam name="TBase">The base class whose constructor to arrange. Must be a non-sealed reference type.</typeparam>
        /// <param name="arg1">First argument value or matcher.</param>
        /// <param name="arg2">Second argument value or matcher.</param>
        /// <param name="arg3">Third argument value or matcher.</param>
        /// <param name="arg4">Fourth argument value or matcher.</param>
        /// <param name="arg5">Fifth argument value or matcher.</param>
        /// <param name="arg6">Sixth argument value or matcher.</param>
        /// <param name="arg7">Seventh argument value or matcher.</param>
        /// <param name="arg8">Eighth argument value or matcher.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to set up the mock behavior.</returns>
        /// <remarks>Requires the JustMock profiler. The arrangement applies globally.</remarks>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown on invalid type or unresolvable constructor overload.</exception>
        public static ActionExpectation ArrangeBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8) where TBase : class
        {
            return ProfilerInterceptor.GuardInternal(() => ArrangeBaseConstructor(typeof(TBase), new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }));
        }

        // ---- Assert overloads ----

        /// <summary>
        /// Asserts that the parameterless base constructor of <typeparamref name="TBase"/> satisfies
        /// any occurrence expectations set via <see cref="ArrangeBaseConstructor{TBase}()"/>.
        /// </summary>
        /// <typeparam name="TBase">The base class whose constructor to assert. Must be a non-sealed reference type.</typeparam>
        /// <remarks>Requires the JustMock profiler. Must be paired with a prior
        /// <see cref="ArrangeBaseConstructor{TBase}()"/> call that sets occurrence expectations.</remarks>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown on invalid type or unresolvable constructor overload.</exception>
        public static void AssertBaseConstructor<TBase>() where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() => AssertBaseConstructor(typeof(TBase), new object[0]));
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified argument.
        /// </summary>
        /// <typeparam name="TBase">The base class whose constructor to assert. Must be a non-sealed reference type.</typeparam>
        /// <param name="arg1">The first argument value or argument matcher.</param>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown on invalid type or unresolvable constructor overload.</exception>
        public static void AssertBaseConstructor<TBase>(object arg1) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() => AssertBaseConstructor(typeof(TBase), new object[] { arg1 }));
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() => AssertBaseConstructor(typeof(TBase), new object[] { arg1, arg2 }));
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2, object arg3) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() => AssertBaseConstructor(typeof(TBase), new object[] { arg1, arg2, arg3 }));
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() => AssertBaseConstructor(typeof(TBase), new object[] { arg1, arg2, arg3, arg4 }));
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4, object arg5) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() => AssertBaseConstructor(typeof(TBase), new object[] { arg1, arg2, arg3, arg4, arg5 }));
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() => AssertBaseConstructor(typeof(TBase), new object[] { arg1, arg2, arg3, arg4, arg5, arg6 }));
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() => AssertBaseConstructor(typeof(TBase), new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 }));
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() => AssertBaseConstructor(typeof(TBase), new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }));
        }

        // ---- Private helpers ----

        private static ActionExpectation ArrangeBaseConstructor(Type baseType, object[] args)
        {
            var repository = MockingContext.CurrentRepository;
            try
            {
                ValidateBaseConstructorTarget(baseType);
                var ctor = ResolveBaseConstructor(baseType, args);
                NormalizeBaseConstructorArguments(ctor, args);
                return ArrangeBaseConstructorCore(ctor, args);
            }
            finally
            {
                repository.MatchersInContext.Clear();
            }
        }

        private static void AssertBaseConstructor(Type baseType, object[] args)
        {
            var repository = MockingContext.CurrentRepository;
            try
            {
                ValidateBaseConstructorTarget(baseType);
                var ctor = ResolveBaseConstructor(baseType, args);
                NormalizeBaseConstructorArguments(ctor, args);
                repository.AssertMethodInfo(null, null, ctor, args, null);
            }
            finally
            {
                repository.MatchersInContext.Clear();
            }
        }

        private static void ValidateBaseConstructorTarget(Type baseType)
        {
            if (ProfilerInterceptor.IsReJitEnabled)
            {
                throw new MockException("Base constructor interception is not available with OnDemand optimization enabled.");
            }

            if (baseType.IsSealed)
            {
                throw new MockException(
                    $"Cannot arrange base constructor on sealed type '{baseType.Name}'. " +
                    "Sealed types cannot be derived from, so base constructor interception is not applicable.");
            }

            if (baseType.IsValueType)
            {
                throw new MockException(
                    $"Cannot arrange base constructor on value type '{baseType.Name}'. " +
                    "Value types (structs) do not use base constructor chaining in the same way as classes.");
            }

            if (!ProfilerInterceptor.IsProfilerAttached)
            {
                throw new ElevatedMockingException(baseType);
            }

            if (!ProfilerInterceptor.IsBaseConstructorInterceptionAvailable)
            {
                throw new ElevatedMockingException(
                    baseType,
                    "The attached JustMock profiler does not support base constructor interception. " +
                    "Ensure that the profiler and managed JustMock assemblies have matching versions.");
            }
        }

        /// <summary>
        /// Resolves a constructor on <paramref name="type"/> whose parameter count matches
        /// <paramref name="args"/>.Length and whose parameter types are compatible with the supplied values.
        /// Typed matcher information takes precedence over placeholder values. Exact parameter
        /// matches and more specific compatible parameter types take precedence over broader overloads.
        /// </summary>
        private static ConstructorInfo ResolveBaseConstructor(Type type, object[] args)
        {
            var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (args == null || args.Length == 0)
            {
                var parameterlessCtor = ctors.FirstOrDefault(c => c.GetParameters().Length == 0);
                if (parameterlessCtor == null)
                {
                    throw new MockException(
                        $"No parameterless constructor found on type '{type.Name}'. " +
                        BuildAvailableConstructorsList(type, ctors));
                }
                return parameterlessCtor;
            }

            var candidates = ctors.Where(c => c.GetParameters().Length == args.Length).ToArray();
            if (candidates.Length == 0)
            {
                throw new MockException(
                    $"No constructor on type '{type.Name}' with {args.Length} parameter(s) was found. " +
                    BuildAvailableConstructorsList(type, ctors));
            }

            var matcherTypes = GetMatcherTypes(args.Length);
            var matches = candidates.Where(c => ConstructorMatchesArguments(c, args, matcherTypes)).ToArray();
            if (matches.Length == 0)
            {
                throw new MockException(
                    $"No constructor on type '{type.Name}' matches the supplied argument types. " +
                    BuildAvailableConstructorsList(type, ctors));
            }
            if (matches.Length > 1)
            {
                matches = matches.Where(candidate => !matches.Any(other =>
                    other != candidate && IsBetterConstructorMatch(other, candidate, args, matcherTypes))).ToArray();
            }
            if (matches.Length > 1)
            {
                throw new MockException(
                    $"Ambiguous constructor match on type '{type.Name}': multiple constructors match the supplied arguments. " +
                    "Provide arguments with more specific types to disambiguate. " +
                    BuildAvailableConstructorsList(type, ctors));
            }

            return matches[0];
        }

        private static bool IsBetterConstructorMatch(ConstructorInfo candidate, ConstructorInfo other, object[] args, Type[] matcherTypes)
        {
            var parameters = candidate.GetParameters();
            var otherParameters = other.GetParameters();
            bool better = false;
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var otherType = otherParameters[i].ParameterType;
                if (parameterType == otherType)
                    continue;

                var argumentType = matcherTypes[i] ?? (args[i] != null ? args[i].GetType() : null);
                if (argumentType == parameterType)
                {
                    better = true;
                    continue;
                }
                if (argumentType == otherType)
                    return false;

                bool candidateConvertsToOther = otherType.IsAssignableFrom(parameterType)
                    || IsImplicitlyConvertible(parameterType, otherType);
                bool otherConvertsToCandidate = parameterType.IsAssignableFrom(otherType)
                    || IsImplicitlyConvertible(otherType, parameterType);
                if (!candidateConvertsToOther || otherConvertsToCandidate)
                    return false;

                better = true;
            }
            return better;
        }

        private static Type[] GetMatcherTypes(int argumentCount)
        {
            var matchers = MockingContext.CurrentRepository.MatchersInContext;
            var matcherTypes = new Type[argumentCount];
            for (int i = 0; i < argumentCount; i++)
            {
                var matcherIndex = i - (argumentCount - matchers.Count);
                if (matcherIndex >= 0 && matcherIndex < matchers.Count)
                {
                    var typedMatcher = matchers[matcherIndex] as ITypedMatcher;
                    if (typedMatcher != null)
                    {
                        matcherTypes[i] = typedMatcher.Type;
                    }
                }
            }

            return matcherTypes;
        }

        private static bool ConstructorMatchesArguments(ConstructorInfo ctor, object[] args, Type[] matcherTypes)
        {
            var parameters = ctor.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var arg = args[i];
                var argType = matcherTypes[i] ?? (arg != null ? arg.GetType() : null);
                if (argType == null)
                {
                    // null matches any reference type or nullable
                    if (parameters[i].ParameterType.IsValueType &&
                        Nullable.GetUnderlyingType(parameters[i].ParameterType) == null)
                    {
                        return false;
                    }
                    continue;
                }

                var paramType = parameters[i].ParameterType;

                if (!paramType.IsAssignableFrom(argType) && !IsImplicitlyConvertible(argType, paramType))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsImplicitlyConvertible(Type from, Type to)
        {
            if (from.IsEnum || to.IsEnum)
                return false;

            var target = Type.GetTypeCode(to);
            switch (Type.GetTypeCode(from))
            {
                case TypeCode.SByte:
                    return target == TypeCode.Int16 || target == TypeCode.Int32 || target == TypeCode.Int64
                        || target == TypeCode.Single || target == TypeCode.Double || target == TypeCode.Decimal;
                case TypeCode.Byte:
                    return target == TypeCode.Int16 || target == TypeCode.UInt16 || target == TypeCode.Int32
                        || target == TypeCode.UInt32 || target == TypeCode.Int64 || target == TypeCode.UInt64
                        || target == TypeCode.Single || target == TypeCode.Double || target == TypeCode.Decimal;
                case TypeCode.Int16:
                    return target == TypeCode.Int32 || target == TypeCode.Int64
                        || target == TypeCode.Single || target == TypeCode.Double || target == TypeCode.Decimal;
                case TypeCode.UInt16:
                case TypeCode.Char:
                    return (from == typeof(char) && target == TypeCode.UInt16)
                        || target == TypeCode.Int32 || target == TypeCode.UInt32 || target == TypeCode.Int64
                        || target == TypeCode.UInt64 || target == TypeCode.Single || target == TypeCode.Double
                        || target == TypeCode.Decimal;
                case TypeCode.Int32:
                    return target == TypeCode.Int64 || target == TypeCode.Single
                        || target == TypeCode.Double || target == TypeCode.Decimal;
                case TypeCode.UInt32:
                    return target == TypeCode.Int64 || target == TypeCode.UInt64 || target == TypeCode.Single
                        || target == TypeCode.Double || target == TypeCode.Decimal;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return target == TypeCode.Single || target == TypeCode.Double || target == TypeCode.Decimal;
                case TypeCode.Single:
                    return target == TypeCode.Double;
                default:
                    return false;
            }
        }

        private static void NormalizeBaseConstructorArguments(ConstructorInfo ctor, object[] args)
        {
            var parameters = ctor.GetParameters();
            var matchers = MockingContext.CurrentRepository.MatchersInContext;
            for (int i = 0; i < args.Length; i++)
            {
                var matcherIndex = i - (args.Length - matchers.Count);
                var parameterType = parameters[i].ParameterType;
                if (matcherIndex >= 0)
                {
                    var typedMatcher = matchers[matcherIndex] as ITypedMatcher;
                    if (typedMatcher != null && IsImplicitlyConvertible(typedMatcher.Type, parameterType))
                    {
                        if (!(typedMatcher is TypeMatcher))
                        {
                            throw new MockException(
                                $"Numeric base constructor matchers other than Arg.IsAny must use parameter type '{parameterType.Name}'.");
                        }
                        matchers[matcherIndex] = new TypeMatcher(parameterType);
                    }
                }
                else if (args[i] != null && IsImplicitlyConvertible(args[i].GetType(), parameterType))
                {
                    var value = args[i] is char ? (object)(ushort)(char)args[i] : args[i];
                    args[i] = Convert.ChangeType(value, parameterType, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
        }

        private static string BuildAvailableConstructorsList(Type type, ConstructorInfo[] ctors)
        {
            var sb = new StringBuilder();
            sb.Append($"Available constructors on '{type.Name}': ");
            sb.Append(string.Join("; ", ctors.Select(c =>
                $"({string.Join(", ", c.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})")));
            return sb.ToString();
        }

        private static ActionExpectation ArrangeBaseConstructorCore(ConstructorInfo ctor, object[] args)
        {
            var repo = MockingContext.CurrentRepository;
            repo.EnableInterception(ctor.DeclaringType);
            
            var expectation = repo.Arrange(null, ctor, args, () => new ActionExpectation());
            // Mark as a base-ctor arrangement so dispatch only fires from InterceptBaseCtorCall,
            // not from normal body interception when CallOriginal() lets the ctor body run.
            ((IMethodMock)expectation).IsBaseCtorInterception = true;
            return expectation;
        }
    }
}
#endif
