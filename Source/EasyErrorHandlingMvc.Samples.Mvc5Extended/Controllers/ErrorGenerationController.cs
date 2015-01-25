namespace EasyErrorHandlingMvc.Samples.Mvc5Extended.Controllers
{
	using System;
	using System.IO;
	using System.Net;
	using System.Web;
	using System.Web.Mvc;

	public class ErrorGenerationController : Controller
	{
		[Route("~/AttributeRouteUnhandledException")]
		public virtual ActionResult AttributeRouteUnhandledException()
		{
			throw new Exception("This exception was deliberately thrown by a GET action method.");
		}

		[HttpGet]
		public virtual ActionResult ChildException()
		{
			return View();
		}

		[HttpGet]
		public virtual ActionResult FailedFileDownload()
		{
			return new FileStreamResult(new UnreadableStream(), "image/jpeg");
		}

		[HttpGet]
		public virtual ActionResult NotFoundException()
		{
			throw new HttpException((int)HttpStatusCode.NotFound,
				"This 404 exception was deliberately thrown from an action method.");
		}

		[HttpGet]
		public virtual ActionResult NotFoundExceptionAjax()
		{
			throw new HttpException((int)HttpStatusCode.NotFound,
				"This 404 exception was deliberately thrown from an AJAX action method.");
		}

		[HttpGet]
		public virtual ActionResult UnauthorizedException()
		{
			throw new HttpException((int)HttpStatusCode.Unauthorized,
				"This 401 exception was deliberately thrown from an action method.");
		}

		[HttpGet]
		public virtual ActionResult UnhandledException()
		{
			throw new Exception("This exception was deliberately thrown by a GET action method.");
		}

		[HttpGet]
		public virtual ActionResult UnhandledExceptionAjax()
		{
			throw new Exception("This exception was deliberately thrown by an AJAX action method.");
		}

		[HttpPost]
		public virtual ActionResult UnhandledExceptionPost()
		{
			throw new Exception("This exception was deliberately thrown by a POST action method.");
		}

		[HttpGet]
		public virtual ActionResult UnhandledExceptionWithValidationError(int inputText)
		{
			return null;
		}

		private class UnreadableStream : Stream
		{
			public override bool CanRead
			{
				get
				{
					return false;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return false;
				}
			}

			public override long Length
			{
				get
				{
					return 1;
				}
			}

			public override long Position { get; set; }

			public override void Flush()
			{
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				throw new Exception("This exception was deliberately thrown from within a FileStreamResult.");
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				return offset;
			}

			public override void SetLength(long value)
			{
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
			}
		}
	}
}