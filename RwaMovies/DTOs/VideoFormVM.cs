using RwaMovies.DTOs.Validations;
using RwaMovies.Services;
using System.ComponentModel.DataAnnotations;

namespace RwaMovies.DTOs;

public partial class VideoFormVM
{
	public VideoRequest VideoRequest { get; set; } = null!;

	[Display(Name = "Image")]
	[DataType(DataType.Upload)]
	[MaxFileSize(IImagesService.MaximumFileSize)]
	[AllowedExtensions(new string[] { ".png" })]
	public IFormFile? ImageFile { get; set; }
}
