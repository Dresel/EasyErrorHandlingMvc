namespace EasyErrorHandlingMvc.Handlers
{
	using System;
	using System.Diagnostics;
	using System.Web.Mvc;
	using System.Web.Mvc.Async;
	using System.Web.Routing;
	using System.Web.SessionState;
	using EasyErrorHandlingMvc.Rendering;

	public class ControllerFactoryWrapper : IControllerFactory
	{
		public ControllerFactoryWrapper(IControllerFactory controllerFactory)
		{
			ControllerFactory = controllerFactory;
		}

		protected ILogger Logger
		{
			get
			{
				// We can't inject this via constructor because IControllerFactory can't be used in RequestContext
				return DependencyResolver.Current.GetService<ILogger>() ?? new NullLogger();
			}
		}

		protected IControllerFactory ControllerFactory { get; set; }

		public IController CreateController(RequestContext requestContext, string controllerName)
		{
			try
			{
				IController controller = ControllerFactory.CreateController(requestContext, controllerName);

				WrapControllerActionInvoker(controller);

				return controller;
			}
			catch (Exception exception)
			{
				if (!requestContext.HttpContext.Items.Contains(Configuration.EasyErrorHandlingLoggedHttpContextItemName))
				{
					requestContext.HttpContext.Items[Configuration.EasyErrorHandlingLoggedHttpContextItemName] = true;
					LogException(requestContext, controllerName, exception);
				}

				// If CustomErrors are disabled or action is child action rethrow
				if (!requestContext.HttpContext.IsCustomErrorEnabled || requestContext.RouteData.IsChildAction())
				{
					throw;
				}

				// Return ErrorHandlingController, set Exception so ErrorHandlingController can choose which HttpStatusCode should be rendered
				ErrorHandlingController errorHandlingController = DependencyResolver.Current.GetService<ErrorHandlingController>();
				errorHandlingController.Exception = exception;

				return errorHandlingController;
			}
		}

		public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
		{
			return ControllerFactory.GetControllerSessionBehavior(requestContext, controllerName);
		}

		public void ReleaseController(IController controller)
		{
			ControllerFactory.ReleaseController(controller);
		}

		protected void WrapControllerActionInvoker(IController controller)
		{
			Controller controllerWithInvoker = controller as Controller;

			if (controllerWithInvoker != null)
			{
				if (controllerWithInvoker.ActionInvoker is IAsyncActionInvoker)
				{
					// Should be async by default
					controllerWithInvoker.ActionInvoker =
						new AsyncActionInvokerWrapper((IAsyncActionInvoker)controllerWithInvoker.ActionInvoker, Logger);
				}
				else
				{
					controllerWithInvoker.ActionInvoker = new ActionInvokerWrapper(controllerWithInvoker.ActionInvoker, Logger);
				}
			}
		}

		private void LogException(RequestContext requestContext, string controllerName, Exception exception)
		{
			string message = string.Format("[ControllerFactoryWrapper]: Controller \"{0}\" not found.", controllerName);

			try
			{
				Logger.Log(message, exception, requestContext);
			}
			catch
			{
				Trace.WriteLine(message);
			}
		}
	}
}