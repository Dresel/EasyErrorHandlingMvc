namespace EasyErrorHandlingMvc.Rendering
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Net;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Routing;
	using EasyErrorHandlingMvc.Handlers;

	public class ErrorHandlingController : ControllerBase
	{
		public ErrorHandlingController()
		{
			Logger = new NullLogger();
			FallbackFileResolver = new StaticFallbackFileResolver();
		}

		public ErrorHandlingController(ILogger logger, IFallbackFileResolver fallbackFileResolver)
		{
			Logger = logger;
			FallbackFileResolver = fallbackFileResolver;
		}

		public Exception Exception { get; set; }

		protected IFallbackFileResolver FallbackFileResolver { get; set; }

		protected ILogger Logger { get; set; }

		public void Execute(Exception exception, HttpContext httpContext)
		{
			// Get the HttpStatusCode the Response should be rendered as, fallback to InternalServerError if no mapping exist
			HttpStatusCode? httpStatusCode =
				Configuration.CorrespondingRenderingHttpStatusCode.Select(x => x(exception)).FirstOrDefault(x => x != null) ??
					HttpStatusCode.InternalServerError;

			Execute(exception, httpStatusCode.Value, httpContext);
		}

		public void Execute(Exception exception, RequestContext requestContext)
		{
			// Get the HttpStatusCode the Response should be rendered as, fallback to InternalServerError if no mapping exist
			HttpStatusCode? httpStatusCode =
				Configuration.CorrespondingRenderingHttpStatusCode.Select(x => x(exception)).FirstOrDefault(x => x != null) ??
					HttpStatusCode.InternalServerError;

			Render(exception, requestContext, httpStatusCode.Value);
		}

		protected ActionResult CreateActionResult(RequestContext requestContext, string viewPath, object model)
		{
			return new ViewResult { ViewName = viewPath, ViewData = new ViewDataDictionary(model) };
		}

		protected NotFoundViewModel CreateNotFoundViewModel(RequestContext requestContext)
		{
			return new NotFoundViewModel
			{
				RequestedUrl = GetRequestedUrl(requestContext.HttpContext.Request),
				ReferrerUrl = GetReferrerUrl(requestContext.HttpContext.Request)
			};
		}

		protected RequestContext CreateRequestContext(HttpStatusCode renderedHttpStatusCode, HttpContext httpContext)
		{
			RouteData routeData = CreateRouteData(renderedHttpStatusCode);

			return new RequestContext(new HttpContextWrapper(httpContext), routeData);
		}

		protected RouteData CreateRouteData(HttpStatusCode renderedHttpStatusCode)
		{
			RouteData routeData = new RouteData();
			routeData.Values.Add("controller", Configuration.ErrorHandlingControllerName);
			routeData.Values.Add("action", renderedHttpStatusCode.ToString());

			return routeData;
		}

		protected void Execute(Exception exception, HttpStatusCode renderedHttpStatusCode, HttpContext httpContext)
		{
			Render(exception, CreateRequestContext(renderedHttpStatusCode, httpContext), renderedHttpStatusCode);
		}

		protected override void ExecuteCore()
		{
			string controller = (string)ControllerContext.RequestContext.RouteData.Values["controller"] ??
				Configuration.ErrorHandlingControllerName;
			string action = (string)ControllerContext.RequestContext.RouteData.Values["action"] ??
				HttpStatusCode.NotFound.ToString();

			// Called by CatchAll route or IIS error handling
			if (controller == Configuration.ErrorHandlingControllerName)
			{
				HttpStatusCode httpStatusCode;

				if (Enum.TryParse(action, true, out httpStatusCode))
				{
					LogExecution(((Route)ControllerContext.RequestContext.RouteData.Route).Url, httpStatusCode);
					Render(null, ControllerContext.RequestContext, httpStatusCode);

					return;
				}
			}

			// Only ControllerFactoryWrapper should call Execute with different controller and action name
			// When this happens, the information was already logged
			Execute(Exception, ControllerContext.RequestContext);
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
			return (request.AppRelativeCurrentExecutionFilePath == string.Format("~/{0}", HttpStatusCode.NotFound))
				? ExtractOriginalUrlFromExecuteUrlModeErrorRequest(request.Url) : request.Url.OriginalString;
		}

		protected void InitResponse(RequestContext requestContext, HttpStatusCode httpStatusCode,
			string contentType = "text/HTML")
		{
			requestContext.HttpContext.Response.Clear();
			requestContext.HttpContext.Response.TrySkipIisCustomErrors = true;
			requestContext.HttpContext.Response.StatusCode = (int)httpStatusCode;
			requestContext.HttpContext.Response.ContentType = contentType;
		}

		protected void LogExecution(string route, HttpStatusCode httpStatusCode)
		{
			string message =
				string.Format("[ErrorHandlingController]: Executed directly via route \"{0}\" with HttpStatusCode \"{1}\".", route,
					httpStatusCode);

			try
			{
				Logger.Log(message, null, ControllerContext.RequestContext);
			}
			catch
			{
				Trace.WriteLine(message);
			}
		}

		protected void Render(Exception exception, RequestContext requestContext, HttpStatusCode renderedHttpStatusCode)
		{
			try
			{
				if (requestContext.HttpContext.Items.Contains(Configuration.ErrorControllerRenderCalledHttpContextItemName))
				{
					throw new InvalidOperationException(
						"[ErrorHandlingController]: Recursive ErrorHandlingController Render calls detected.");
				}

				requestContext.HttpContext.Items[Configuration.ErrorControllerRenderCalledHttpContextItemName] = true;

				ActionResult actionResult;

				string errorViewPath = Configuration.ErrorViewPaths[renderedHttpStatusCode];

				if (!requestContext.HttpContext.Request.IsAjaxRequest())
				{
					InitResponse(requestContext, renderedHttpStatusCode);

					// If error should be rendered as 404 create NotFoundViewModel
					if (renderedHttpStatusCode == HttpStatusCode.NotFound)
					{
						actionResult = CreateActionResult(requestContext, errorViewPath, CreateNotFoundViewModel(requestContext));
					}
					else
					{
						actionResult = CreateActionResult(requestContext, errorViewPath, null);
					}
				}
				else
				{
					InitResponse(requestContext, renderedHttpStatusCode, "application/json");

					string message = exception != null && !requestContext.HttpContext.IsCustomErrorEnabled
						? exception.ToString() : string.Empty;

					actionResult = new JsonResult
					{
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						Data =
							new
							{
								httpStatusCode = renderedHttpStatusCode,
								httpStatusMessage = renderedHttpStatusCode.ToString(),
								message = message
							}
					};
				}

				actionResult.ExecuteResult(new ControllerContext(requestContext, new ErrorHandlingController()));
			}
			catch (Exception e)
			{
				string message = "[ErrorHandlingController]: A fatal error occured while rendering error information.";

				try
				{
					Logger.Log(message, e, requestContext);
				}
				catch
				{
					Trace.WriteLine(message);
				}

				// If action is child action rethrow
				if (requestContext.RouteData.IsChildAction())
				{
					throw;
				}

				InitResponse(requestContext, HttpStatusCode.InternalServerError);

				try
				{
					string filePath = FallbackFileResolver.GetFilePath();

					requestContext.HttpContext.Response.WriteFile(filePath);
				}
				catch (Exception)
				{
					// Render minimal static HTML
					requestContext.HttpContext.Response.Write(Configuration.FatalErrorFallbackContent);
				}
			}
		}
	}
}