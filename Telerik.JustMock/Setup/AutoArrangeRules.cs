using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;

namespace Telerik.JustMock.Setup
{
	internal static class LooseBehaviorReturnRules
	{
		public static readonly List<ILooseBehaviorReturnRule> Rules = new List<ILooseBehaviorReturnRule>
		{
			new ArrayLooseBehaviorReturnRule(),
			new DictionaryLooseBehaviorReturnRule(),
			new EnumerableLooseBehaviorReturnRule(),
			new TaskLooseBehaviorReturnRule(),
		};

		internal static object CreateValue(LooseBehaviorReturnRequest request)
		{
			foreach (var rule in Rules)
			{
				var value = rule.CreateValue(request);
				if (value != null)
				{
					return value;
				}
			}
			return null;
		}
	}

	internal class LooseBehaviorReturnRequest
	{
		public Type Type { get; internal set; }
		public IMockMixin ArrangedMock { get; internal set; }
		public bool MustReturnMock { get; internal set; }

		internal RecursiveMockingBehavior Source { get; set; }
		internal Invocation Invocation { get; set; }

		public object CreateMock(Type type)
		{
			return this.Source.CreateMock(type, this.ArrangedMock.Repository, this.Invocation);
		}
	}

	internal interface ILooseBehaviorReturnRule
	{
		object CreateValue(LooseBehaviorReturnRequest request);
	}

	internal class ArrayLooseBehaviorReturnRule : ILooseBehaviorReturnRule
	{
		public object CreateValue(LooseBehaviorReturnRequest request)
		{
			return request.Type.IsArray
				? Array.CreateInstance(request.Type.GetElementType(), Enumerable.Repeat(0, request.Type.GetArrayRank()).ToArray())
				: null;
		}
	}

	internal class DictionaryLooseBehaviorReturnRule : ILooseBehaviorReturnRule
	{
		public object CreateValue(LooseBehaviorReturnRequest request)
		{
			var idictionaryType = request.Type.GetImplementationOfGenericInterface(typeof(IDictionary<,>));
			if (idictionaryType != null)
			{
				var dictType = typeof(Dictionary<,>).MakeGenericType(idictionaryType.GetGenericArguments());
				return MockCollection.Create(request.Type, request.ArrangedMock.Repository, request.ArrangedMock as IMockReplicator, (IEnumerable)MockingUtil.CreateInstance(dictType));
			}
			return null;
		}
	}

	internal class EnumerableLooseBehaviorReturnRule : ILooseBehaviorReturnRule
	{
		public object CreateValue(LooseBehaviorReturnRequest request)
		{
			var ienumerableType = request.Type.GetImplementationOfGenericInterface(typeof(IEnumerable<>));
			if (ienumerableType != null)
			{
				var listType = typeof(List<>).MakeGenericType(ienumerableType.GetGenericArguments());
				return MockCollection.Create(request.Type, request.ArrangedMock.Repository, request.ArrangedMock as IMockReplicator, (IEnumerable)MockingUtil.CreateInstance(listType));
			}
			return null;
		}
	}

	internal class TaskLooseBehaviorReturnRule : ILooseBehaviorReturnRule
	{
		public object CreateValue(LooseBehaviorReturnRequest request)
		{
			var type = request.Type;
			if (typeof(Task).IsAssignableFrom(type))
			{
				var elementType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>)
					? type.GetGenericArguments()[0] : typeof(object);
				var taskResultValue = request.MustReturnMock
					? request.CreateMock(elementType)
					: elementType.GetDefaultValue();

				Expression<Func<Task<object>>> taskFromResult = () => MockingUtil.TaskFromResult((object)null);
				return ((MethodCallExpression)taskFromResult.Body).Method
					.GetGenericMethodDefinition()
					.MakeGenericMethod(elementType)
					.Invoke(null, new object[] { taskResultValue });
			}
			return null;
		}
	}
}
