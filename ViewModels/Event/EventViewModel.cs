using DiplomService.Models;
using DiplomService.Services;
using DiplomService.ViewModels.Measures;
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
        public virtual List<ViewModels.User.UserSuggestionViewModel> DivisionLeaders { get; set; } = new();


        [NotMapped]
        public string? MimeType { get { return ImageWorker.GetImageMimeType(PriviewImage); } }

        public List<EventMeasuresViewModel> MeasuresViewModel { get; set; } = new();
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
