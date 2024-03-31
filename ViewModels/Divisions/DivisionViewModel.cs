using DiplomService.ViewModels.Measures;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomService.ViewModels.Divisions
{
    public class DivisionViewModel
    {

        public int Id { get; set; }
        public int EventId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [MaxLength(60)]

        public string Name { get; set; } = "";
        public string? Description { get; set; } = "";

        [Range(-180, 180)]
        public double? Longitude { get; set; }
        [Range(-90, 90)]
        public double? Latitude { get; set; }
        public string PlaceName { get; set; } = "";

        public List<MeasureViewModel> Measures { get; set; } = new();


        public byte[]? PriviewImage { get; set; } = null;
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
        private IFormFile? previewImageFile;
        [Display(Name = "Изображение")]
        public IFormFile? PreviewImageFile
        {
            get { return previewImageFile; }
            set
            {
                previewImageFile = value;
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
