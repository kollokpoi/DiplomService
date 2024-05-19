using DiplomService.Services;
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
        public string EventName { get; set; } = string.Empty;

        public List<SectionViewModel> Sections { get; set; } = new();

        [NotMapped]
        public string? MimeType { get { return ImageWorker.GetImageMimeType(PriviewImage); } }

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
