// Licensed under the Apache License, Version 2.0.
// See LICENSE in the repository root for license information.

namespace Telerik.JustMock.Core.Castle.DynamicProxy.Internal
{
    using System;

    internal sealed class NoTargetException : NotImplementedException
    {
        internal NoTargetException(string message)
            : base(message)
        {
        }
    }
}
