using DiplomService.Models;
using System.ComponentModel.DataAnnotations;

namespace DiplomService.ViewModels.OrganizationViewModels
{
    public class OrganizationViewModel
    {
        public Organization Organization { get; set; } = new();

        [Required]
        private IFormFile? imageFile = null;

        [Required(ErrorMessage = "Добавьте изображение")]
        [Display(Name = "Изображение")]
        public IFormFile? ImageFile
        {
            get { return imageFile; }
            set
            {
                imageFile = value;
                if (value != null)
                {
                    using var memoryStream = new MemoryStream();
                    value.CopyTo(memoryStream);
                    Organization.Preview = memoryStream.ToArray();
                }
            }
        }
    }
}
