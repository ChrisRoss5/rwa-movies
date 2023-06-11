using Microsoft.AspNetCore.Mvc;
using RwaMovies.Models;
using RwaMovies.Services;

namespace RwaMovies.Controllers
{
	public class ImagesController : Controller
	{
		private readonly IImagesService _imagesService;

		public ImagesController(IImagesService imagesService)
		{
			_imagesService = imagesService;
		}

		public async Task<IActionResult> Image(int id)
		{
			try
			{
				return await _imagesService.GetImage(id);
			}
			catch
			{
				return NotFound();
			}
		}
	}
}
