namespace EasyErrorHandlingMvc
{
	using System;
	using System.Web;
	using System.Web.Routing;

	public class NullLogger : ILogger
	{
		public void Log(string message, Exception exception, HttpContext context)
		{
		}

		public void Log(string message, Exception exception, RequestContext context)
		{
		}
	}
}