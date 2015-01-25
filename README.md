# Nuget package

There is a nuget package avaliable here http://nuget.org/packages/EasyErrorHandlingMvc.

## Introduction

EasyErrorHandlingMvc is an MVC package (inspired by https://www.nuget.org/packages/NotFoundMvc/ and lots of blogs / articles about this topic) that simplifies error handling of MVC web applications.

SimpleErrorMVC handles all error types and calls the (configured) corresponding view:

	private static void SetRenderingConfiguration()
	{
		// Define which HttpStatusCode should be rendered for which type of error, from most to less specific
		// For defining individual rules, add them to CorrespondingRenderingHttpStatusCode directly
		Configuration.RenderDangerousParametersAs(HttpStatusCode.NotFound);
		Configuration.RenderInvalidParametersAs(HttpStatusCode.NotFound);
		Configuration.RenderHttpExceptionAs(HttpStatusCode.Forbidden, HttpStatusCode.NotFound);
		Configuration.RenderHttpExceptionAs(HttpStatusCode.NotFound, HttpStatusCode.NotFound);

		Configuration.RenderHttpExceptionAs(HttpStatusCode.InternalServerError, HttpStatusCode.InternalServerError);
		Configuration.RenderHttpExceptionAs(HttpStatusCode.Unauthorized, HttpStatusCode.Unauthorized);

		Configuration.RenderEverythingElseAs(HttpStatusCode.InternalServerError);

		Configuration.SetErrorViewPath(HttpStatusCode.InternalServerError, "~/Views/ErrorHandling/InternalServerError.cshtml");
		Configuration.SetErrorViewPath(HttpStatusCode.Unauthorized, "~/Views/ErrorHandling/Unauthorized.cshtml");
		Configuration.SetErrorViewPath(HttpStatusCode.NotFound, "~/Views/ErrorHandling/NotFound.cshtml");

		Configuration.FatalErrorFilePath = "~/Views/ErrorHandling/FatalError.htm";
	}

If something really bad happens, and the error view can't be rendered, a static file (FatalError.htm) is served.

You can use MVC Dependency Injection and ILogger Interface to hook to log information. You can also set IFallbackFileResolver if you want to modify the static file delivery process. See examples for more information.
