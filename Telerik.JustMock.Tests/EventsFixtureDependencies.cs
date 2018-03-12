
using System;

namespace Telerik.JustMock.Tests.EventFixureDependencies
{
	public class FooArgs : EventArgs
	{
		public FooArgs()
		{
		}

		public FooArgs(string value)
		{
			this.Value = value;
		}

		public string Value { get; set; }
	}

	public interface IExecutor<T>
	{
		event EventHandler<FooArgs> Done;
		event EventHandler Executed;
		void Execute(T value);
		void Execute(string value);
		void Execute(string s, int i);
		void Execute(string s, int i, bool b);
		void Execute(string s, int i, bool b, string v);

		string Echo(string s);
	}

	public class Foo
	{

	}

	public interface IFoo
	{
		event CustomEvent CustomEvent;
		event EchoEvent EchoEvent;
		void RaiseMethod();
		string Echo(string arg);
		void Execute();
	}

	public class ProjectNavigatorViewModel
	{
		public ProjectNavigatorViewModel(ISolutionService solutionService)
		{
			this.SolutionService = solutionService;
			SolutionService.ProjectAdded += new EventHandler<ProjectEventArgs>(SolutionService_ProjectAdded);
		}

		void SolutionService_ProjectAdded(object sender, ProjectEventArgs e)
		{
			IsProjectAddedCalled = true;
		}

		public ISolutionService SolutionService { get; set; }
		public bool IsProjectAddedCalled { get; set; }
	}

	public class ProjectEventArgs : EventArgs
	{
		private IFoo foo;

		public ProjectEventArgs(IFoo foo)
		{
			this.foo = foo;
		}
	}

	public interface ISolutionService
	{
		event EventHandler<ProjectEventArgs> ProjectAdded;
	}

	public delegate void CustomEvent(string value);
	public delegate void EchoEvent(bool echoed);

	public class SolutionService : ISolutionService
	{
		public event EventHandler<ProjectEventArgs> ProjectAdded;
	}
}
