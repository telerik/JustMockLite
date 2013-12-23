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
using System.Linq;
using System.Linq.Expressions;

namespace Telerik.JustMock
{
    public sealed class MsCorlibInitializer
    {
        /// <summary>
        /// Initializes all method for the target mscorlib member.
        /// </summary>
        /// <param name="expression">Target expression</param>
        /// <typeparam name="TDelegate">Delegate of the container type</typeparam>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
        public void In<TDelegate>(Expression<Action<TDelegate>> expression)
        {
        }

        /// <summary>
        /// Initializes all method for the target mscorlib member.
        /// </summary>
        /// <typeparam name="T">Container type</typeparam>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
        public void In<T>()
        {
        }
    }
}
