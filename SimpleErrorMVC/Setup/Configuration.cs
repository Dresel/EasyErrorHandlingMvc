namespace SimpleErrorMVC.Setup
{
	public class Configuration
	{
		public static string ErrorControllerName = "Error";

		public static string FallbackContent = @"<!DOCTYPE HTML>
	<html>
		<head>
			<title>Internal Server Error</title>
		</head>
		<body>
			<h1>Internal Server Error</h1>
			<p>A fatal internal server error occured. We are sorry :(</p>
		</body>
	</html>";

		public static string FatalErrorFilePath = "~/Views/Error/FatalError.htm";

		public static string InternalServerErrorActionName = "InternalServerError";

		public static string InternalServerErrorUrl = "InternalServerError/";

		public static string InternalServerErrorViewPath = "~/Views/Error/InternalServerError.cshtml";

		public static string NotFoundActionName = "NotFound";

		public static string NotFoundUrl = "NotFound/";

		public static string NotFoundViewPath = "~/Views/Error/NotFound.cshtml";

		public static string UnauthorizedActionName = "Unauthorized";

		public static string UnauthorizedUrl = "Unauthorized/";

		public static string UnauthorizedViewPath = "~/Views/Error/Unauthorized.cshtml";
	}
}