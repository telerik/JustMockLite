﻿/*
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

using System.Diagnostics;

namespace Telerik.JustMock.Diagnostics
{
    // This class is created to overcome an issues breaking the application when calling Debug.Assert in .Net Core when there is no debugger attached to the process.
    internal static class JMDebug
    {
        internal static void Assert(bool condition)
        {
            if (Debugger.IsAttached)
            {
                Debug.Assert(condition);
            }
        }

        internal static void Assert(bool condition, string message)
        {
            if (Debugger.IsAttached)
            {
                Debug.Assert(condition, message);
            }
        }
    }
}