namespace SimpleErrorMVC.WebSampleExtended.Extension
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Net;
	using System.Text;
	using System.Web;
	using System.Web.Helpers;
	using System.Web.Mvc;
	using System.Web.Routing;
	using SimpleErrorMVC.Logger;

	public class HttpContextLogger : ILogger
	{
		public void Log(string message, HttpStatusCode renderedStatusCode, LogLevel level, Exception exception,
			ControllerContext context)
		{
			LogToHttpContext(context.HttpContext.ApplicationInstance.Context,
				string.Format(
					"Message: {0}\nRenderHttpStatusCode: {1}\nLevel: {2}\nException: {3}\n\nAdditionalInformation\n{4}\n\n{5}", message,
					renderedStatusCode, level, exception != null ? GetExceptionInformation(exception) : "-",
					GetControllerContextInformation(context),
					GetHttpContextInformation(context.HttpContext.ApplicationInstance.Context)));
		}

		public void Log(string message, HttpStatusCode renderedStatusCode, LogLevel level, Exception exception,
			HttpContext context)
		{
			LogToHttpContext(context,
				string.Format(
					"Message: {0}\nRenderHttpStatusCode: {1}\nLevel: {2}\nException: {3}\n\nAdditionalInformation\n{4}\n\n{5}", message,
					renderedStatusCode, level, exception != null ? GetExceptionInformation(exception) : "-", "-",
					GetHttpContextInformation(context)));
		}

		public void Log(string message, HttpStatusCode renderedStatusCode, LogLevel level, Exception exception,
			RequestContext context)
		{
			LogToHttpContext(context.HttpContext.ApplicationInstance.Context,
				string.Format(
					"Message: {0}\nRenderHttpStatusCode: {1}\nLevel: {2}\nException: {3}\n\nAdditionalInformation\n{4}\n\n{5}", message,
					renderedStatusCode, level, exception != null ? GetExceptionInformation(exception) : "-",
					GetRouteDataInformation(context.RouteData),
					GetHttpContextInformation(context.HttpContext.ApplicationInstance.Context)));
		}

		protected string GetControllerContextInformation(ControllerContext controllerContext)
		{
			StringBuilder builder = new StringBuilder(1000);

			if (controllerContext.ParentActionViewContext != null)
			{
				builder.AppendLine("Parent");
				builder.AppendLine(GetRouteDataInformation(controllerContext.ParentActionViewContext.RouteData));
			}

			builder.Append(GetRouteDataInformation(controllerContext.RouteData));

			return builder.ToString();
		}

		protected string GetExceptionInformation(Exception exception)
		{
			StringBuilder builder = new StringBuilder(1000);

			builder.Append("ExceptionType: ");
			builder.AppendLine(exception.GetType().ToString());

			builder.Append("ExceptionMessage: ");
			builder.AppendLine(exception.Message);

			// Handle inner exceptions
			Exception innerException = exception;

			while ((innerException = innerException.InnerException) != null)
			{
				builder.AppendLine();
				builder.AppendLine("InnerException");

				builder.Append("ExceptionType: ");
				builder.AppendLine(innerException.GetType().ToString());

				builder.Append("ExceptionMessage: ");
				builder.AppendLine(innerException.Message);
			}

			return builder.ToString();
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

		protected string GetStackFrameInformation()
		{
			StackFrame stackFrame;
			int index = 1;

			while (true)
			{
				stackFrame = new StackFrame(index, true);

				if (stackFrame.GetMethod() == null)
				{
					return "Undefined";
				}

				if (stackFrame.GetMethod().ReflectedType != typeof(ILogger))
				{
					break;
				}

				index++;
			}

			return stackFrame.GetMethod().ReflectedType.Name + "::" + stackFrame.GetMethod().Name + " (" +
				stackFrame.GetFileName() + " at Line " + stackFrame.GetFileLineNumber() + ")";
		}

		protected void LogToHttpContext(HttpContext context, string message)
		{
			if (context.Items["loginfo"] == null)
			{
				context.Items["loginfo"] = new List<string>();
			}

			((List<string>)context.Items["loginfo"]).Add(message);
		}
	}
}