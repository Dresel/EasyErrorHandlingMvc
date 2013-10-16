namespace SimpleErrorMVC.WebSampleExtended.Extension
{
	using SimpleErrorMVC.Setup;

	public static class SimpleErrorMVCConfiguration
	{
		public static void Configure()
		{
			// If you dont want to use default structure, change ...
			Configuration.UnauthorizedViewPath = "~/Views/ErrorCustom/Unauthorized.cshtml";
			Configuration.NotFoundViewPath = "~/Views/ErrorCustom/NotFound.cshtml";
			Configuration.InternalServerErrorViewPath = "~/Views/ErrorCustom/InternalServerError.cshtml";

			// If you change these, don't forget to change the web.config also ...
			Configuration.UnauthorizedUrl = "Custom/Unauthorized/Url";
			Configuration.NotFoundUrl = "Custom/Not/Found/Url";
			Configuration.InternalServerErrorUrl = "Custom/Internal/Server/Error/Url";
		}
	}
}