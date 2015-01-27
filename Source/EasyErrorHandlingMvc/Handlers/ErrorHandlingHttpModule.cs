namespace EasyErrorHandlingMvc.Handlers
{
	using System;
	using System.Diagnostics;
	using System.Web;
	using System.Web.Mvc;
	using EasyErrorHandlingMvc.Rendering;

	public class ErrorHandlingHttpModule : IHttpModule
	{
		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
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

			// Create a custom ErrorPage
			httpContext.ClearError();
			httpContext.Response.TrySkipIisCustomErrors = true;

			ErrorHandlingController errorHandlingController = DependencyResolver.Current.GetService<ErrorHandlingController>();
			errorHandlingController.Execute(exception, httpContext);
		}
	}
}