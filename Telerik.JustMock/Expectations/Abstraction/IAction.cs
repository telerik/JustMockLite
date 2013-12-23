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

namespace Telerik.JustMock.Expectations.Abstraction
{
	/// <summary>
	/// Defines common expecations.
	/// </summary>
	public interface IAction<TContainer> : IDoInstead<TContainer>, IThrows<TContainer>, IAssertable
	{
		///<summary>
		/// Raises the expected with sepecic arguments
		///</summary>
		///<param name="eventExpression"></param>
		///<param name="args"></param>
		///<returns></returns>
		///<exception cref="InvalidOperationException"></exception>
		TContainer Raises(Action eventExpression, params object[] args);

		///<summary>
		/// Raises the expected with sepecic arguments
		///</summary>
		///<param name="eventExpression"></param>
		///<param name="args">Event arguments</param>
		///<returns></returns>
		///<exception cref="InvalidOperationException"></exception>
		TContainer Raises(Action eventExpression, EventArgs args);

		///<summary>
		/// Raises the expected event for the setup.
		///</summary>
		///<param name="eventExpression"></param>
		///<param name="func">An function that will be used to construct event arguments</param>
		///<returns></returns>
		///<exception cref="InvalidOperationException"></exception>
		TContainer Raises<T1>(Action eventExpression, Func<T1, EventArgs> func);

		///<summary>
		/// Raises the expected event for the setup.
		///</summary>
		///<param name="eventExpression"></param>
		///<param name="func">An function that will be used to construct event arguments</param>
		///<returns></returns>
		///<exception cref="InvalidOperationException"></exception>
		TContainer Raises<T1, T2>(Action eventExpression, Func<T1, T2, EventArgs> func);

		///<summary>
		/// Raises the expected event for the setup.
		///</summary>
		///<param name="eventExpression"></param>
		///<param name="func">An function that will be used to construct event arguments</param>
		///<returns></returns>
		///<exception cref="InvalidOperationException"></exception>
		TContainer Raises<T1, T2, T3>(Action eventExpression, Func<T1, T2, T3, EventArgs> func);

		///<summary>
		/// Raises the expected event for the setup.
		///</summary>
		///<param name="eventExpression"></param>
		///<param name="func">An function that will be used to construct event arguments</param>
		///<returns></returns>
		///<exception cref="InvalidOperationException"></exception>
		TContainer Raises<T1, T2, T3, T4>(Action eventExpression, Func<T1, T2, T3, T4, EventArgs> func);

		/// <summary>
		///  Specfies call a to step over (loose mocks only).
		/// </summary>
		/// <remarks>
		/// For loose mocks by default the behavior is step over.
		/// </remarks>
		/// <returns>Refarence to <see cref="IAssertable"/></returns>
		IAssertable DoNothing();
	}
}
