using DiplomService.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomService.ViewModels.News
{
    public class NewsViewModel
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public byte[]? PriviewImage { get; set; }
        [Display(Name = "Заголовок")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Содержание")]
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string EventName { get; set; }=string.Empty;

        public List<SectionViewModel> Sections { get; set; } = new();

        [NotMapped]
        public string? MimeType { get { return GetImageMimeType(); } }
        private string GetImageMimeType()
        {
            if (PriviewImage == null) return "application/octet-stream";
            if (PriviewImage.Length < 4)
            {
                return "application/octet-stream"; // По умолчанию, если массив слишком короткий для определения
            }

            if (PriviewImage[0] == 0xFF && PriviewImage[1] == 0xD8 && PriviewImage[2] == 0xFF)
            {
                return "image/jpeg";
            }
            else if (PriviewImage[0] == 0x89 && PriviewImage[1] == 0x50 && PriviewImage[2] == 0x4E && PriviewImage[3] == 0x47)
            {
                return "image/png";
            }
            // Добавьте другие проверки для других форматов, если это необходимо

            return "application/octet-stream"; // Если формат неизвестен, вернуть по умолчанию
        }

        private IFormFile? imageFile;

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
                    PriviewImage = memoryStream.ToArray();
                }
            }
        }
    }
}
