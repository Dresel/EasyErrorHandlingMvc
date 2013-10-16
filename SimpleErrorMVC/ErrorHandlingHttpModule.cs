namespace SimpleErrorMVC
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Net;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Routing;
	using System.Web.SessionState;
	using SimpleErrorMVC.Helpers;
	using SimpleErrorMVC.Logger;
	using SimpleErrorMVC.Setup;
	using SimpleErrorMVC.Wrappers;

	public class ErrorHandlingHttpModule : IHttpModule
	{
		private static readonly object InstallerLock = new object();

		private static bool installed;

		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			context.BeginRequest += MvcApplicationBeginRequest;
			context.PostRequestHandlerExecute += MvcApplicationPostRequestHandlerExecute;

			context.Error += MvcApplicationError;

			InstallOnce();
		}

		protected void Install()
		{
			// Wrap ControllerFactory
			WrapControllerFactory();

			// Add global ExceptionFilter
			GlobalFilters.Filters.Add(DependencyResolver.Current.GetService<ExceptionFilterAttribute>());

			// Set Error Routes
			RouteCollection routes = RouteTable.Routes;

			using (routes.GetWriteLock())
			{
				// Specific Error Routes, called by IIS error handling
				routes.MapRoute("Unauthorized", Configuration.UnauthorizedUrl,
					new { controller = Configuration.ErrorControllerName, action = Configuration.UnauthorizedActionName });
				routes.MapRoute("NotFound", Configuration.NotFoundUrl,
					new { controller = Configuration.ErrorControllerName, action = Configuration.NotFoundActionName });
				routes.MapRoute("InternalServerError", Configuration.InternalServerErrorUrl,
					new { controller = Configuration.ErrorControllerName, action = Configuration.InternalServerErrorActionName });

				// Default Error Route
				routes.MapRoute("NotFoundCatchAll", "{*any}",
					new { controller = Configuration.ErrorControllerName, action = Configuration.NotFoundActionName });
			}
		}

		protected void InstallOnce()
		{
			if (installed)
			{
				return;
			}

			lock (InstallerLock)
			{
				if (installed)
				{
					return;
				}

				Install();

				installed = true;
			}
		}

		protected void WrapControllerFactory()
		{
			ControllerBuilder.Current.SetControllerFactory(
				new ControllerFactoryWrapper(ControllerBuilder.Current.GetControllerFactory()));
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

			ILogger logger = DependencyResolver.Current.GetService<ILogger>() ?? new NoLogger();

			try
			{
				logger.Log("An exception was thrown and handled by ErrorHandlingHttpModule.",
					exception.CorrespondingHttpStatusCode(), LogLevel.Error, exception, httpContext);
			}
			catch
			{
				Trace.WriteLine(exception);
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
			ErrorController errorController = DependencyResolver.Current.GetService<ErrorController>();

			switch (exception.CorrespondingHttpStatusCode())
			{
				case HttpStatusCode.NotFound:
					errorController.ExecuteNotFound(httpContext);
					break;

				case HttpStatusCode.Unauthorized:
					errorController.ExecuteUnauthorized(httpContext);
					break;

				default:
					errorController.ExecuteInternalServerError(httpContext);
					break;
			}
		}
	}
}