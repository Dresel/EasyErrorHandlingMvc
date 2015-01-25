namespace EasyErrorHandlingMvc.Rendering
{
	using EasyErrorHandlingMvc;

	public class StaticFallbackFileResolver : IFallbackFileResolver
	{
		public string GetFilePath()
		{
			return Configuration.FatalErrorFilePath;
		}
	}
}