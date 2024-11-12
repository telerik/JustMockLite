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


#if NUNIT
using FrameworkAssert = NUnit.Framework.Assert;
#elif XUNIT
using FrameworkAssert = Xunit.Assert;
#elif PORTABLE
using FrameworkAssert = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert;
#else
using FrameworkAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
#endif



namespace Telerik.JustMock.Tests
{
    /// <summary>
    /// Assertion wrapper that exposes Xunit alike methods.
    /// </summary>
    public static class Assert
    {
        public static Exception Throws<T>(Action action) where T : Exception
        {
            Exception targetException = null;

            try
            {
                action();
            }
            catch (T ex)
            {
                // Test pass
                return ex;
            }
#if PORTABLE
            catch (System.Reflection.TargetInvocationException ex)
            {
                var inner = ex.InnerException;
                if (inner is T)
                {
                    return inner;
                }
                else
                {
                    FrameworkAssert.Fail(String.Format("Wrong exception type thrown. Expected {0}, got {1}.", typeof(T), inner.GetType()));
                }
            }
#endif
            catch (Exception ex)
            {
#if XUNIT
                FrameworkAssert.True(false,String.Format("Wrong exception type thrown. Expected {0}, got {1}.", typeof(T), ex.GetType()));
#else
                FrameworkAssert.Fail(String.Format("Wrong exception type thrown. Expected {0}, got {1}.", typeof(T), ex.GetType()));
#endif
            }
#if XUNIT
            FrameworkAssert.True(false, String.Format("No Expected {0} was thrown", typeof(T).FullName));
#else
            FrameworkAssert.Fail(String.Format("No Expected {0} was thrown", typeof(T).FullName));
#endif
            throw new Exception();
        }

        public static void NotNull(object value)
        {
#if XUNIT
            FrameworkAssert.NotNull(value);
#else
            FrameworkAssert.IsNotNull(value);
#endif
        }

        public static void Null(object value)
        {
#if XUNIT
            FrameworkAssert.Null(value);
#else
            FrameworkAssert.IsNull(value);
#endif
        }

        public static void Equal<T>(T expected, T actual)
        {
#if XUNIT
            FrameworkAssert.Equal(expected, actual);
#else
            FrameworkAssert.AreEqual(expected, actual);
#endif
        }

        public static void NotEqual<T>(T notExpected, T actual)
        {
#if XUNIT
            FrameworkAssert.NotEqual(notExpected, actual);
#else
            FrameworkAssert.AreNotEqual(notExpected, actual);
#endif
        }

        public static void True(bool condition)
        {
#if XUNIT
            FrameworkAssert.True(condition);
#else
            FrameworkAssert.IsTrue(condition);
#endif
        }

        public static void False(bool condition)
        {
#if XUNIT
            FrameworkAssert.False(condition);
#else
            FrameworkAssert.IsFalse(condition);
#endif
        }

        public static void Same(object expected, object actual)
        {
#if XUNIT
            FrameworkAssert.Same(expected, actual);
#else
            FrameworkAssert.AreSame(expected, actual);
#endif
        }

        public static void NotSame(object expected, object actual)
        {
#if XUNIT
            FrameworkAssert.NotSame(expected, actual);
#else
            FrameworkAssert.AreNotSame(expected, actual);
#endif
        }
    }
}