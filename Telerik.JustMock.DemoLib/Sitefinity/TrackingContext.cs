/*
 JustMock Lite
 Copyright © 2010-2014 Telerik EAD

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


namespace Telerik.Sitefinity.Model
{
	/// <summary>
	/// Marker interface for items that will keep track of operations performed (like: deleted, modified, added, all translation versions deleted).
	/// </summary>
	internal interface IHasTrackingContext
	{
		/// <summary>
		/// Gets the tracking context that keeps concrete data about operation performed.
		/// </summary>
		/// <value>The tracking context.</value>
		ITrackingContext TrackingContext { get; }
	}

	/// <summary>
	/// Interface will be used for registration operations tracking on items that implement <see cref="TrackingContext"/>.
	/// In multilingual mode registered operations are of language specific.
	/// </summary>
	internal interface ITrackingContext
	{
		/// <summary>
		/// Registers a deleted operation. If language is specified the deleted operation is registered for the specified language;
		/// Otherwise it is registered for all languages if the application is in multilingual mode.
		/// In monolingual mode deleted operation is registered.
		/// </summary>
		/// <param name="language">The language.</param>
		void RegisterDeletedOperation(string language);

		/// <summary>
		/// Registers a language specific operation.
		/// </summary>
		/// <param name="operation">The operation.</param>
		void RegisterOperation(OperationStatus operation, string language);
	}

	/// <summary>
	/// An enumeration that describes possible operations applied on the types that implement <see cref="IHasTrackingContext"/>.
	/// </summary>
	internal enum OperationStatus
	{
		None,
		Created,
		Modified,
		Deleted,
		DeletedWithAllTranslations,
		Published,
		Unpublished
	}

	internal class TrackingContext : ITrackingContext
	{
		/// <summary>
		/// Registers a deleted operation. If language is specified the deleted operation is registered for the specified language;
		/// Otherwise it is registered for all languages if the application is in multilingual mode.
		/// In monolingual mode deleted operation is registered.
		/// </summary>
		/// <param name="language">The language.</param>
		public void RegisterDeletedOperation(string language)
		{
			//in multilingual if language is not applied all translations should be registered.

		}

		/// <summary>
		/// Registers a specified operation.
		/// </summary>
		/// <param name="operation">The operation.</param>
		public void RegisterOperation(OperationStatus operation, string language)
		{
			if (!string.IsNullOrEmpty(language))
			{
			}
		}
	}

	internal static class HasTrackingContextExtensions
	{
		internal static void RegisterOperation(this IHasTrackingContext context, OperationStatus operation, string language)
		{
			var trackingContext = context.TrackingContext;
			trackingContext.RegisterOperation(operation, language);
		}
	}
}
