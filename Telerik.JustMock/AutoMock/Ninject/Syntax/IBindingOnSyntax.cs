// -------------------------------------------------------------------------------------------------
// <copyright file="IBindingOnSyntax.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Telerik.JustMock.AutoMock.Ninject.Syntax
{
    using System;

    using Telerik.JustMock.AutoMock.Ninject.Activation;

    /// <summary>
    /// Used to add additional actions to be performed during activation or deactivation of instances via a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingOnSyntax<T> : IBindingSyntax
    {
        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnActivation(Action<T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnActivation<TImplementation>(Action<TImplementation> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnActivation(Action<IContext, T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnActivation<TImplementation>(Action<IContext, TImplementation> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnDeactivation(Action<T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnDeactivation<TImplementation>(Action<TImplementation> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnDeactivation(Action<IContext, T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnDeactivation<TImplementation>(Action<IContext, TImplementation> action);
    }
}