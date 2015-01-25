using System.Web;
using EasyErrorHandlingMvc;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStarter), "StartOnce")]

namespace EasyErrorHandlingMvc
{
	using EasyErrorHandlingMvc.Handlers;
	using Microsoft.Web.Infrastructure.DynamicModuleHelper;

	public class PreApplicationStarter
	{
		private static bool started;

		public static void StartOnce()
		{
			if (PreApplicationStarter.started)
			{
				return;
			}

			PreApplicationStarter.started = true;

			DynamicModuleUtility.RegisterModule(typeof(ErrorHandlingHttpModule));
		}
	}
}