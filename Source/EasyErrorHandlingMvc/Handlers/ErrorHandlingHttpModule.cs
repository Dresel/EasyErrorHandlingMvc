namespace EasyErrorHandlingMvc.Handlers
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.SessionState;
	using EasyErrorHandlingMvc.Rendering;

	public class ErrorHandlingHttpModule : IHttpModule
	{
		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			context.BeginRequest += MvcApplicationBeginRequest;
			context.PostRequestHandlerExecute += MvcApplicationPostRequestHandlerExecute;

			context.Error += MvcApplicationError;
		}

		protected void LogException(HttpContext httpContext, Exception exception)
		{
			ILogger logger = DependencyResolver.Current.GetService<ILogger>() ?? new NullLogger();

			string message = "[ErrorHandlingHttpModule]: An exception was thrown.";

			try
			{
				logger.Log(message, exception, httpContext);
			}
			catch
			{
				Trace.WriteLine(message);
			}
		}

		protected void MvcApplicationBeginRequest(object sender, EventArgs e)
		{
			HttpContext httpContext = ((HttpApplication)sender).Context;

			// Error pages may need Session, which some HTTPHandler may not request
			httpContext.SetSessionStateBehavior(SessionStateBehavior.Required);
		}

		protected void MvcApplicationError(object sender, EventArgs e)
		{
			HttpContext httpContext = ((HttpApplication)sender).Context;
			Exception exception = httpContext.Error;

			if (exception == null)
			{
				return;
			}

			if (!httpContext.Items.Contains(Configuration.EasyErrorHandlingLoggedHttpContextItemName))
			{
				httpContext.Items[Configuration.EasyErrorHandlingLoggedHttpContextItemName] = true;
				LogException(httpContext, exception);
			}

			if (!httpContext.IsCustomErrorEnabled)
			{
				return;
			}

			// Clear Errors
			httpContext.ClearError();

			// Save RenderCustomErrorPage for rendering in PostMapRequestHandler
			if (!httpContext.Items.Contains("RenderCustomErrorPage"))
			{
				httpContext.Items.Add("RenderCustomErrorPage", true);
			}

			httpContext.Response.TrySkipIisCustomErrors = true;
		}

		protected void MvcApplicationPostRequestHandlerExecute(object sender, EventArgs e)
		{
			// Render error page if necessary after acquire request state, so Session is available
			HttpContext httpContext = ((HttpApplication)sender).Context;

			if (!httpContext.Items.Contains("RenderCustomErrorPage") || httpContext.AllErrors == null)
			{
				return;
			}

			Exception exception = httpContext.AllErrors.Last();

			// Custom ErrorPage
			ErrorHandlingController errorHandlingController = DependencyResolver.Current.GetService<ErrorHandlingController>();
			errorHandlingController.Execute(exception, httpContext);
		}
	}
}