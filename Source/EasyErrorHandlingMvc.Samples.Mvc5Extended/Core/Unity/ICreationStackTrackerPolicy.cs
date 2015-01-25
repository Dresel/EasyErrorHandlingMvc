// See https://github.com/roblevine/UnityLog4NetExtension

namespace EasyErrorHandlingMvc.Samples.Mvc5Extended.Core.Unity
{
	using System;
	using Microsoft.Practices.ObjectBuilder2;

	public interface ICreationStackTrackerPolicy : IBuilderPolicy
	{
		PeekableStack<Type> TypeStack { get; }
	}
}