namespace EasyErrorHandlingMvc.Handlers
{
	using System;
	using System.Web.Mvc;
	using System.Web.Mvc.Async;

	public class AsyncActionInvokerWrapper : ActionInvokerWrapper, IAsyncActionInvoker
	{
		public AsyncActionInvokerWrapper()
		{
		}

		public AsyncActionInvokerWrapper(ILogger logger)
			: base(logger)
		{
		}

		protected string ActionName { get; set; }

		protected ControllerContext ControllerContext { get; set; }

		public IAsyncResult BeginInvokeAction(ControllerContext controllerContext, string actionName, AsyncCallback callback,
			object state)
		{
			// Save as Logging Information for EndInvokeAction
			ControllerContext = controllerContext;
			ActionName = actionName;

			return ((IAsyncActionInvoker)ActionInvoker).BeginInvokeAction(controllerContext, actionName, callback, state);
		}

		public bool EndInvokeAction(IAsyncResult asyncResult)
		{
			try
			{
				if (((IAsyncActionInvoker)ActionInvoker).EndInvokeAction(asyncResult))
				{
					return true;
				}
			}
			catch (Exception exception)
			{
				return HandleActionInvokingException(ControllerContext, ActionName, exception);
			}

			return HandleNotFound(ControllerContext, ActionName);
		}
	}
}