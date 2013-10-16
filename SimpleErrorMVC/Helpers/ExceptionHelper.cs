namespace SimpleErrorMVC.Helpers
{
	using System;
	using System.Net;
	using System.Web;
	using System.Web.Mvc;

	public static class ExceptionHelper
	{
		public static HttpStatusCode CorrespondingHttpStatusCode(this Exception e)
		{
			// If its Http Forbidden (403), NotFound (404) or action with invalid or dangerous values
			if (e.IsHttpNotFoundException() || e.IsHttpForbiddenException() || e.IsInvalidActionParameterException() ||
				e.IsDangerousActionParameterException())
			{
				// Render as Http NotFound
				return HttpStatusCode.NotFound;
			}

			if (e.IsHttpUnauthorizedException())
			{
				// Render as Http Unauthorized
				return HttpStatusCode.Unauthorized;
			}

			// Render as Http InternalServerError
			return HttpStatusCode.InternalServerError;
		}

		public static bool IsDangerousActionParameterException(this Exception e)
		{
			return e is HttpRequestValidationException;
		}

		private static bool IsHttpForbiddenException(this Exception e)
		{
			return (e is HttpException) && (((HttpException)e).GetHttpCode() == (int)HttpStatusCode.Forbidden);
		}

		private static bool IsHttpNotFoundException(this Exception e)
		{
			return (e is HttpException) && (((HttpException)e).GetHttpCode() == (int)HttpStatusCode.NotFound);
		}

		private static bool IsHttpUnauthorizedException(this Exception e)
		{
			return (e is HttpException) && (((HttpException)e).GetHttpCode() == (int)HttpStatusCode.Unauthorized);
		}

		private static bool IsInvalidActionParameterException(this Exception e)
		{
			return (e is ArgumentException) && (e.TargetSite.DeclaringType == typeof(ActionDescriptor));
		}
	}
}