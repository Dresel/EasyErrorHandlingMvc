// See https://github.com/roblevine/UnityLog4NetExtension

namespace EasyErrorHandlingMvc.Samples.Mvc5Extended.Core.Unity
{
	using Microsoft.Practices.ObjectBuilder2;
	using NLog;

	public class SimpleErrorHandlingNLogAdapterStrategy : BuilderStrategy
	{
		public override void PreBuildUp(IBuilderContext context)
		{
			ICreationStackTrackerPolicy policy = context.Policies.Get<ICreationStackTrackerPolicy>(buildKey: null,
				localOnly: true);

			if (policy.TypeStack.Count >= 3 && policy.TypeStack.Peek(0) == typeof(Logger) &&
				policy.TypeStack.Peek(1) == typeof(NLogAdapter))
			{
				context.Existing = LogManager.GetLogger(policy.TypeStack.Peek(2).FullName);
			}
			else if (policy.TypeStack.Count >= 2 && policy.TypeStack.Peek(0) == typeof(Logger))
			{
				context.Existing = LogManager.GetLogger(policy.TypeStack.Peek(1).FullName);
			}

			base.PreBuildUp(context);
		}
	}
}