/*
 JustMock Lite
 Copyright © 2010-2015 Progress Software Corporation

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

#region JustMock Test Attributes
#if NUNIT
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#elif XUNIT
using Xunit;
using Telerik.JustMock.XUnit.Test.Attributes;
using TestCategory = Telerik.JustMock.XUnit.Test.Attributes.XUnitCategoryAttribute;
using TestClass = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestClassAttribute;
using TestMethod = Xunit.FactAttribute;
using TestInitialize = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestInitializeAttribute;
using TestCleanup = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestCleanupAttribute;
using AssertionException = Telerik.JustMock.XUnit.AssertFailedException;
#elif VSTEST_PORTABLE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AssertionException = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssertFailedException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#endif
#endregion


namespace Telerik.JustMock.Tests
{
    [TestClass]
    public class WaitEventFixture
    {
        [TestMethod, TestCategory("Lite"), TestCategory("Events")]
        public void ShouldWaitForSpecificDurationBeforeRasingTheEvent()
        {
            string userName = string.Empty;
            string password = string.Empty;

            var mockLogger = Mock.Create<ILogger>();

            Mock.Arrange(() => mockLogger.LogMessage(userName)).OccursOnce();

            var mockValidator = Mock.Create<IUserValidationService>();

            Mock.Arrange(() => mockValidator.ValidateUser(userName, password))
                .Raises(() => mockValidator.CustomEvent += null, userName, Wait.For(2))
                .Returns(true);

            var sut = new Login(mockValidator, mockLogger);

            Assert.Equal(true, sut.LoginUser(userName, password));

            Mock.Assert(mockLogger);
            Mock.Assert(mockValidator);

            Assert.True(sut.ElapsedTime.Seconds >= 1);
        }

        public class Login
        {
            private readonly IUserValidationService _validationService;

            private readonly ILogger _logger;

            //Next two properties are added to show Delayed Event Execution
            public TimeSpan ElapsedTime { get; private set; }
            private DateTime _startTime;

            public Login(IUserValidationService service)
                : this(service, null)
            {
            }

            public Login(IUserValidationService service, ILogger logger)
            {
                _logger = logger;
                _validationService = service;
                _validationService.CustomEvent +=
                    new CustomEventHandler(HandleValidationEvent);
            }

            void HandleValidationEvent(string message)
            {
                ///Thread.Sleep(1000);
                ElapsedTime = DateTime.Now - _startTime;
                if (_logger != null)
                {
                    _logger.LogMessage(message);
                }
            }

            public bool LoginUser(string userName, string password)
            {
                ElapsedTime = new TimeSpan(0);
                _startTime = DateTime.Now;
                return _validationService.ValidateUser(userName, password);
            }
        }

        public interface ILogger
        {
            void LogMessage(string message);
        }

        public interface IUserValidationService
        {
            bool ValidateUser(string userName, string password);
            event EventHandler StandardEvent;
            event CustomEventHandler CustomEvent;
        }

        public delegate void CustomEventHandler(string s);

        public class CustomEventArgs : EventArgs
        {
            public string Name { get; private set; }

            public CustomEventArgs(string name)
            {
                this.Name = name;
            }
        }
    }
}
