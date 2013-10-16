using System.Web;
using SimpleErrorMVC.Setup;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStarter), "StartOnce")]

namespace SimpleErrorMVC.Setup
{
	using Microsoft.Web.Infrastructure.DynamicModuleHelper;

	public class PreApplicationStarter
	{
		private static bool started;

		public static void StartOnce()
		{
			if (started)
			{
				return;
			}

			started = true;

			DynamicModuleUtility.RegisterModule(typeof(ErrorHandlingHttpModule));
		}
	}
}