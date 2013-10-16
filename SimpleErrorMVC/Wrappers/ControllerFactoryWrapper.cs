namespace SimpleErrorMVC.Wrappers
{
	using System;
	using System.Diagnostics;
	using System.Net;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Routing;
	using System.Web.SessionState;
	using SimpleErrorMVC.Logger;

	public class ControllerFactoryWrapper : IControllerFactory
	{
		public ControllerFactoryWrapper(IControllerFactory controllerFactory)
		{
			ControllerFactory = controllerFactory;
		}

		private IControllerFactory ControllerFactory { get; set; }

		private ILogger Logger
		{
			get
			{
				// We can't inject this via constructor because IControllerFactory can't be used in RequestContext
				return DependencyResolver.Current.GetService<ILogger>() ?? new NoLogger();
			}
		}

		public IController CreateController(RequestContext requestContext, string controllerName)
		{
			try
			{
				IController controller = ControllerFactory.CreateController(requestContext, controllerName);

				WrapControllerActionInvoker(controller);

				return controller;
			}
			catch (Exception e)
			{
				try
				{
					Logger.Log(string.Format("Controller {0} not found and handled by ControllerFactoryWrapper.", controllerName),
						HttpStatusCode.NotFound, LogLevel.Information, e, requestContext);
				}
				catch
				{
					Trace.WriteLine(e);
				}

				if (!requestContext.HttpContext.IsCustomErrorEnabled)
				{
					throw;
				}

				// If its not an Http404 Exception, rethrow and let it be handled in global.asax
				if (!(e is HttpException && ((HttpException)e).GetHttpCode() == (int)HttpStatusCode.NotFound))
				{
					throw;
				}

				return DependencyResolver.Current.GetService<ErrorController>();
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

		private void WrapControllerActionInvoker(IController controller)
		{
			Controller controllerWithInvoker = controller as Controller;

			if (controllerWithInvoker != null)
			{
				controllerWithInvoker.ActionInvoker = new ActionInvokerWrapper(controllerWithInvoker.ActionInvoker, Logger);
			}
		}
	}
}