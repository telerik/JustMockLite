// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if !MONO
namespace Telerik.JustMock.Core.Castle.Core.Internal
{
	using System;

	internal sealed class WeakKey
	{
        private readonly WeakReference weakRef;
        private readonly int hashCode;

		public WeakKey(object target, int hashCode)
		{
            this.weakRef = new WeakReference(target);
			this.hashCode = hashCode;
		}

		public object Target
		{
			get { return weakRef.Target; }
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public override bool Equals(object other)
		{
			return WeakKeyComparer<object>.Default.Equals(this, other);
		}
	}
}
#endif