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
	/// Defines members for setting up delegate to execute as mehtod body.
	/// </summary>
	public interface IDoInstead<TContainer>
	{
		/// <summary>
		/// Specifies the delegate that will execute for the expected method.
		/// </summary>
		/// <param name="action">delegate the method body</param>
		/// <returns></returns>
		TContainer DoInstead(Action action);

		/// <summary>
		/// Specifies the delegate that will execute for the expected method.
		/// </summary>
		/// <param name="delegate">Target delegate to evaluate.</param>
		TContainer DoInstead(Delegate @delegate);
		
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1>(Action<T1> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2>(Action<T1, T2> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3>(Action<T1, T2, T3> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T11">Type of the eleventh parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T11">Type of the eleventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T12">Type of the twelveth parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T11">Type of the eleventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T12">Type of the twelveth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T13">Type of the thirteenth parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T11">Type of the eleventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T12">Type of the twelveth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T13">Type of the thirteenth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T14">Type of the fourteenth parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T11">Type of the eleventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T12">Type of the twelveth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T13">Type of the thirteenth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T14">Type of the fourteenth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T15">Type of the fifteenth parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action);
				
		///<summary>
		/// Specifies the delegate that will execute for the expected method.
		///</summary>
		/// <typeparam name="T1">Type of the first parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T11">Type of the eleventh parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T12">Type of the twelveth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T13">Type of the thirteenth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T14">Type of the fourteenth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T15">Type of the fifteenth parameter of the method that this delegate encapsulates</typeparam>
		/// <typeparam name="T16">Type of the sixteenth parameter of the method that this delegate encapsulates</typeparam>
		///	<param name="action">Target action delegate to execute as method body</param>
		TContainer DoInstead<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action);
				
	}
}
