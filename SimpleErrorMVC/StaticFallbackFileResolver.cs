namespace SimpleErrorMVC
{
	using SimpleErrorMVC.Setup;

	public class StaticFallbackFileResolver : IFallbackFileResolver
	{
		public string GetFilePath()
		{
			return Configuration.FatalErrorFilePath;
		}
	}
}