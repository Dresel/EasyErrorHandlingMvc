namespace EasyErrorHandlingMvc
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Web;
	using System.Web.Mvc;

	public static class Configuration
	{
		static Configuration()
		{
			CorrespondingRenderingHttpStatusCode = new List<Func<HttpContext, Exception, HttpStatusCode?>>();
			ErrorViewPaths = new Dictionary<HttpStatusCode, string>();
			FatalErrorFilePath = string.Empty;

			FatalErrorFallbackContent = @"<!DOCTYPE HTML>

<html>
	<head>
		<title>Internal Server Error</title>
	</head>
	<body>
		<h1>Internal Server Error</h1>
		<p>A fatal internal server error occured.</p>
	</body>
</html>";

			EasyErrorHandlingLoggedHttpContextItemName = "EasyErrorHandlingLogged";
			ErrorControllerRenderCalledHttpContextItemName = "ErrorControllerRenderCalled";

			ErrorHandlingControllerName = "ErrorHandling";
		}

		public static ICollection<Func<HttpContext, Exception, HttpStatusCode?>> CorrespondingRenderingHttpStatusCode { get;
			set; }

		public static string EasyErrorHandlingLoggedHttpContextItemName { get; set; }

		public static string ErrorControllerRenderCalledHttpContextItemName { get; set; }

		public static string ErrorHandlingControllerName { get; set; }

		public static IDictionary<HttpStatusCode, string> ErrorViewPaths { get; set; }

		public static string FatalErrorFallbackContent { get; set; }

		public static string FatalErrorFilePath { get; set; }

		public static void RenderEverythingElseAs(HttpStatusCode renderedHttpStatusCode)
		{
			CorrespondingRenderingHttpStatusCode.Add((httpContext, exception) => renderedHttpStatusCode);
		}

		public static void RenderGetDangerousParametersAs(HttpStatusCode renderedHttpStatusCode)
		{
			CorrespondingRenderingHttpStatusCode.Add((httpContext, exception) =>
			{
				if (exception is HttpRequestValidationException && httpContext.Request.HttpMethod == "GET")
				{
					return renderedHttpStatusCode;
				}

				return null;
			});
		}

		public static void RenderGetInvalidParametersAs(HttpStatusCode renderedHttpStatusCode)
		{
			CorrespondingRenderingHttpStatusCode.Add((httpContext, exception) =>
			{
				if ((exception is ArgumentException) && (exception.TargetSite.DeclaringType == typeof(ActionDescriptor)) &&
					httpContext.Request.HttpMethod == "GET")
				{
					return renderedHttpStatusCode;
				}

				return null;
			});
		}

		public static void RenderHttpExceptionAs(HttpStatusCode exceptionHttpStatusCode, HttpStatusCode renderedHttpStatusCode)
		{
			CorrespondingRenderingHttpStatusCode.Add((httpContext, exception) =>
			{
				HttpException httpException = exception as HttpException;

				if ((httpException != null) && (httpException.GetHttpCode() == (int)exceptionHttpStatusCode))
				{
					return renderedHttpStatusCode;
				}

				return null;
			});
		}

		public static void SetErrorViewPath(HttpStatusCode httpStatusCode, string viewPath)
		{
			ErrorViewPaths[httpStatusCode] = viewPath;
		}
	}
}