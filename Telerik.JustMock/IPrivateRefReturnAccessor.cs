/*
 JustMock Lite
 Copyright © 2018 Telerik EAD

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

namespace Telerik.JustMock
{
    /// <summary>
    /// Gives access to the non-public ref returns members of a type or instance.
    /// </summary>
    public interface IPrivateRefReturnAccessor
    {
        /// <summary>
        /// Calls the specified method by name.
        /// </summary>
        /// <typeparam name="TRefReturn">Ref return type</typeparam>
        /// <param name="name">The name of the method to call.</param>
        /// <param name="args">Arguments to pass to the method.</param>
        /// <returns>The ref value returned by the specified method.</returns>
        ref TRefReturn CallMethod<TRefReturn>(string name, params object[] args);

        /// <summary>
        /// Gets the ref return value of a property by name.
        /// </summary>
        /// <typeparam name="TRefReturn">Ref return type</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        ref TRefReturn GetProperty<TRefReturn>(string name);
    }
}