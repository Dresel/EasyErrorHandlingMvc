// See https://github.com/roblevine/UnityLog4NetExtension

namespace EasyErrorHandlingMvc.Samples.Mvc5Extended.Core.Unity
{
	using System;
	using System.Diagnostics;
	using System.Web.Mvc;
	using Microsoft.Practices.ObjectBuilder2;
	using NLog;

	public class EasyErrorHandlingNLogAdapterStrategy : BuilderStrategy
	{
		public override void PreBuildUp(IBuilderContext context)
		{
			ICreationStackTrackerPolicy policy = context.Policies.Get<ICreationStackTrackerPolicy>(buildKey: null,
				localOnly: true);

			if (policy.TypeStack.Count >= 3 && policy.TypeStack.Peek(0) == typeof(Logger) &&
				policy.TypeStack.Peek(1) == typeof(NLogAdapter))
			{
				// Injections of Logger to class NLogAdapter
				context.Existing = LogManager.GetLogger(policy.TypeStack.Peek(2).FullName);
			}
			else if (policy.TypeStack.Count >= 2 && policy.TypeStack.Peek(0) == typeof(Logger) &&
				policy.TypeStack.Peek(1) == typeof(NLogAdapter))
			{
				// Injections of Logger to class NLogAdapter, try to detect DepedencyResolver.Current.Resolve (Service Locator Anti-Pattern) calls
				StackTrace stackTrace = new StackTrace();

				for (int i = 0; i < stackTrace.FrameCount - 1; i++)
				{
					if (stackTrace.GetFrame(i).GetMethod().DeclaringType == typeof(DependencyResolverExtensions))
					{
						Type declaringType = stackTrace.GetFrame(i + 1).GetMethod().DeclaringType;

						if (declaringType != null)
						{
							context.Existing = LogManager.GetLogger(declaringType.FullName);
						}
					}
				}
			}
			else if (policy.TypeStack.Count >= 2 && policy.TypeStack.Peek(0) == typeof(Logger))
			{
				// Injections of Logger to classes other than NLogAdapter
				context.Existing = LogManager.GetLogger(policy.TypeStack.Peek(1).FullName);
			}

			base.PreBuildUp(context);
		}
	}
}