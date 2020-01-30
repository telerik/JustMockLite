﻿/*
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
using System.Linq.Expressions;

namespace Telerik.JustMock.Core
{
    internal partial class ArgExprImplementation : IArgExpr
    {
        public Expression IsAny<T>()
        {
            return ArgExpr.IsAny<T>();
        }

        public Expression IsNull<T>()
        {
            return ArgExpr.IsNull<T>();
        }

        public Expression Matches<T>(Expression<Predicate<T>> match)
        {
            return ArgExpr.Matches<T>(match);
        }

        public Expression Out<T>(T value)
        {
            return ArgExpr.Out<T>(value);
        }

        public Expression Ref<T>(T value)
        {
            return ArgExpr.Ref<T>(value);
        }
    }
}