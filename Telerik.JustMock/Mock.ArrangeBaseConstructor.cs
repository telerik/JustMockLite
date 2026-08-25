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

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Context;
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
        /// <para>The arrangement applies globally to all call sites of the specified constructor,
        /// regardless of which derived type triggers it.</para>
        /// <para>When <see cref="ActionExpectation.DoNothing"/> is used to suppress the base constructor body,
        /// any further constructors chained from within that body (e.g. grandparent constructors) are also
        /// not executed, as they are only reachable through the suppressed body.</para>
        /// </remarks>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown when <typeparamref name="TBase"/> is sealed or a value type,
        /// or when no matching constructor is found.</exception>
        /// <example>
        /// <code>
        /// Mock.ArrangeBaseConstructor&lt;MyBase&gt;().DoNothing().Occurs(1);
        /// var sut = new MyDerived();
        /// Mock.AssertBaseConstructor&lt;MyBase&gt;();
        /// </code>
        /// </example>
        public static ActionExpectation ArrangeBaseConstructor<TBase>() where TBase : class
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var ctor = ResolveBaseConstructor(typeof(TBase), Type.EmptyTypes);
                return ArrangeBaseConstructorCore(ctor, new object[0]);
            });
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
            return ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                return ArrangeBaseConstructorCore(ctor, args);
            });
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
            return ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                return ArrangeBaseConstructorCore(ctor, args);
            });
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
            return ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2, arg3 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                return ArrangeBaseConstructorCore(ctor, args);
            });
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
            return ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2, arg3, arg4 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                return ArrangeBaseConstructorCore(ctor, args);
            });
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
            return ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2, arg3, arg4, arg5 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                return ArrangeBaseConstructorCore(ctor, args);
            });
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
            return ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2, arg3, arg4, arg5, arg6 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                return ArrangeBaseConstructorCore(ctor, args);
            });
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
            return ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                return ArrangeBaseConstructorCore(ctor, args);
            });
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
            return ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                return ArrangeBaseConstructorCore(ctor, args);
            });
        }

        // ---- Assert overloads ----

        /// <summary>
        /// Asserts that the parameterless base constructor of <typeparamref name="TBase"/> satisfies
        /// any occurrence expectations set via <see cref="ArrangeBaseConstructor{TBase}()"/>.
        /// </summary>
        /// <typeparam name="TBase">The base class whose constructor to assert. Must be a non-sealed reference type.</typeparam>
        /// <param name="message">Optional failure message.</param>
        /// <remarks>Requires the JustMock profiler. Must be paired with a prior
        /// <see cref="ArrangeBaseConstructor{TBase}()"/> call that sets occurrence expectations.</remarks>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown on invalid type or unresolvable constructor overload.</exception>
        public static void AssertBaseConstructor<TBase>(string message = null) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var ctor = ResolveBaseConstructor(typeof(TBase), Type.EmptyTypes);
                MockingContext.CurrentRepository.AssertMethodInfo(message, null, ctor, new object[0], null);
            });
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified argument.
        /// </summary>
        /// <typeparam name="TBase">The base class whose constructor to assert. Must be a non-sealed reference type.</typeparam>
        /// <param name="arg1">The first argument value or argument matcher.</param>
        /// <param name="message">Optional failure message.</param>
        /// <exception cref="ElevatedMockingException">Thrown when the profiler is not attached.</exception>
        /// <exception cref="MockException">Thrown on invalid type or unresolvable constructor overload.</exception>
        public static void AssertBaseConstructor<TBase>(object arg1, string message = null) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, null, ctor, args, null);
            });
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2, string message = null) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, null, ctor, args, null);
            });
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2, object arg3, string message = null) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2, arg3 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, null, ctor, args, null);
            });
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4, string message = null) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2, arg3, arg4 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, null, ctor, args, null);
            });
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4, object arg5, string message = null) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2, arg3, arg4, arg5 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, null, ctor, args, null);
            });
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, string message = null) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2, arg3, arg4, arg5, arg6 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, null, ctor, args, null);
            });
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, string message = null) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, null, ctor, args, null);
            });
        }

        /// <summary>
        /// Asserts the base constructor of <typeparamref name="TBase"/> matching the specified arguments.
        /// </summary>
        public static void AssertBaseConstructor<TBase>(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, string message = null) where TBase : class
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                ValidateBaseConstructorTarget(typeof(TBase));
                var args = new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 };
                var ctor = ResolveBaseConstructor(typeof(TBase), args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, null, ctor, args, null);
            });
        }

        // ---- Private helpers ----

        private static void ValidateBaseConstructorTarget(Type baseType)
        {
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
        }

        /// <summary>
        /// Resolves a constructor on <paramref name="type"/> whose parameter count matches
        /// <paramref name="args"/>.Length and whose parameter types are compatible with the supplied values.
        /// Argument matchers (e.g. <c>Arg.IsAny&lt;int&gt;()</c>) are supported — the matcher value's
        /// runtime type is used for compatibility checking.
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

            var matches = candidates.Where(c => ConstructorMatchesArguments(c, args)).ToArray();
            if (matches.Length == 0)
            {
                throw new MockException(
                    $"No constructor on type '{type.Name}' matches the supplied argument types. " +
                    BuildAvailableConstructorsList(type, ctors));
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

        private static ConstructorInfo ResolveBaseConstructor(Type type, Type[] argTypes)
        {
            var ctor = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, argTypes, null);

            if (ctor == null)
            {
                throw new MockException(
                    $"No constructor on type '{type.Name}' with parameter types [{string.Join(", ", argTypes.Select(t => t.Name))}] was found.");
            }

            return ctor;
        }

        private static bool ConstructorMatchesArguments(ConstructorInfo ctor, object[] args)
        {
            var parameters = ctor.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var arg = args[i];
                if (arg == null)
                {
                    // null matches any reference type or nullable
                    if (parameters[i].ParameterType.IsValueType &&
                        Nullable.GetUnderlyingType(parameters[i].ParameterType) == null)
                    {
                        return false;
                    }
                    continue;
                }

                var argType = arg.GetType();
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
            // Handle common numeric promotions used with default values from Arg matchers
            if (to == typeof(long) && (from == typeof(int) || from == typeof(short) || from == typeof(byte)))
                return true;
            if (to == typeof(double) && (from == typeof(float) || from == typeof(int) || from == typeof(long)))
                return true;
            if (to == typeof(float) && (from == typeof(int)))
                return true;
            if (to == typeof(decimal) && (from == typeof(int) || from == typeof(long)))
                return true;
            // Unsigned numeric promotions
            if (to == typeof(ulong) && (from == typeof(uint) || from == typeof(ushort) || from == typeof(byte)))
                return true;
            if (to == typeof(uint) && (from == typeof(ushort) || from == typeof(byte)))
                return true;
            if (to == typeof(ushort) && from == typeof(byte))
                return true;
            return false;
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
            // RequestRejitForTypeMethods only covers regular methods, not constructors.
            // Explicitly ReJIT the specific constructor so EmitCallInterceptionCode
            // instruments it, allowing InterceptCall to intercept the base call.
            if (ProfilerInterceptor.IsReJitEnabled)
            {
                ProfilerInterceptor.RequestReJit(ctor);
            }
            var expectation = repo.Arrange(null, ctor, args, () => new ActionExpectation());
            // Mark as a base-ctor arrangement so dispatch only fires from InterceptBaseCtorCall,
            // not from normal body interception when CallOriginal() lets the ctor body run.
            ((IMethodMock)expectation).IsBaseCtorInterception = true;
            return expectation;
        }
    }
}
