using DiplomService.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomService.ViewModels.News
{
    public class SectionViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Заголовок")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Содержание")]
        public string Description { get; set; } = string.Empty;
        public byte[]? Image { get; set; }
        [NotMapped]
        public string? MimeType { get { return ImageWorker.GetImageMimeType(Image); } }
        public bool ToDelete { get; set; } = false;
        private IFormFile? imageFile;
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
                    Image = memoryStream.ToArray();
                }
            }
        }
    }
}
