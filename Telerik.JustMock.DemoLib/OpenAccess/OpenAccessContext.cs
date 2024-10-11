/*
 JustMock Lite
 Copyright © 2010-2014 Progress Software Corporation

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

namespace Telerik.JustMock.DemoLib
{
    public class OpenAccessContextBase : System.Object
    {
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public OpenAccessContextBase() { }

        public void SaveChanges()
        {
            this.SaveChanges();
        }

        /// <summary>
        /// Saves the changes with the specified concurency mode
        /// </summary>
        /// <param name="failureMode">Mode to use</param>
        public virtual void SaveChanges(string failureMode)
        {

        }

        public void Add(System.Collections.IEnumerable entities)
        {
            this.Add(entities);
        }

        public void Add(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
