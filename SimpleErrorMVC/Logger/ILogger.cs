namespace SimpleErrorMVC.Logger
{
	using System;
	using System.Net;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Routing;

	public interface ILogger
	{
		void Log(string message, HttpStatusCode renderedStatusCode, LogLevel level, Exception exception,
			ControllerContext context);

		void Log(string message, HttpStatusCode renderedStatusCode, LogLevel level, Exception exception, HttpContext context);

		void Log(string message, HttpStatusCode renderedStatusCode, LogLevel level, Exception exception,
			RequestContext context);
	}
}