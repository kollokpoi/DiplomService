using DiplomService.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomService.ViewModels.Event
{
    public class EventViewModel
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.Upload)]
        public byte[]? PriviewImage { get; set; }

        [Required(ErrorMessage = "Поле обязательно к заполнению.")]
        [MaxLength(40)]
        [Display(Name = "Название*")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Поле обязательно к заполнению.")]
        [Display(Name = "Дата начала*")]
        public DateTime DateOfStart { get; set; } = DateTime.Now.Date;

        [Display(Name = "Дата окончания")]
        public DateTime? DateOfEnd { get; set; }
        [Required]
        [Display(Name = "Является ли событие публичным*")]
        public bool PublicEvent { get; set; } = true;
        [Required]
        [Display(Name = "Имеются ли направления*")]
        public bool DivisionsExist { get; set; } = true;

        [Range(-180, 180)]
        public double? Longitude { get; set; }
        [Range(-90, 90)]
        public double? Latitude { get; set; }
        public string? PlaceName { get; set; }


        public virtual List<Division> Divisions { get; set; } = new();
        public virtual List<Organization> Organizations { get; set; } = new();
        public virtual List<Measure> Measures { get; set; } = new();

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
