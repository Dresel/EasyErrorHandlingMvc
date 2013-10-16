namespace SimpleErrorMVC.WebSampleExtended.Extension
{
	using Ninject.Modules;
	using SimpleErrorMVC.Logger;

	public class WebSampleExtendedNinjectModule : NinjectModule
	{
		public override void Load()
		{
			Bind<ILogger>().To<HttpContextLogger>();

			Bind<IFallbackFileResolver>().To<LocaleDependentFallbackFileResolver>();
		}
	}
}