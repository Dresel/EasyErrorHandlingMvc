namespace SimpleErrorMVC.Logger
{
	using System;
	using System.Net;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Routing;

	public class NoLogger : ILogger
	{
		public void Log(string message, HttpStatusCode renderedStatusCode, LogLevel level, Exception exception,
			ControllerContext context)
		{
		}

		public void Log(string message, HttpStatusCode renderedStatusCode, LogLevel level, Exception exception,
			HttpContext context)
		{
		}

		public void Log(string message, HttpStatusCode renderedStatusCode, LogLevel level, Exception exception,
			RequestContext context)
		{
		}
	}
}