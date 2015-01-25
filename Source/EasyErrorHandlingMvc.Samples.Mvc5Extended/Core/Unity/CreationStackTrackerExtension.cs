// See https://github.com/roblevine/UnityLog4NetExtension

namespace EasyErrorHandlingMvc.Samples.Mvc5Extended.Core.Unity
{
	using Microsoft.Practices.Unity;
	using Microsoft.Practices.Unity.ObjectBuilder;

	public class CreationStackTrackerExtension : UnityContainerExtension
	{
		protected override void Initialize()
		{
			Context.Strategies.AddNew<CreationStackTrackerStrategy>(UnityBuildStage.TypeMapping);
		}
	}
}