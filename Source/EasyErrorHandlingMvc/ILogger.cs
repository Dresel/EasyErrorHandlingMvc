namespace EasyErrorHandlingMvc
{
	using System;
	using System.Web;
	using System.Web.Routing;

	public interface ILogger
	{
		void Log(string message, Exception exception, HttpContext context);

		void Log(string message, Exception exception, RequestContext context);
	}
}