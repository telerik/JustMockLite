/*
 JustMock Lite
 Copyright © 2010-2015 Telerik AD

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


#if !NUNIT
#if !PORTABLE
using FrameworkAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
#else
using FrameworkAssert = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert;
#endif
#else
using FrameworkAssert = NUnit.Framework.Assert;
#endif

namespace Telerik.JustMock.MSTest2.Tests
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
				FrameworkAssert.Fail(String.Format("Wrong exception type thrown. Expected {0}, got {1}.", typeof(T), ex.GetType()));
			}

			FrameworkAssert.Fail(String.Format("No Expected {0} was thrown", typeof(T).FullName));
			throw new Exception();
		}

		public static void NotNull(object value)
		{
			FrameworkAssert.IsNotNull(value);
		}

		public static void Null(object value)
		{
			FrameworkAssert.IsNull(value);
		}

		public static void Equal<T>(T expected, T actual)
		{
			FrameworkAssert.AreEqual(expected, actual);
		}

		public static void NotEqual<T>(T notExpected, T actual)
		{
			FrameworkAssert.AreNotEqual(notExpected, actual);
		}

		public static void True(bool condition)
		{
			FrameworkAssert.IsTrue(condition);
		}

		public static void False(bool condition)
		{
			FrameworkAssert.IsFalse(condition);
		}

		public static void Same(object expected, object actual)
		{
			FrameworkAssert.AreSame(expected, actual);
		}

		public static void NotSame(object expected, object actual)
		{
			FrameworkAssert.AreNotSame(expected, actual);
		}
	}
}
