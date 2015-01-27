namespace EasyErrorHandlingMvc.Rendering
{
	using System.ComponentModel;

	public class NotFoundViewModel
	{
		[DisplayName("Referrer Url")]
		public string ReferrerUrl { get; set; }

		[DisplayName("Requested Url")]
		public string RequestedUrl { get; set; }
	}
}