/*
 JustMock Lite
 Copyright © 2010-2015 Telerik EAD

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
using System.Linq;

namespace Telerik.JustMock.Setup
{
	/// <summary>
	/// Used to disable the automatic generation of calls to Mock.Reset() in test methods by the profiler.
	/// </summary>
	/// <remarks>
	/// When the JustMock profiler is enabled, it is quite necessary to call Mock.Reset() at the end of
	/// every test method that does mocking or calls other methods that do mocking. This way you'll be
	/// sure that no state leaks from test to test and your tests always run at top speed.
	/// 
	/// The JustMock profiler automatically adds calls to Mock.Reset() to the end of every test method
	/// that is in an assembly that has a reference to the Telerik.JustMock assembly. If your test method looks like
	/// <code>
	/// [Test]
	/// public void MyTest()
	/// {
	///     ... // body of test
	/// }
	/// </code>
	/// ...then the profiler wraps it in a try/finally block like so:
	/// <code>
	/// [Test]
	/// public void MyTest()
	/// {
	///     try {
	///         ... // body of test
	///     } finally {
	///         Mock.Reset();
	///     }
	/// }
	/// </code>
	/// ...thus ensuring that Mock.Reset() is called at the end of every test method without any
	/// burden imposed on the test author.
	/// 
	/// Sometimes this try/finally block gets in your way though. For example, try/catch/finally
	/// blocks change the lifetime of objects in such a way that objects referenced only by WeakReferences
	/// will not be collected in the scope they're handled if you call GC.Collect() in that same scope.
	/// 
	/// Whenever you have a test that passes with the JustMock profiler disabled, but fails when the profiler
	/// is enabled, *and* you don't use mocking in that test, the issue may be resolved by decorating your
	/// test with this attribute.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class DisableAutomaticRepositoryResetAttribute : Attribute
	{
		/// <summary>
		/// Allow the use of the mocking API inside this method. You must explicitly call
		/// Mock.Reset() at the end of your test method.
		/// </summary>
		public bool AllowMocking { get; set; }
	}
}
