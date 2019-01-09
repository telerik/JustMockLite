/*
 JustMock Lite
 Copyright © 2010-2014 Progress Software Corporation

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

using System.Globalization;
using Telerik.Sitefinity.Model;
using ModelContent = Telerik.Sitefinity.GenericContent.Model.Content;

namespace Telerik.Sitefinity.Fluent.AnyContent.Implementation
{
	public class AnyContentManager : IAnyContentManager
	{
		public ModelContent Unpublish(ModelContent item, CultureInfo culture)
		{
			var hasTracking = item as IHasTrackingContext;
			if (hasTracking != null)
			{
				hasTracking.RegisterOperation(OperationStatus.Unpublished, null);
			}
			return null;
		}
	}
}
