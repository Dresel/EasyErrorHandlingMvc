namespace SimpleErrorMVC
{
	using System;
	using System.Diagnostics;
	using System.Net;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Routing;
	using SimpleErrorMVC.Logger;
	using SimpleErrorMVC.Models;
	using SimpleErrorMVC.Setup;

	public class ErrorController : IController
	{
		public ErrorController()
		{
			Logger = new NoLogger();
			FallbackFileResolver = new StaticFallbackFileResolver();
		}

		public ErrorController(ILogger logger)
		{
			Logger = logger;
			FallbackFileResolver = new StaticFallbackFileResolver();
		}

		public ErrorController(IFallbackFileResolver fallbackFileResolver)
		{
			Logger = new NoLogger();
			FallbackFileResolver = fallbackFileResolver;
		}

		public ErrorController(ILogger logger, IFallbackFileResolver fallbackFileResolver)
		{
			Logger = logger;
			FallbackFileResolver = fallbackFileResolver;
		}

		protected IFallbackFileResolver FallbackFileResolver { get; set; }

		protected ILogger Logger { get; set; }

		public void Execute(RequestContext requestContext)
		{
			string controller = (string)requestContext.RouteData.Values["controller"] ?? Configuration.ErrorControllerName;
			string action = (string)requestContext.RouteData.Values["action"] ?? Configuration.NotFoundActionName;

			// Called by CatchAll route or IIS error handling
			if (controller == Configuration.ErrorControllerName)
			{
				if (action == Configuration.UnauthorizedActionName)
				{
					try
					{
						Logger.Log("Executing ErrorController ExecuteUnauthorized.", HttpStatusCode.InternalServerError,
							LogLevel.Information, null, requestContext);
					}
					catch
					{
						Trace.WriteLine("Executing ErrorController.");
					}

					ExecuteUnauthorized(requestContext);
					return;
				}

				if (action == Configuration.NotFoundActionName)
				{
					try
					{
						Logger.Log("Executing ErrorController ExecuteNotFound.", HttpStatusCode.InternalServerError, LogLevel.Information,
							null, requestContext);
					}
					catch
					{
						Trace.WriteLine("Executing ErrorController.");
					}

					ExecuteNotFound(requestContext);
					return;
				}

				if (action == Configuration.InternalServerErrorActionName)
				{
					try
					{
						Logger.Log("Executing ErrorController ExecuteInternalServerError.", HttpStatusCode.InternalServerError,
							LogLevel.Information, null, requestContext);
					}
					catch
					{
						Trace.WriteLine("Executing ErrorController.");
					}

					ExecuteInternalServerError(requestContext);
					return;
				}
			}

			// Only ControllerFactoryWrapper should call Execute with different controller and action name
			// When this happens, the information was already logged
			ExecuteNotFound(requestContext);
		}

		public void ExecuteInternalServerError(RequestContext requestContext)
		{
			Render(requestContext, CreateInternalServerErrorActionResult(requestContext));
		}

		public void ExecuteInternalServerError(HttpContext httpContext)
		{
			ExecuteInternalServerError(CreateInternalServerErrorRequestContext(httpContext));
		}

		public void ExecuteNotFound(RequestContext requestContext)
		{
			Render(requestContext, CreateNotFoundActionResult(requestContext));
		}

		public void ExecuteNotFound(HttpContext httpContext)
		{
			ExecuteNotFound(CreateNotFoundRequestContext(httpContext));
		}

		public void ExecuteUnauthorized(RequestContext requestContext)
		{
			Render(requestContext, CreateUnauthorizedActionResult(requestContext));
		}

		public void ExecuteUnauthorized(HttpContext httpContext)
		{
			ExecuteNotFound(CreateUnauthorizedRequestContext(httpContext));
		}

		protected ActionResult CreateActionResult(RequestContext requestContext, string viewPath, object model)
		{
			if (requestContext.HttpContext.Request.IsAjaxRequest())
			{
				// TODO: Set some Error Information?
				return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			}

			return new ViewResult { ViewName = viewPath, ViewData = new ViewDataDictionary(model) };
		}

		protected ActionResult CreateInternalServerErrorActionResult(RequestContext requestContext)
		{
			InitResponse(requestContext, (int)HttpStatusCode.InternalServerError);

			return CreateActionResult(requestContext, Configuration.InternalServerErrorViewPath, null);
		}

		protected RequestContext CreateInternalServerErrorRequestContext(HttpContext httpContext)
		{
			return new RequestContext(new HttpContextWrapper(httpContext),
				CreateRouteData(Configuration.InternalServerErrorActionName));
		}

		protected ActionResult CreateNotFoundActionResult(RequestContext requestContext)
		{
			InitResponse(requestContext, (int)HttpStatusCode.NotFound);

			NotFoundViewModel model = new NotFoundViewModel
			{
				RequestedUrl = GetRequestedUrl(requestContext.HttpContext.Request),
				ReferrerUrl = GetReferrerUrl(requestContext.HttpContext.Request)
			};

			return CreateActionResult(requestContext, Configuration.NotFoundViewPath, model);
		}

		protected RequestContext CreateNotFoundRequestContext(HttpContext httpContext)
		{
			return new RequestContext(new HttpContextWrapper(httpContext), CreateRouteData(Configuration.NotFoundActionName));
		}

		protected RouteData CreateRouteData(string action)
		{
			RouteData routeData = new RouteData();
			routeData.Values.Add("controller", Configuration.ErrorControllerName);
			routeData.Values.Add("action", action);

			return routeData;
		}

		protected ActionResult CreateUnauthorizedActionResult(RequestContext requestContext)
		{
			InitResponse(requestContext, (int)HttpStatusCode.Unauthorized);

			return CreateActionResult(requestContext, Configuration.UnauthorizedViewPath, null);
		}

		protected RequestContext CreateUnauthorizedRequestContext(HttpContext httpContext)
		{
			return new RequestContext(new HttpContextWrapper(httpContext), CreateRouteData(Configuration.UnauthorizedActionName));
		}

		protected string ExtractOriginalUrlFromExecuteUrlModeErrorRequest(Uri url)
		{
			// Expected format is "?404; http://hostname.com/some/path"
			int start = url.Query.IndexOf(';');

			if (0 <= start && start < url.Query.Length - 1)
			{
				return url.Query.Substring(start + 1);
			}
			// Unexpected format, so just return the full url!
			return url.ToString();
		}

		protected string GetReferrerUrl(HttpRequestBase request)
		{
			return ((request.UrlReferrer != null) && (request.UrlReferrer.OriginalString != request.Url.OriginalString))
				? request.UrlReferrer.OriginalString : null;
		}

		protected string GetRequestedUrl(HttpRequestBase request)
		{
			return (request.AppRelativeCurrentExecutionFilePath == string.Format("~/{0}", Configuration.NotFoundActionName))
				? ExtractOriginalUrlFromExecuteUrlModeErrorRequest(request.Url) : request.Url.OriginalString;
		}

		protected void InitResponse(RequestContext requestContext, int httpStatusCode)
		{
			requestContext.HttpContext.Response.Clear();
			requestContext.HttpContext.Response.TrySkipIisCustomErrors = true;
			requestContext.HttpContext.Response.StatusCode = httpStatusCode;
			requestContext.HttpContext.Response.ContentType = "text/HTML";
		}

		protected void Render(RequestContext requestContext, ActionResult actionResult)
		{
			try
			{
				actionResult.ExecuteResult(new ControllerContext(requestContext, new FakeController()));
			}
			catch (Exception e)
			{
				try
				{
					Logger.Log("A fatal error occured while rendering error information.", HttpStatusCode.InternalServerError,
						LogLevel.Fatal, e, requestContext);
				}
				catch
				{
					Trace.WriteLine(e);
				}

				InitResponse(requestContext, (int)HttpStatusCode.InternalServerError);

				try
				{
					string filePath = FallbackFileResolver.GetFilePath();

					requestContext.HttpContext.Response.WriteFile(filePath);
				}
				catch (Exception)
				{
					// Render minimal HTML
					requestContext.HttpContext.Response.Write(Configuration.FallbackContent);
				}
			}
		}

		public class FakeController : Controller
		{
		}
	}
}