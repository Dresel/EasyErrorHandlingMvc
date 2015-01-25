namespace EasyErrorHandlingMvc.Samples.Mvc5Extended
{
	using System.Web.Mvc;
	using EasyErrorHandlingMvc.Rendering;
	using EasyErrorHandlingMvc.Samples.Mvc5Extended.Core;
	using EasyErrorHandlingMvc.Samples.Mvc5Extended.Core.Unity;
	using Microsoft.Practices.Unity;
	using Unity.Mvc5;

	public static class UnityConfig
	{
		public static void RegisterComponents()
		{
			UnityContainer container = new UnityContainer();
			container.AddNewExtension<SimpleErrorHandlingNLogAdapterExtension>();

			// Register EasyErrorHandling Interfaces
			container.RegisterType<ILogger, NLogAdapter>();
			container.RegisterType<IFallbackFileResolver, LocaleSensitiveFallbackFileResolver>();

			DependencyResolver.SetResolver(new UnityDependencyResolver(container));
		}
	}
}