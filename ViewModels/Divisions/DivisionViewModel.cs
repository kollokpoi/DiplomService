using DiplomService.Services;
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
        public List<ViewModels.User.UserSuggestionViewModel> DivisionLeaders { get; set; } = new();

        public byte[]? PriviewImage { get; set; } = null;
        [NotMapped]
        public string? MimeType { get { return ImageWorker.GetImageMimeType(PriviewImage); } }

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
