// See https://github.com/roblevine/UnityLog4NetExtension

namespace EasyErrorHandlingMvc.Samples.Mvc5Extended.Core.Unity
{
	using Microsoft.Practices.ObjectBuilder2;

	public class CreationStackTrackerStrategy : BuilderStrategy
	{
		public override void PostBuildUp(IBuilderContext context)
		{
			ICreationStackTrackerPolicy policy = context.Policies.Get<ICreationStackTrackerPolicy>(buildKey: null,
				localOnly: true);

			if (policy.TypeStack.Count > 0)
			{
				policy.TypeStack.Pop();
			}

			base.PostBuildUp(context);
		}

		public override void PreBuildUp(IBuilderContext context)
		{
			ICreationStackTrackerPolicy policy = context.Policies.Get<ICreationStackTrackerPolicy>(buildKey: null,
				localOnly: true);
			if (policy == null)
			{
				context.Policies.Set(typeof(ICreationStackTrackerPolicy), new CreationStackTrackerPolicy(), null);
				policy = context.Policies.Get<ICreationStackTrackerPolicy>(buildKey: null, localOnly: true);
			}

			policy.TypeStack.Push(context.BuildKey.Type);

			base.PreBuildUp(context);
		}
	}
}