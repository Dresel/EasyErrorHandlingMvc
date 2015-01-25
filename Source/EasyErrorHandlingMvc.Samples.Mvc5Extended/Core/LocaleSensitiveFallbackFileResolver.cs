namespace EasyErrorHandlingMvc.Samples.Mvc5Extended.Core
{
	using System;
	using System.Globalization;
	using System.Linq;
	using System.Web;
	using EasyErrorHandlingMvc.Rendering;

	public class LocaleSensitiveFallbackFileResolver : IFallbackFileResolver
	{
		public string GetFilePath()
		{
			string[] acceptedCultures = { "en", "de" };

			// Defaultlanguage
			string cultureName = acceptedCultures.First();

			if (HttpContext.Current.Request.UserLanguages != null)
			{
				// Get language from HTTP Header
				foreach (string userLanguage in HttpContext.Current.Request.UserLanguages)
				{
					try
					{
						CultureInfo userCultureInfo = new CultureInfo(userLanguage);
						if (!acceptedCultures.Contains(userCultureInfo.TwoLetterISOLanguageName))
						{
							continue;
						}

						// Culture found that is supported
						cultureName = userCultureInfo.TwoLetterISOLanguageName;
						break;
					}
					catch
					{
						// Ignore invalid cultures
						continue;
					}
				}
			}

			switch (cultureName)
			{
				case "en":
					return "~/Views/ErrorHandling/FatalErrorEN.htm";
				case "de":
					return "~/Views/ErrorHandling/FatalErrorDE.htm";
				default:
					throw new InvalidOperationException();
			}
		}
	}
}