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
			CorrespondingRenderingHttpStatusCode = new List<Func<Exception, HttpStatusCode?>>();
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

			LoggedHttpContextItemName = "LoggedByEasyErrorHandling";
		}

		public static string ErrorHandlingControllerName
		{
			get
			{
				return "ErrorHandling";
			}
		}

		public static ICollection<Func<Exception, HttpStatusCode?>> CorrespondingRenderingHttpStatusCode { get; set; }

		public static IDictionary<HttpStatusCode, string> ErrorViewPaths { get; set; }

		public static string FatalErrorFallbackContent { get; set; }

		public static string FatalErrorFilePath { get; set; }

		public static string LoggedHttpContextItemName { get; set; }

		public static void RenderDangerousParametersAs(HttpStatusCode renderedHttpStatusCode)
		{
			CorrespondingRenderingHttpStatusCode.Add(exception =>
			{
				if (exception is HttpRequestValidationException)
				{
					return renderedHttpStatusCode;
				}

				return null;
			});
		}

		public static void RenderEverythingElseAs(HttpStatusCode renderedHttpStatusCode)
		{
			CorrespondingRenderingHttpStatusCode.Add(exception => renderedHttpStatusCode);
		}

		public static void RenderHttpExceptionAs(HttpStatusCode exceptionHttpStatusCode, HttpStatusCode renderedHttpStatusCode)
		{
			CorrespondingRenderingHttpStatusCode.Add(exception =>
			{
				HttpException httpException = exception as HttpException;

				if ((httpException != null) && (httpException.GetHttpCode() == (int)exceptionHttpStatusCode))
				{
					return renderedHttpStatusCode;
				}

				return null;
			});
		}

		public static void RenderInvalidParametersAs(HttpStatusCode renderedHttpStatusCode)
		{
			CorrespondingRenderingHttpStatusCode.Add(exception =>
			{
				if ((exception is ArgumentException) && (exception.TargetSite.DeclaringType == typeof(ActionDescriptor)))
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