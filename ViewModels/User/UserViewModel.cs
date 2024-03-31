using DiplomService.Services.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomService.ViewModels.User
{
    public class UserViewModel
    {

        public string Id { get; set; } = "";

        [Required(ErrorMessage = "Не указано имя")]
        [Display(Name = "Имя")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        [Display(Name = "Фамилия")]
        public string? SecondName { get; set; }

        [Required(ErrorMessage = "Не указано отчество")]
        [Display(Name = "Отчество")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Не указана почта")]
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        [Display(Name = "Адрес электронной почты")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        public string? Email { get; set; } = "";

        [Display(Name = "Номер телефона")]
        [RussianPhoneNumber(ErrorMessage = "Введите действительный номер телефона")]
        public string? PhoneNumber { get; set; }

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

            return "application/octet-stream";
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
                    Image = memoryStream.ToArray();
                }
            }
        }
    }
}
