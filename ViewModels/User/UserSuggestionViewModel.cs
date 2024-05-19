using System.ComponentModel.DataAnnotations;

namespace DiplomService.ViewModels.User
{
    public class UserSuggestionViewModel
    {
        [Required(ErrorMessage = "Выберите из списка")]
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
    }
}
