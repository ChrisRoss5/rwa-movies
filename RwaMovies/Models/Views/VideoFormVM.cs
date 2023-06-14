using RwaMovies.Models.Validations;
using RwaMovies.Services;
using System.ComponentModel.DataAnnotations;
using RwaMovies.Models.Shared;

namespace RwaMovies.Models.Views
{
    public class VideoFormVM
    {
        public VideoRequest VideoRequest { get; set; } = null!;
        [Display(Name = "Image")]
        [DataType(DataType.Upload)]
        [MaxFileSize(IImagesService.MaximumFileSize)]
        [AllowedExtensions(new string[] { ".png" })]
        /* The given database doesn't have a column for the image file extension, so I'm just hardcoding it */
        public IFormFile? ImageFile { get; set; }
    }
}
