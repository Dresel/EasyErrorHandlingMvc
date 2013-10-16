namespace SimpleErrorMVC.WebSampleExtended
{
	using System;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Routing;
	using SimpleErrorMVC.WebSampleExtended.Extension;

	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : HttpApplication
	{
		public override void Init()
		{
			base.Init();

			BeginRequest += (sender, args) =>
			{
				if (Request.Url.AbsoluteUri.Contains("BeginRequestException"))
				{
					throw new Exception();
				}
			};
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			SimpleErrorMVCConfiguration.Configure();
		}
	}
}