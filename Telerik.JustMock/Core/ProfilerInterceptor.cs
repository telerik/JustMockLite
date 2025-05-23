/*
 JustMock Lite
 Copyright © 2010-2015,2021-2023,2025 Progress Software Corporation

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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Diagnostics;
using Telerik.JustMock.Setup;
#if DEBUG
using Telerik.JustMock.Helpers;
#endif
#if NETCORE
using System.Runtime.InteropServices;
#endif

namespace Telerik.JustMock.Core
{
    internal static class ProfilerInterceptor
    {
        private static bool DispatchInvocation(Invocation invocation)
        {
            DebugView.TraceEvent(IndentLevel.Dispatch, () => String.Format("Intercepted profiler call: {0}", invocation.InputToString()));
            DebugView.PrintStackTrace();

            var mockMixin = invocation.MockMixin;
            var repo = mockMixin != null ? mockMixin.Repository : MockingContext.ResolveRepository(UnresolvedContextBehavior.CreateNewContextual);

            if (repo == null)
                repo = TryFindGlobalInterceptor(invocation.Method);
            if (repo == null)
                return false;

            lock (repo)
            {
                repo.DispatchInvocation(invocation);
            }

            if (invocation.CallOriginal)
            {
                DebugView.TraceEvent(IndentLevel.DispatchResult, () => "Calling original implementation");
            }
            else if (invocation.IsReturnValueSet)
            {
                DebugView.TraceEvent(IndentLevel.DispatchResult, () => String.Format("Returning value '{0}'", invocation.ReturnValue));
            }

            return true;
        }

        private static bool InterceptCall(RuntimeTypeHandle typeHandle, RuntimeMethodHandle methodHandle, object[] data)
        {
            if (!IsInterceptionEnabled || isFinalizerThread)
                return true;

            try
            {
                ReentrancyCounter++;

                var method = MethodBase.GetMethodFromHandle(methodHandle, typeHandle);
                if (method.DeclaringType == null)
                    return true;

                if (method == skipMethodInterceptionOnce)
                {
                    skipMethodInterceptionOnce = null;
                    return true;
                }

#if DEBUG
                ProfilerLogger.Info("*** +++ [MANAGED] Intercepting method call for {0}.{1}", method.DeclaringType.Name, method.Name);
#endif

                var invocation = new Invocation(data[0], method, data.Skip(2).ToArray());

                if (DispatchInvocation(invocation))
                {
                    data[1] = invocation.ReturnValue;
                    int methodArgsCount = method.GetParameters().Length;
                    for (int i = 0; i < methodArgsCount; ++i)
                        data[i + 2] = invocation.Args[i];

                    return invocation.CallOriginal;
                }
                return true;
            }
            finally
            {
                ReentrancyCounter--;
            }
        }

        private static object InterceptNewobj(RuntimeTypeHandle typeHandle, RuntimeMethodHandle methodHandle, object[] data)
        {
            if (!IsInterceptionEnabled || isFinalizerThread)
                return null;

            try
            {
                ReentrancyCounter++;

                var method = MethodBase.GetMethodFromHandle(methodHandle, typeHandle);

#if DEBUG
                ProfilerLogger.Info("*** +++ [MANAGED] Intercepting constructor call for {0}.{1}", method.DeclaringType.Name, method.Name);
#endif

                var invocation = new Invocation(MockingUtil.TryGetUninitializedObject(method.DeclaringType), method, data ?? new object[0]);

                if (DispatchInvocation(invocation))
                {
                    if (invocation.CallOriginal)
                    {
                        SkipMethodInterceptionOnce(method);
                        return null;
                    }
                    if (invocation.IsReturnValueSet && invocation.ReturnValue != null)
                    {
                        return invocation.ReturnValue;
                    }
                    return invocation.Instance;
                }
                return null;
            }
            finally
            {
                ReentrancyCounter--;
            }
        }

        static ProfilerInterceptor()
        {
#if !LITE_EDITION
            var mscorlib = typeof(object).Assembly;
            bridge = mscorlib.GetType("Telerik.JustMock.Profiler");
#endif
            if (bridge == null)
            {
                getReentrancyCounter = GetSurrogateReentrancyCounter;
                setReentrancyCounter = SetSurrogateReentrancyCounter;
            }
            else
            {
                var newObjInterceptionOnOverwriteEnabledEnv = Environment.GetEnvironmentVariable("JUSTMOCK_NEWOBJ_INTERCEPTION_ON_OVERWRITE_ENABLED");
                NewObjInterceptionOnOverwriteEnabled = string.IsNullOrEmpty(newObjInterceptionOnOverwriteEnabledEnv)
                                                         ? false
                                                         : newObjInterceptionOnOverwriteEnabledEnv == "1";

                bridge = bridge.MakeGenericType(typeof(object));

#if !DEBUG
                CheckProfilerCompatibility();
#endif

                CreateDelegateFromBridge("GetUninitializedObject", out GetUninitializedObjectImpl);
                CreateDelegateFromBridge("CreateStrongNameAssemblyName", out CreateStrongNameAssemblyNameImpl);
                CreateDelegateFromBridge("RunClassConstructor", out runClassConstructor);
                CreateDelegateFromBridge("CreateInstanceWithArgs", out CreateInstanceWithArgsImpl);

                InitializeFieldAccessors("IsInterceptionSetup", ref getIsInterceptionSetup, ref setIsInterceptionSetup);
                InitializeFieldAccessors("ReentrancyCounter", ref getReentrancyCounter, ref setReentrancyCounter);

                WrapCallToDelegate("GetTypeId", out GetTypeIdImpl);
                WrapCallToDelegate("RequestReJit", out RequestReJitImpl);

                bridge.GetMethod("Init").Invoke(null, null);
            }
        }

        private static void CheckProfilerCompatibility()
        {
            Assembly justMockAssembly = Assembly.GetExecutingAssembly();
            Version justMockAssemblyVersion = new AssemblyName(justMockAssembly.FullName).Version;

            string profilerVersion = "0000.0.0.0";
            MethodInfo getProfilerVersion = bridge.GetMethod("GetProfilerVersion");
            if (getProfilerVersion != null)
            {
                profilerVersion = (string)getProfilerVersion.Invoke(null, null);
            }
            JMDebug.Assert(null != profilerVersion);
            var codeWeaverAssemblyVersion = new Version(profilerVersion);

            if (justMockAssemblyVersion.CompareTo(codeWeaverAssemblyVersion) != 0)
            {
                string errorMessage = string.Format("JustMock is configured to use an incompatible profiler:" + Environment.NewLine
                    + "\tTelerik.JustMock.dll version: {0}" + Environment.NewLine
                    + "\tTelerik.CodeWeaver.Profiler{1} version: {2}" + Environment.NewLine
                    + "If you are using JustMock inside Visual Studio, please verify that the version of the locally installed product and the referenced one via NuGet package or assembly are identical." + Environment.NewLine
                    + "On the command line, ensure that the environment is configured to use the JustMock profiler with a version matching the referenced JustMock assembly." + Environment.NewLine,
                    justMockAssemblyVersion.ToString(),
                    GetProfilerExtension(),
                    codeWeaverAssemblyVersion.ToString());

                throw new MockException(errorMessage);
            }
        }

        private static string GetProfilerExtension()
        {
#if !NETCORE
            return ".dll";
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return ".dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return ".so";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return ".dylib";
            }
            else
            {
                return string.Empty;
            }
#endif
        }

        public static void Initialize()
        {
            if (!IsProfilerAttached)
                return;

            lock (mutex)
            {
                if (!IsInterceptionSetup)
                {
                    FinalizerThreadIdentifier.Identify();

                    var processInvocationType = typeof(object).Assembly.GetType("Telerik.JustMock.ProcessInvocationDelegate");
                    Func<RuntimeTypeHandle, RuntimeMethodHandle, object[], bool> interceptCallAsAction = InterceptCall;
                    var interceptCallDelegate = Delegate.CreateDelegate(processInvocationType, interceptCallAsAction.Method);
                    bridge.GetField("ProcessInvocation").SetValue(null, interceptCallDelegate);

                    var processNewobjType = typeof(object).Assembly.GetType("Telerik.JustMock.ProcessNewobjDelegate");
                    Func<RuntimeTypeHandle, RuntimeMethodHandle, object[], object> interceptNewobjAsAction = InterceptNewobj;
                    var interceptNewobjDelegate = Delegate.CreateDelegate(processNewobjType, interceptNewobjAsAction.Method);
                    bridge.GetField("ProcessNewobj").SetValue(null, interceptNewobjDelegate);

                    var arrangedTypesField = bridge.GetField("ArrangedTypesArray");
                    arrangedTypesField.SetValue(null, arrangedTypesArray);

                    IsInterceptionSetup = true;
                }
            }
        }

        public static int ReentrancyCounter
        {
            [DebuggerHidden]
            get { return getReentrancyCounter(); }
            [DebuggerHidden]
            set
            {
                if (value < 0)
                    throw new InvalidOperationException();
                setReentrancyCounter(value);
            }
        }

        private static bool IsInterceptionSetup
        {
            get { return ProfilerInterceptor.IsProfilerAttached ? getIsInterceptionSetup() : false; }
            set { if (ProfilerInterceptor.IsProfilerAttached) setIsInterceptionSetup(value); }
        }

#if DEBUG
        private static readonly Dictionary<MocksRepository, HashSet<Type>> typesEnabledByRepo = new Dictionary<MocksRepository, HashSet<Type>>();
#endif
        public static void EnableInterception(Type type, bool enabled, MocksRepository behalf)
        {
            if (IsProfilerAttached)
            {
#if DEBUG
                lock (typesEnabledByRepo)
                {
                    HashSet<Type> types;
                    if (!typesEnabledByRepo.TryGetValue(behalf, out types))
                    {
                        types = new HashSet<Type>();
                        typesEnabledByRepo.Add(behalf, types);
                    }

                    if (types.Contains(type) == enabled)
                    {
                        throw new InvalidOperationException("Type interception enabled or disabled twice.");
                    }

                    if (enabled)
                        types.Add(type);
                    else types.Remove(type);

                    if (types.Count == 0)
                        typesEnabledByRepo.Remove(behalf);
                }
#endif

                bool enabledInAnyRepository;

                DebugView.TraceEvent(IndentLevel.Configuration, () => String.Format("Interception of type {0} is now {1}", type, enabled ? "on" : "off"));

                lock (enabledInterceptions)
                {
                    bool hasKey = enabledInterceptions.ContainsKey(type);
                    if (!hasKey)
                    {
                        enabledInterceptions.Add(type, enabled ? 1 : 0);
                        enabledInAnyRepository = enabled;
                    }
                    else
                    {
                        int count = enabledInterceptions[type];
                        if (!enabled && count > 0)
                            count--;
                        else if (enabled)
                            count++;
                        enabledInAnyRepository = count > 0;
                        enabledInterceptions[type] = count;
                    }
                }

                int typeId = GetTypeId(type);
                int arrayIndex = typeId >> 3;
                int arrayMask = 1 << (typeId & ((1 << 3) - 1));
                lock (arrangedTypesArray)
                {
                    if (enabledInAnyRepository)
                        arrangedTypesArray[arrayIndex] = (byte)(arrangedTypesArray[arrayIndex] | arrayMask);
                    else
                        arrangedTypesArray[arrayIndex] = (byte)(arrangedTypesArray[arrayIndex] & ~arrayMask);
                }
            }
        }

        // for calling in the debugger
        public static bool IsTypeIntercepted(Type type)
        {
            var typeId = GetTypeId(type);
            var arrayIndex = typeId >> 3;
            var arrayMask = 1 << (typeId & ((1 << 3) - 1));
            return (arrangedTypesArray[arrayIndex] & arrayMask) != 0;
        }

        public static void RequestReJit(MethodBase method)
        {
            if (!IsReJitEnabled)
            {
                ThrowElevatedMockingException();
            }

            var typeName =
                method.DeclaringType.IsGenericType
                    ?
                        string.Format("{0}[{1}]", method.DeclaringType.Name, string.Join(", ", method.DeclaringType.GetGenericArguments().Select(current => current.ToString()).ToArray()))
                        :
                        method.DeclaringType.Name;

            var methodName =
                method.IsGenericMethod
                    ?
                        string.Format("{0}[{1}]", method.Name, string.Join(", ", method.GetGenericArguments().Select(current => current.ToString()).ToArray()))
                        :
                        method.Name;

#if DEBUG
            ProfilerLogger.Info("*** [MANAGED] Requesting ReJit for {0}.{1}", typeName, methodName);
#endif

            var typeHandle = method.DeclaringType.TypeHandle;
            var methodToken = method.MetadataToken;
            IntPtr[] methodGenericArgHandles = method.IsGenericMethod ? method.GetGenericArguments().Select(type => type.TypeHandle.Value).ToArray() : null;
            var methodGenericArgHandlesCount = methodGenericArgHandles != null ? methodGenericArgHandles.Length : 0;
            bool requestSucceeded = RequestReJitImpl(typeHandle.Value, methodToken, methodGenericArgHandlesCount, methodGenericArgHandles);
            if (!requestSucceeded)
            {
                throw new MockException(string.Format("ReJit request failed for {0}.{1}", typeName, methodName));
            }
        }

        internal static void RegisterGlobalInterceptor(MethodBase method, MocksRepository repo)
        {
            lock (globalInterceptors)
            {
                List<MocksRepository> repos;
                if (!globalInterceptors.TryGetValue(method, out repos))
                {
                    globalInterceptors[method] = repos = new List<MocksRepository>();
                }
                repos.Add(repo);
            }
        }

        internal static void UnregisterGlobalInterceptor(MethodBase method, MocksRepository repo)
        {
            lock (globalInterceptors)
            {
                var repos = globalInterceptors[method];
                repos.Remove(repo);
                if (repos.Count == 0)
                    globalInterceptors.Remove(method);
            }
        }

        private static MocksRepository TryFindGlobalInterceptor(MethodBase method)
        {
            lock (globalInterceptors)
            {
                List<MocksRepository> repos;
                if (globalInterceptors.TryGetValue(method, out repos))
                    return repos.LastOrDefault();
            }
            return null;
        }

        private static int GetTypeId(Type type)
        {
#if SILVERLIGHT
            return GetTypeIdImpl(type.Module.ToString(), type.MetadataToken);
#else
            return GetTypeIdImpl(type.Module.ModuleVersionId.ToString("B").ToUpperInvariant(), type.MetadataToken);
#endif
        }

        [DebuggerHidden]
        private static int GetSurrogateReentrancyCounter()
        {
            return surrogateReentrancyCounter;
        }

        [DebuggerHidden]
        private static void SetSurrogateReentrancyCounter(int value)
        {
            surrogateReentrancyCounter = value;
        }

        [DebuggerHidden]
        public static void GuardInternal(Action guardedAction)
        {
            try
            {
                ReentrancyCounter++;
                guardedAction();
            }
            finally
            {
                ReentrancyCounter--;
            }
        }

        [DebuggerHidden]
        public static T GuardInternal<T>(Func<T> guardedAction)
        {
            try
            {
                ReentrancyCounter++;
                return guardedAction();
            }
            finally
            {
                ReentrancyCounter--;
            }
        }

        [DebuggerHidden]
        public static ref T GuardInternal<T>(RefReturn<T> @delegate, object target, object[] args)
        {
            try
            {
                ReentrancyCounter++;
                return ref @delegate(target, args);
            }
            finally
            {
                ReentrancyCounter--;
            }
        }

        [DebuggerHidden]
        public static void GuardExternal(Action guardedAction)
        {
            var oldCounter = ProfilerInterceptor.ReentrancyCounter;
            try
            {
                ProfilerInterceptor.ReentrancyCounter = 0;
                guardedAction();
            }
            finally
            {
                ProfilerInterceptor.ReentrancyCounter = oldCounter;
            }
        }

        [DebuggerHidden]
        public static T GuardExternal<T>(Func<T> guardedAction)
        {
            var oldCounter = ProfilerInterceptor.ReentrancyCounter;
            try
            {
                ProfilerInterceptor.ReentrancyCounter = 0;
                return guardedAction();
            }
            finally
            {
                ProfilerInterceptor.ReentrancyCounter = oldCounter;
            }
        }

        public delegate ref T RefReturn<T>(object target, object[] args);

        [DebuggerHidden]
        public static ref T GuardExternal<T>(RefReturn<T> @delegate, object target, object[] args)
        {
            var oldCounter = ProfilerInterceptor.ReentrancyCounter;
            try
            {
                ProfilerInterceptor.ReentrancyCounter = 0;
                return ref @delegate(target, args);
            }
            finally
            {
                ProfilerInterceptor.ReentrancyCounter = oldCounter;
            }
        }

        public static void CreateDelegateFromBridge<T>(string bridgeMethodName, out T delg)
        {
            if (bridge == null)
                ProfilerInterceptor.ThrowElevatedMockingException();
            var method = bridge.GetMethod(bridgeMethodName);
            delg = (T)(object)Delegate.CreateDelegate(typeof(T), method);
        }

        public static void WrapCallToDelegate<T>(string wrappedDelegateFieldName, out T delg)
        {
            if (bridge == null)
                ProfilerInterceptor.ThrowElevatedMockingException();

            var wrappedDelegateField = bridge.GetField(wrappedDelegateFieldName);
            var invokeMethod = wrappedDelegateField.FieldType.GetMethod("Invoke");
            var parameters = invokeMethod.GetParameters().Select(p => Expression.Parameter(p.ParameterType, "")).ToArray();
            var caller = Expression.Call(Expression.Field(null, wrappedDelegateField), invokeMethod, parameters);
            delg = (T)(object)Expression.Lambda(typeof(T), caller, parameters).Compile();
        }

        public static void RunClassConstructor(RuntimeTypeHandle typeHandle)
        {
            if (runClassConstructor != null && !SecuredReflection.HasReflectionPermission)
                GuardExternal(() => runClassConstructor(typeHandle));
            else
                GuardExternal(() => RuntimeHelpers.RunClassConstructor(typeHandle));
        }

        public static void CheckIfSafeToInterceptWholesale(Type type)
        {
            if (!IsProfilerAttached || !TypeSupportsInstrumentation(type))
                return;

            if (AllowedMockableTypes.List.Contains(type))
                return;

            if (typeof(CriticalFinalizerObject).IsAssignableFrom(type))
            {
                MockException.ThrowUnsafeTypeException(type);
            }

            var hasUnmockableInstanceMembers =
                type.Assembly == typeof(object).Assembly
                && type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Any(method =>
                    {
                        if (method.DeclaringType == typeof(object)
                            || method.DeclaringType == typeof(ValueType)
                            || method.DeclaringType == typeof(Enum))
                            return false;

                        var methodImpl = method.GetMethodImplementationFlags();
                        return (methodImpl & MethodImplAttributes.InternalCall) != 0
                            || (methodImpl & MethodImplAttributes.CodeTypeMask) != MethodImplAttributes.IL;
                    });

            if (!typeof(Exception).IsAssignableFrom(type) && hasUnmockableInstanceMembers)
            {
                MockException.ThrowUnsafeTypeException(type);
            }
        }

        public static bool TypeSupportsInstrumentation(Type type)
        {
            if (typeof(Delegate).IsAssignableFrom(type))
                return true;

            if (type == typeof(WeakReference)
                || type == typeof(MemberInfo)
                || type == typeof(Type)
                || type == typeof(MethodBase)
                || type == typeof(MethodInfo)
                || type == typeof(ConstructorInfo)
                || type == typeof(FieldInfo)
                || type == typeof(PropertyInfo)
                || type == typeof(EventInfo)
                || type == typeof(System.CannotUnloadAppDomainException)
                || Nullable.GetUnderlyingType(type) != null)
                return false;

            return true;
        }

        private static void InitializeFieldAccessors<TFieldType>(string fieldName, ref Func<TFieldType> getter, ref Action<TFieldType> setter)
        {
            var field = bridge.GetField(fieldName);

            getter = Expression.Lambda(Expression.Field(null, field)).Compile() as Func<TFieldType>;

            var valueParam = Expression.Parameter(typeof(TFieldType), "value");

            setter = Expression.Lambda(CreateFieldAssignmentExpression(field, valueParam), valueParam).Compile() as Action<TFieldType>;
        }

        private static Expression CreateFieldAssignmentExpression(FieldInfo assignee, ParameterExpression valueParam)
        {
            var fieldType = assignee.FieldType;

            var action = MockingUtil.CreateDynamicMethodWithVisibilityChecks(typeof(void), new[] { fieldType }, il =>
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Stsfld, assignee);
                    il.Emit(OpCodes.Ret);
                });

            return Expression.Call(null, action, valueParam);
        }

        private class FinalizerThreadIdentifier
        {
            public static void Identify()
            {
                new FinalizerThreadIdentifier();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            ~FinalizerThreadIdentifier()
            {
                isFinalizerThread = true;
            }
        }

        public static void ThrowElevatedMockingException(MemberInfo member = null)
        {
            var ex =
                member != null
                    ?
                        new ElevatedMockingException(member)
                        :
                        new ElevatedMockingException();
            throw ex;
        }

        public static bool IsProfilerAttached { [DebuggerHidden] get { return bridge != null; } }
        public static bool IsInterceptionEnabled { get; set; }
        public static readonly Func<Type, object> GetUninitializedObjectImpl;
        public static readonly Func<string, byte[], object> CreateStrongNameAssemblyNameImpl;
        public static readonly Func<Type, object[], object> CreateInstanceWithArgsImpl;

        public static bool NewObjInterceptionOnOverwriteEnabled { get; private set; }

        private static readonly Type bridge;
        private static readonly Func<string/*module mvid or name (SL)*/, int /*typedef token*/, int /*id*/> GetTypeIdImpl;
        private static readonly Dictionary<Type, int> enabledInterceptions = new Dictionary<Type, int>();
        private static readonly byte[] arrangedTypesArray = new byte[100000];
        private static readonly object mutex = new object();
        private static readonly Func<int> getReentrancyCounter;
        private static readonly Action<int> setReentrancyCounter;
        private static readonly Func<bool> getIsInterceptionSetup;
        private static readonly Action<bool> setIsInterceptionSetup;
        private static readonly Action<RuntimeTypeHandle> runClassConstructor;

        private static readonly Dictionary<MethodBase, List<MocksRepository>> globalInterceptors = new Dictionary<MethodBase, List<MocksRepository>>();

        public static bool IsReJitEnabled { [DebuggerHidden] get { return IsProfilerAttached && (bool)bridge.GetMethod("IsReJitEnabled").Invoke(null, null); } }
        private static readonly Func<IntPtr/*type handle*/, int /* method token*/, int /* method generic args count */, IntPtr[] /* method generic args */, bool /*result*/> RequestReJitImpl;

        [ThreadStatic]
        private static int surrogateReentrancyCounter;

        [ThreadStatic]
        private static bool isFinalizerThread;

        [ThreadStatic]
        private static MethodBase skipMethodInterceptionOnce;

        internal static void SkipMethodInterceptionOnce(MethodBase method)
        {
            JMDebug.Assert(skipMethodInterceptionOnce == null || skipMethodInterceptionOnce == method);
            skipMethodInterceptionOnce = method;
        }
    }
}
