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
using System.Linq;

namespace Telerik.JustMock.Core.MatcherTree
{
	internal class MethodMockMatcherTreeNode : MatcherTreeNode
	{
		public int Id { get; private set; }
		public IMethodMock MethodMock { get; set; }
		public MethodMockMatcherTreeNode(IMethodMock methodMock = null, int id = 0)
			: base(null)
		{
			MethodMock = methodMock;
			Id = id;
		}

		public override IMatcherTreeNode Clone()
		{
			return new MethodMockMatcherTreeNode(MethodMock, Id);
		}

		public IMatcherTreeNode DetachMethodMock()
		{
			IMatcherTreeNode current = this;
			while (current.Parent != null && current.Parent.Children.Count == 1)
			{
				current.Parent.Children.Clear();
				current = current.Parent;
			}

			if (current.Parent != null)
				current.Parent.Children.Remove(current);

			while (current.Parent != null)
				current = current.Parent;

			return current;
		}

		public void ReattachMethodMock()
		{
			var root = DetachMethodMock();
			((MethodInfoMatcherTreeNode)root).AddChild(MethodMock.CallPattern, this);
		}
	}
}
