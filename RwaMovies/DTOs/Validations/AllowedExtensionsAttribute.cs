using System.ComponentModel.DataAnnotations;

namespace RwaMovies.DTOs.Validations
{
	public class AllowedExtensionsAttribute : ValidationAttribute
	{
		private readonly string[] _extensions;

		public AllowedExtensionsAttribute(string[] extensions)
		{
			_extensions = extensions;
		}

		protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
		{
			var file = value as IFormFile;
			if (file != null)
			{
				var extension = Path.GetExtension(file.FileName);
				if (!_extensions.Contains(extension.ToLower()))
				{
					return new ValidationResult(GetErrorMessage());
				}
			}
			return ValidationResult.Success!;
		}

		public string GetErrorMessage()
		{
			return $"Allowed extensions are: {string.Join(", ", _extensions)}";
		}
	}
}
