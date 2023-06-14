using RwaMovies.SharedModels;
using RwaMovies.Validations;
using RwaMovies.Services;
using System.ComponentModel.DataAnnotations;

namespace RwaMovies.ViewModels
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
