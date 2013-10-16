namespace SimpleErrorMVC
{
	using System;
	using System.Diagnostics;
	using System.Net;
	using System.Web.Mvc;
	using SimpleErrorMVC.Helpers;
	using SimpleErrorMVC.Logger;

	public class ExceptionFilterAttribute : FilterAttribute, IExceptionFilter
	{
		public ExceptionFilterAttribute()
		{
			ErrorController = DependencyResolver.Current.GetService<ErrorController>();
			Logger = new NoLogger();
		}

		public ExceptionFilterAttribute(ILogger logger)
		{
			ErrorController = DependencyResolver.Current.GetService<ErrorController>();
			Logger = logger;
		}

		protected ErrorController ErrorController { get; set; }

		protected ILogger Logger { get; set; }

		public void OnException(ExceptionContext filterContext)
		{
			if (filterContext == null)
			{
				throw new ArgumentNullException();
			}

			// If action is childaction or exception is handled or thrown during while rendering by ErrorController,
			// return to prevent recursive ErrorController calls
			if (filterContext.ExceptionHandled || filterContext.IsChildAction || filterContext.ParentRouteDataIsErrorController())
			{
				return;
			}

			HandleException(filterContext);
		}

		protected void HandleException(ExceptionContext filterContext)
		{
			HttpStatusCode correspondingHttpStatusCode = filterContext.Exception.CorrespondingHttpStatusCode();

			try
			{
				Logger.Log("An exception was thrown and handled by ExceptionFilterAttribute.", correspondingHttpStatusCode,
					LogLevel.Error, filterContext.Exception, filterContext.RequestContext);
			}
			catch
			{
				Trace.WriteLine(filterContext.Exception);
			}

			if (!filterContext.HttpContext.IsCustomErrorEnabled)
			{
				return;
			}

			filterContext.ExceptionHandled = true;

			switch (correspondingHttpStatusCode)
			{
				case HttpStatusCode.NotFound:
					ErrorController.ExecuteNotFound(filterContext.RequestContext);
					break;

				case HttpStatusCode.Unauthorized:
					ErrorController.ExecuteUnauthorized(filterContext.RequestContext);
					break;

				default:
					ErrorController.ExecuteInternalServerError(filterContext.RequestContext);
					break;
			}
		}
	}
}