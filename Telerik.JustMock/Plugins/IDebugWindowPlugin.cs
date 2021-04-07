/*
 JustMock Lite
 Copyright © 2020 - 2021 Progress Software Corporation

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
using System.Reflection;
using Telerik.JustMock.AutoMock.Ninject.Modules;
using Telerik.JustMock.Core;

#if !PORTABLE
namespace Telerik.JustMock.Plugins
{
    internal interface ITraceEventsPublisher
    {
        void TraceMessage(string message);
    }

    internal interface IMockRepositoryEventsPublisher
    {
        void MockCreated(int repositoryId, string repositoryPath, MockInfo mock, MatcherInfo[] argumentMatchers);
        void MockInvoked(int repositoryId, string repositoryPath, MockInfo mock, InvocationInfo invocation);
        void RepositoryCreated(int repositoryId, string repositoryPath, MethodMockInfo methodInfo);
        void RepositoryRetired(int repositoryId, string repositoryPath);
    }

    internal interface IDebugWindowPlugin : ITraceEventsPublisher, IMockRepositoryEventsPublisher, INinjectModule, IDisposable
    {

    }
}
#endif
