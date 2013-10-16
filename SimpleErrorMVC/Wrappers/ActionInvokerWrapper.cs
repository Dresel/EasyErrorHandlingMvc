namespace SimpleErrorMVC.Wrappers
{
	using System;
	using System.Diagnostics;
	using System.Net;
	using System.Web.Mvc;
	using SimpleErrorMVC.Logger;

	public class ActionInvokerWrapper : IActionInvoker
	{
		public ActionInvokerWrapper(IActionInvoker actionInvoker, ILogger logger)
		{
			ActionInvoker = actionInvoker;
			Logger = logger;
		}

		protected IActionInvoker ActionInvoker { get; set; }

		protected ILogger Logger { get; set; }

		public bool InvokeAction(ControllerContext controllerContext, string actionName)
		{
			if (ActionInvoker.InvokeAction(controllerContext, actionName))
			{
				return true;
			}

			try
			{
				Logger.Log(
					string.Format("Action {0} for Controller {1} not found and handled by ActionInvokerWrapper.", actionName,
						controllerContext.Controller.GetType().Name), HttpStatusCode.NotFound, LogLevel.Information, null,
					controllerContext);
			}
			catch (Exception)
			{
				Trace.WriteLine(string.Format("Action {0} for Controller {1} not found and handled by ActionInvokerWrapper.",
					actionName, controllerContext.Controller.GetType().Name));
			}

			if (!controllerContext.HttpContext.IsCustomErrorEnabled)
			{
				return false;
			}

			ErrorController controller = DependencyResolver.Current.GetService<ErrorController>();
			controller.ExecuteNotFound(controllerContext.RequestContext);

			return true;
		}
	}
}