/*
 JustMock Lite
 Copyright © 2010-2014 Telerik AD

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

namespace Telerik.JustMock.Expectations.Abstraction
{
	/// <summary>
	/// Interface containing Func type method expecatations.
	/// </summary>
	/// <typeparam name="TReturn"></typeparam>
	public interface IFunc<TReturn> : IThrows<IFunc<TReturn>>, IReturns<TReturn>, IAssertable
	{
		/// <summary>	
		/// Specifies the return value for the expected method.
		/// </summary>
		/// <param name="value">any object value</param>
		/// <returns></returns>
		IAssertable Returns(TReturn value);

		/// <summary>
		/// Specifies the delegate to evaluate and return for the expected method.
		/// </summary>
		/// <param name="delegate">Target delegate to evaluate.</param>
		/// <returns>Reference to <see cref="IAssertable"/> interface</returns>
		IAssertable Returns(Delegate @delegate);

		/// <summary>
		/// Specifies the delegate to evaluate and return for the expected method.
		/// </summary>
		/// <param name="func">Target delegate to evaluate</param>
		/// <returns>Reference to <see cref="IAssertable"/> interface</returns>
		IAssertable Returns(Func<TReturn> func);
		
#if !LITE_EDITION

		/// <summary>
		/// Returns a enumerable collecton for the target query.
		/// </summary>
		/// <typeparam name="TArg">Argument type</typeparam>
		/// <typeparam name="TReturn">Return type for the collection</typeparam>
		/// <param name="funcExpectation">Defines the setup instance under which the collection will be set.</param>
		/// <param name="collection">Enumerable colleciton</param>
		/// <returns>Instance of <see cref="IAssertable"/></returns>
		IAssertable ReturnsCollection<TArg>(IEnumerable<TArg> collection);
#endif
	}
}
