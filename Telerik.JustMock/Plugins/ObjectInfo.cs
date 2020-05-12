/*
 JustMock Lite
 Copyright © 2020 Progress Software Corporation

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

namespace Telerik.JustMock.Plugins
{
    public class ObjectInfo
    {
        public Type Type { get; private set; }
        public object Value { get; private set; }

        private ObjectInfo(Type type, object value)
        {
            this.Type = type;
            this.Value = value;
        }

        public static ObjectInfo FromNullObject(Type type)
        {
            return new ObjectInfo(type, null);
        }

        public static ObjectInfo FromObject(object value)
        {
            return new ObjectInfo(value.GetType(), value);
        }
    }
}
