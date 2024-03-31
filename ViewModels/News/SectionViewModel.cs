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
        public string? MimeType { get { return GetImageMimeType(); } }
        private string GetImageMimeType()
        {
            if (Image == null) return "application/octet-stream";
            if (Image.Length < 4)
            {
                return "application/octet-stream"; // По умолчанию, если массив слишком короткий для определения
            }

            if (Image[0] == 0xFF && Image[1] == 0xD8 && Image[2] == 0xFF)
            {
                return "image/jpeg";
            }
            else if (Image[0] == 0x89 && Image[1] == 0x50 && Image[2] == 0x4E && Image[3] == 0x47)
            {
                return "image/png";
            }

            return "application/octet-stream"; // Если формат неизвестен, вернуть по умолчанию
        }
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
