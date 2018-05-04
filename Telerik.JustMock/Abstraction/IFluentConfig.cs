/*
 JustMock Lite
 Copyright © 2010-2015 Telerik EAD

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
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Telerik.JustMock.Abstraction
{
	/// <summary>
	/// Interface providing methods for setting up mock instance.
	/// </summary>
	/// <typeparam name="T">Target type</typeparam>
	public interface IFluentConfig<T> : IFluentConfig
	{
		/// <summary>
		/// Implements interface to the target mock.
		/// </summary>
		/// <returns>Reference to <see cref="IFluentConfig{T}"/></returns>
		IFluentConfig<T> Implements<TInterface>();

		/// <summary>
		/// Invokes the specified constructor. 
		/// </summary>
		/// <param name="expression"></param>
		/// <returns>Reference to <see cref="IFluentConfig{T}"/></returns>
		IFluentConfig<T> CallConstructor(Expression<Func<T>> expression);
	}

	/// <summary>
	/// Interface providing handy methods for setting up mock instance.
	/// </summary>
	public interface IFluentConfig
	{
		/// <summary>
		/// Sets the behavior of the mock object.
		/// </summary>
		/// <param name="behavior">The mock behavior.</param>
		/// <returns>The fluent configuration.</returns>
		IFluentConfig SetBehavior(Behavior behavior);

		/// <summary>
		/// Specifies to mock the constructor.
		/// </summary>
		/// <returns>The fluent configuration.</returns>
		IFluentConfig MockConstructor();

		/// <summary>
		/// Invokes the specified non-public constructor.
		/// </summary>
		/// <param name="args">Arguments to be passed to the constructor.</param>
		/// <returns>The fluent configuration.</returns>
		IFluentConfig CallConstructor(object[] args);

#if !PORTABLE
		/// <summary>
		/// Add an attribute to the created proxy type.
		/// </summary>
		/// <param name="attributeBuilder">An attribute builder object containing the attribute.</param>
		/// <returns>The fluent configuration.</returns>
		IFluentConfig AddAttributeToProxy(CustomAttributeBuilder attributeBuilder);

		/// <summary>
		/// Sets a predicate that will filter whether a dynamic proxy method will be intercepted or not.
		/// </summary>
		/// <remarks>
		/// Dynamic proxy methods are the methods that proxy calls from interface methods, abstract methods or
		/// inherited virtual methods. If a filter is not specified, then, by default, all methods are
		/// intercepted. If a method is not intercepted, it cannot be mocked.
		/// 
		/// The interceptor filter allows you to specify which methods will be intercepted and which won't be.
		/// Normally, you'd want to intercept all methods. However, there are practical limitations to the
		/// number of members that can be intercepted on a given type. If a type has more than about 5000
		/// interceptable members, then the time needed to generate the proxy type may be impractically long.
		/// In those cases specify a filter that will remove from interception those members that you don't
		/// intend to mock anyway.
		/// </remarks>
		/// <param name="filter"></param>
		/// <returns>The fluent configuration.</returns>
		IFluentConfig SetInterceptorFilter(Expression<Predicate<MethodInfo>> filter);
#endif
	}
}
