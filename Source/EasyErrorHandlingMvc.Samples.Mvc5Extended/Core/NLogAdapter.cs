namespace EasyErrorHandlingMvc.Samples.Mvc5Extended.Core
{
	using System;
	using System.Net;
	using System.Text;
	using System.Web;
	using System.Web.Helpers;
	using System.Web.Routing;
	using NLog;

	public class NLogAdapter : ILogger
	{
		public NLogAdapter(Logger logger)
		{
			Logger = logger;
		}

		public Logger Logger { get; set; }

		public void Log(string message, Exception exception, HttpContext context)
		{
			message = string.Format("{0}<br />AdditionalInformation<br />{1}<br />", message, GetHttpContextInformation(context));

			if (exception == null || IsHttpCode(exception, HttpStatusCode.NotFound))
			{
				Logger.Info(message, exception);
			}
			else
			{
				Logger.Error(message, exception);
			}
		}

		public void Log(string message, Exception exception, RequestContext context)
		{
			message = string.Format("{0}<br />AdditionalInformation<br />{1}<br />{2}", message,
				GetRouteDataInformation(context.RouteData),
				GetHttpContextInformation(context.HttpContext.ApplicationInstance.Context));

			if (exception == null || IsHttpCode(exception, HttpStatusCode.NotFound))
			{
				Logger.Info(message, exception);
			}
			else
			{
				Logger.Error(message, exception);
			}
		}

		protected string GetHttpContextInformation(HttpContext httpContext)
		{
			StringBuilder builder = new StringBuilder(1000);
			builder.Append("URL: ");
			builder.AppendLine(httpContext.Request.Url.ToString());
			builder.Append("HTTP-Method: ");
			builder.AppendLine(httpContext.Request.HttpMethod);
			builder.AppendLine("GET Parameters: ");

			foreach (string key in httpContext.Request.Unvalidated().QueryString)
			{
				builder.Append(key);
				builder.Append(": ");

				foreach (string value in httpContext.Request.Unvalidated().QueryString.GetValues(key))
				{
					builder.AppendLine(value);
					builder.AppendLine(";");
				}
			}

			builder.AppendLine();
			builder.AppendLine("POST Parameters: ");

			foreach (string key in httpContext.Request.Unvalidated().Form)
			{
				builder.Append(key);
				builder.Append(": ");

				foreach (string value in httpContext.Request.Unvalidated().Form.GetValues(key))
				{
					builder.AppendLine(value);
					builder.AppendLine(";");
				}
			}

			builder.AppendLine();

			return builder.ToString();
		}

		protected string GetRouteDataInformation(RouteData routeData)
		{
			StringBuilder builder = new StringBuilder(1000);
			builder.Append("Route: ");
			builder.AppendLine(((Route)routeData.Route).Url);
			builder.Append("Controller: ");
			builder.AppendLine(routeData.Values["controller"] != null ? routeData.Values["controller"].ToString() : "Undefined");
			builder.Append("Action: ");
			builder.AppendLine(routeData.Values["action"] != null ? routeData.Values["action"].ToString() : "Undefined");

			return builder.ToString();
		}

		protected bool IsHttpCode(Exception exception, HttpStatusCode httpStatusCode)
		{
			return (exception is HttpException) && (((HttpException)exception).GetHttpCode() == (int)httpStatusCode);
		}
	}
}