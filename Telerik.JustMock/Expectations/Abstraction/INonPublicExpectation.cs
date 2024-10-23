/*
 JustMock Lite
 Copyright © 2010-2015, 2018-2020 Progress Software Corporation

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
using System.Reflection;

namespace Telerik.JustMock.Expectations.Abstraction
{
    /// <summary>
    /// Defines methods to mock non-public members.
    /// </summary>
    /// <remarks>
    /// Non-public methods are identified by their name and, optionally, by their arguments
    /// when there's a need to disambiguate between overloads, or when you need to specify
    /// matchers for the arguments using. Arguments are passed using either constant objects
    /// when a specific value needs to be matched, or using one of the members of the
    /// <see cref="Arg.Expr"/> class. If a member is not overloaded and you want the arrangement
    /// to work for all arguments, then you can specify just the name of the member and omit
    /// all arguments. This is equivalent to passing the correct Arg.Expr.IsAny&lt;T&gt;() arguments
    /// or adding the .IgnoreArguments() clause.
    /// </remarks>
    public interface INonPublicExpectation
    {
        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="memberName">Member name</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(object target, string memberName, params object[] args);

        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="memberName">Member name</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(object target, string memberName, Type[] typeArguments, params object[] args);

        /// <summary>
        /// Setups a non-public method for mocking.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="method">Method to setup taken from reflection.</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(object target, MethodInfo method, params object[] args);


        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <typeparam name="TReturn">Return type</typeparam>
        /// <param name="target">Target instance.</param>
        /// <param name="memberName">Target member name</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        FuncExpectation<TReturn> Arrange<TReturn>(object target, string memberName, params object[] args);


        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <typeparam name="TReturn">Return type</typeparam>
        /// <param name="target">Target instance.</param>
        /// <param name="memberName">Target member name</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        FuncExpectation<TReturn> Arrange<TReturn>(object target, string memberName, Type[] typeArguments, params object[] args);

        /// <summary>
        /// Setups a non-public method for mocking.
        /// </summary>
        /// <typeparam name="TReturn">Return type</typeparam>
        /// <param name="target">Target instance</param>
        /// <param name="method">Method to setup taken from reflection.</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(object target, MethodInfo method, params object[] args);

        /// <summary>
        /// Arranges a non-public property setter for mocking.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Property value. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation ArrangeSet(object target, string propertyName, object value);

        /// <summary>
        /// Arranges a non-public static property setter contained by type parameter "T" for mocking.
        /// </summary>
        /// <typeparam name="T">Type that contains the non-public static property</typeparam>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Property value. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation ArrangeSet<T>(string propertyName, object value);

        /// <summary>
        /// Arranges a non-public static property setter contained by "type" for mocking.
        /// </summary>
        /// <param name="type">Type that contains the non-public static property</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Property value. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation ArrangeSet(Type type, string propertyName, object value);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="memberName">Name of the member</param>
        /// <typeparam name="TReturn">Return type of the method</typeparam>
        /// <param name="args">Method arguments</param>
        void Assert<TReturn>(object target, string memberName, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="memberName">Name of the member</param>
        /// <typeparam name="TReturn">Return type of the method</typeparam>
        /// <param name="args">Method arguments</param>
        void Assert<TReturn>(object target, string memberName, Type[] typeArguments, params object[] args);

        /// <summary>
        /// Asserts the specified method that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="method">Method to assert taken from reflection.</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, MethodInfo method, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string memberName, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string memberName, Type[] typeArguments, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <typeparam name="TReturn">Return type of the method</typeparam>
        /// <param name="args">Method arguments</param>
        void Assert<TReturn>(object target, string memberName, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <typeparam name="TReturn">Return type of the method</typeparam>
        /// <param name="args">Method arguments</param>
        void Assert<TReturn>(object target, string memberName, Type[] typeArguments, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified method that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="method">Method to assert taken from reflection.</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, MethodInfo method, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string memberName, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string memberName, Type[] typeArguments, Occurs occurs, params object[] args);

        /// <summary>
        /// Returns the number of times the specified member was called.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="method">Method taken from reflection</param>
        /// <param name="args">Method arguments</param>
        /// <returns>Number of calls.</returns>
        int GetTimesCalled(object target, MethodInfo method, params object[] args);

        /// <summary>
        /// Returns the number of times the specified member was called.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="args">Method arguments</param>
        /// <returns>Number of calls.</returns>
        int GetTimesCalled(object target, string memberName, params object[] args);


#if !LITE_EDITION
        /// <summary>
        /// Returns the number of times the specified member was called.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="args">Method arguments</param>
        /// <returns>Number of calls.</returns>
        int GetTimesCalled(object target, string memberName, Type[] typeArguments, params object[] args);

        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <typeparam name="T">Type of the target.</typeparam>
        /// <typeparam name="TReturn">Return type</typeparam>
        /// <param name="memberName">Target member name</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        FuncExpectation<TReturn> Arrange<T, TReturn>(string memberName, params object[] args);

        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <typeparam name="T">Type of the target.</typeparam>
        /// <typeparam name="TReturn">Return type</typeparam>
        /// <param name="memberName">Target member name</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        FuncExpectation<TReturn> Arrange<T, TReturn>(string memberName, Type[] typeArguments, params object[] args);

        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <typeparam name="T">Type of the target.</typeparam>
        /// <param name="memberName">Target member name</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        ActionExpectation Arrange<T>(string memberName, params object[] args);

        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <typeparam name="T">Type of the target.</typeparam>
        /// <param name="memberName">Target member name</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        ActionExpectation Arrange<T>(string memberName, Type[] typeArguments, params object[] args);

        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <typeparam name="TReturn">Return type</typeparam>
        /// <param name="targetType">Target type</param>
        /// <param name="memberName">Target member name</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        FuncExpectation<TReturn> Arrange<TReturn>(Type targetType, string memberName, params object[] args);

        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <typeparam name="TReturn">Return type</typeparam>
        /// <param name="targetType">Target type</param>
        /// <param name="memberName">Target member name</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        FuncExpectation<TReturn> Arrange<TReturn>(Type targetType, string memberName, Type[] typeArguments, params object[] args);

        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <param name="targetType">Target type</param>
        /// <param name="memberName">Target member name</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        ActionExpectation Arrange(Type targetType, string memberName, params object[] args);

        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <param name="targetType">Target type</param>
        /// <param name="memberName">Target member name</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        ActionExpectation Arrange(Type targetType, string memberName, Type[] typeArguments, params object[] args);

        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <typeparam name="TReturn">Return type</typeparam>
        /// <param name="method">Target method</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        FuncExpectation<TReturn> Arrange<TReturn>(MethodBase method, params object[] args);

        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <param name="method">Target method</param>
        /// <param name="args">Method arguments. Use <see cref="Arg.Expr"/> for predefined argument matchers.</param>
        ActionExpectation Arrange(MethodBase method, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="T">Specify the target type</typeparam>
        /// <param name="memberName">Name of the member</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert<T>(string memberName, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="T">Specify the target type</typeparam>
        /// <param name="memberName">Name of the member</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert<T>(string memberName, Type[] typeArguments, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="T">Specify the target type</typeparam>
        /// <typeparam name="TReturn">Specify the return type for the method</typeparam>
        /// <param name="memberName">Name of the member</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert<T, TReturn>(string memberName, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="T">Specify the target type</typeparam>
        /// <typeparam name="TReturn">Specify the return type for the method</typeparam>
        /// <param name="memberName">Name of the member</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert<T, TReturn>(string memberName, Type[] typeArguments, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="targetType">Type of the target</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert(Type targetType, string memberName, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="targetType">Type of the target</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert(Type targetType, string memberName, Type[] typeArguments, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="targetType">Type of the target</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert<TReturn>(Type targetType, string memberName, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="targetType">Type of the target</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert<TReturn>(Type targetType, string memberName, Type[] typeArguments, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified method that it is called as expected.
        /// </summary>
        /// <param name="method">Target method</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert(MethodBase method, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="T">Specify the target type</typeparam>
        /// <param name="memberName">Name of the member</param>
        /// <param name="args">Method arguments</param>
        void Assert<T>(string memberName, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="T">Specify the target type</typeparam>
        /// <param name="memberName">Name of the member</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="args">Method arguments</param>
        void Assert<T>(string memberName, Type[] typeArguments, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="T">Specify the target type</typeparam>
        /// <typeparam name="TReturn">Specify the return type for the method</typeparam>
        /// <param name="memberName">Name of the member</param>
        /// <param name="args">Method arguments</param>
        void Assert<T, TReturn>(string memberName, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="T">Specify the target type</typeparam>
        /// <typeparam name="TReturn">Specify the return type for the method</typeparam>
        /// <param name="memberName">Name of the member</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="args">Method arguments</param>
        void Assert<T, TReturn>(string memberName, Type[] typeArguments, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="targetType">Type of the target</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="args">Method arguments</param>
        void Assert(Type targetType, string memberName, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <param name="targetType">Type of the target</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="args">Method arguments</param>
        void Assert(Type targetType, string memberName, Type[] typeArguments, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="TReturn">Specify the return type method</typeparam>
        /// <param name="targetType">Type of the target</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="args">Method arguments</param>
        void Assert<TReturn>(Type targetType, string memberName, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="TReturn">Specify the return type method</typeparam>
        /// <param name="targetType">Type of the target</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="args">Method arguments</param>
        void Assert<TReturn>(Type targetType, string memberName, Type[] typeArguments, params object[] args);

        /// <summary>
        /// Asserts the specified method that it is called as expected.
        /// </summary>
        /// <param name="method">Target method</param>
        /// <param name="args">Method arguments</param>
        void Assert(MethodBase method, params object[] args);

        /// <summary>
        /// Returns the number of times the specified member was called.
        /// </summary>
        /// <param name="method">Target method</param>
        /// <param name="args">Method arguments</param>
        /// <returns>Number of calls.</returns>
        int GetTimesCalled(MethodBase method, params object[] args);

        /// <summary>
        /// Returns the number of times the specified member was called.
        /// </summary>
        /// <param name="type">Type of the target</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="args">Method arguments</param>
        /// <returns>Number of calls.</returns>
        int GetTimesCalled(Type type, string memberName, params object[] args);

        /// <summary>
        /// Returns the number of times the specified member was called.
        /// </summary>
        /// <param name="type">Type of the target</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="typeArguments">Generic type arguments of the member specified by memberName</param>
        /// <param name="args">Method arguments</param>
        /// <returns>Number of calls.</returns>
        int GetTimesCalled(Type type, string memberName, Type[] typeArguments, params object[] args);
#endif

        /// <summary>
        /// Raises an event specified using reflection. If the event is declared on a C# or VB class
        /// and has the default implementation for add/remove, then that event can also be raised using this 
        /// method, even with the profiler off.
        /// </summary>
        /// <param name="instance">Instance on which to raise the event.</param>
        /// <param name="eventInfo">The event to raise.</param>
        /// <param name="args">Arguments to pass to the event handlers.</param>
        void Raise(object instance, EventInfo eventInfo, params object[] args);

        /// <summary>
        /// Raises a static event specified using reflection. If the event is declared on a C# or VB class
        /// and has the default implementation for add/remove, then that event can also be raised using this 
        /// method, even with the profiler off.
        /// </summary>
        /// <param name="eventInfo">The event to raise.</param>
        /// <param name="args">Arguments to pass to the event handlers.</param>
        void Raise(EventInfo eventInfo, params object[] args);

        /// <summary>
        /// Raises an event by name. If the event is declared on a C# or VB class
        /// and has the default implementation for add/remove, then that event can also be raised using this 
        /// method, even with the profiler off.
        /// </summary>
        /// <param name="instance">Instance on which to raise the event.</param>
        /// <param name="eventName">The name of event to raise.</param>
        /// <param name="args">Arguments to pass to the event handlers.</param>
        void Raise(object instance, string eventName, params object[] args);

        /// <summary>
        /// Raises a static event by name. If the event is declared on a C# or VB class
        /// and has the default implementation for add/remove, then that event can also be raised using this 
        /// method, even with the profiler off.
        /// </summary>
        /// <param name="type">The type on which the event is declared.</param>
        /// <param name="eventName">The name of event to raise.</param>
        /// <param name="args">Arguments to pass to the event handlers.</param>
        void Raise(Type type, string eventName, params object[] args);

        /// <summary>
        /// Creates an accessor object that can invoke non-public methods and get/set non-public properties and fields.
        /// Equivalent to <code>new PrivateAccessor(instance)</code>.
        /// </summary>
        /// <param name="instance">Instance to which non-public access will be given.</param>
        /// <returns>Non-public accessor.</returns>
        PrivateAccessor MakePrivateAccessor(object instance);

        /// <summary>
        /// Creates an accessor object that can invoke static (Shared in Visual Basic) non-public methods and static get/set non-public properties and fields.
        /// Equivalent to <code>PrivateAccessor.ForType(type)</code>.
        /// </summary>
        /// <param name="type">Type whose static members will be given non-public access to.</param>
        /// <returns>Non-public accessor.</returns>
        PrivateAccessor MakeStaticPrivateAccessor(Type type);

        /// <summary>
        /// Returns a dynamic wrapper of the given object that can be used to arrange non-public instance members on the object.
        /// </summary>
        /// <param name="instance">The object to wrap.</param>
        /// <returns>A wrapper that can be passed to Arrange.</returns>
        dynamic Wrap(object instance);

        /// <summary>
        /// Returns a dynamic wrapper of the given type that can be used to arrange non-public static members on the type.
        /// </summary>
        /// <param name="type">The type to wrap.</param>
        /// <returns>A wrapper that can be passed to Arrange.</returns>
        dynamic WrapType(Type type);

        /// <summary>
        /// Arranges an expectation on a void method given using a dynamic wrapper built with Wrap().
        /// </summary>
        /// <param name="dynamicExpression">An expression built using a wrapper returned by Wrap.</param>
        /// <returns>An expectation object that can further configure the arrangement</returns>
        /// <example>
        /// var mock = Mock.NonPublic.Wrap(myobj);
        /// Mock.NonPublic.Arrange(mock.PrivateMethod(ArgExpr.IsAny&lt;int&gt;()).DoNothing();
        /// Mock.NonPublic.Arrange(mock.Value = ArgExpr.IsAny&lt;int&gt;()).MustBeCalled();
        /// 
        /// var staticMock = Mock.NonPublic.WrapType(typeof(MyType));
        /// Mock.NonPublic.Arrange(staticMock.PrivateMethod()).DoNothing();
        /// </example>
        ActionExpectation Arrange(dynamic dynamicExpression);

        /// <summary>
        /// Arranges an expectation on a non-void method given using a dynamic wrapper built with Wrap().
        /// </summary>
        /// <param name="dynamicExpression">An expression built using a wrapper returned by Wrap.</param>
        /// <returns>An expectation object that can further configure the arrangement</returns>
        /// <example>
        /// var mock = Mock.NonPublic.Wrap(myobj);
        /// Mock.NonPublic.Arrange&lt;int&gt;(mock.PrivateValue).Returns(123);
        /// Mock.NonPublic.Arrange&lt;string&gt;(mock.GetNamePrivate(123)).Returns("me");
        /// </example>
        FuncExpectation<TReturn> Arrange<TReturn>(dynamic dynamicExpression);

        /// <summary>
        /// Asserts an expectation given using a dynamic wrapper built with Wrap()
        /// </summary>
        /// <param name="dynamicExpression">An expression built using a wrapper returned by Wrap.</param>
        /// <param name="occurs">Occurrence expectation to assert.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        void Assert(dynamic dynamicExpression, Occurs occurs, string message = null);

        /// <summary>
        /// Asserts an expectation given using a dynamic wrapper built with Wrap()
        /// </summary>
        /// <param name="dynamicExpression">An expression built using a wrapper returned by Wrap.</param>
        /// <param name="args">Additional arguments to clarify the assertion expression.</param>
        /// <param name="occurs">Occurrence expectation to assert.</param>
        /// <param name="message">A message to display if the assertion fails.</param>
        void Assert(dynamic dynamicExpression, Args args, Occurs occurs, string message = null);

#if !PORTABLE
        /// <summary>
        /// Non public ref return interface for mocking.
        /// </summary>
        INonPublicRefReturnExpectation RefReturn { get; }
#endif
    }
}
