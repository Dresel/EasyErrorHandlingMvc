namespace EasyErrorHandlingMvc.Samples.Mvc5Extended.Core.Unity
{
	using System;
	using System.Collections.Generic;

	public class CreationStackTrackerPolicy : ICreationStackTrackerPolicy
	{
		private readonly PeekableStack<Type> typeStack = new PeekableStack<Type>();

		public PeekableStack<Type> TypeStack
		{
			get
			{
				return this.typeStack;
			}
		}
	}
}