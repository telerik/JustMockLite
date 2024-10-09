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
using Telerik.JustMock.Core;
using System.Linq.Expressions;


namespace Telerik.JustMock
{
    /// <summary>
    /// Provides various argument matching shortcuts.
    /// </summary>
    public partial interface IArgExpr
    {

        /// <summary>
        /// Matches argument can contain any int value.
        /// </summary>
        Expression AnyInt { get; }

        /// <summary>
        /// Matches argument can contain any float value.
        /// </summary>
        Expression AnyFloat { get; }

        /// <summary>
        /// Matches argument can contain any double value.
        /// </summary>
        Expression AnyDouble { get; }

        /// <summary>
        /// Matches argument can contain any decimal value.
        /// </summary>
        Expression AnyDecimal { get; }

        /// <summary>
        /// Matches argument can contain any long value.
        /// </summary>
        Expression AnyLong { get; }

        /// <summary>
        /// Matches argument can contain any char value.
        /// </summary>
        Expression AnyChar { get; }

        /// <summary>
        /// Matches argument can contain any string value.
        /// </summary>
        Expression AnyString { get; }

        /// <summary>
        /// Matches argument can contain any object value.
        /// </summary>
        Expression AnyObject { get; }

        /// <summary>
        /// Matches argument can contain any short value.
        /// </summary>
        Expression AnyShort { get; }

        /// <summary>
        /// Matches argument can contain any bool value.
        /// </summary>
        Expression AnyBool { get; }

        /// <summary>
        /// Matches argument can contain any Guid value.
        /// </summary>
        Expression AnyGuid { get; }

        /// <summary>
        /// Matches argument can contain any DateTime value.
        /// </summary>
        Expression AnyDateTime { get; }

        /// <summary>
        /// Matches argument can contain any TimeSpan value.
        /// </summary>
        Expression AnyTimeSpan { get; }
            
        /// <summary>
        /// Matches argument can contain any byte value.
        /// </summary>
        Expression AnyByte { get; }
            
        /// <summary>
        /// Matches argument can contain any SByte value.
        /// </summary>
        Expression AnySByte { get; }

        /// <summary>
        /// Matches argument can contain any Uri value.
        /// </summary>
        Expression AnyUri { get; }

    }
}
