namespace EasyErrorHandlingMvc
{
	using System;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Routing;

	public class NullLogger : ILogger
	{
		public void Log(string message, Exception exception, ControllerContext context)
		{
		}

		public void Log(string message, Exception exception, HttpContext context)
		{
		}

		public void Log(string message, Exception exception, RequestContext context)
		{
		}
	}
}