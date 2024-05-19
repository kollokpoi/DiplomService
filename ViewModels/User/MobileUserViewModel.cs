namespace DiplomService.ViewModels.User
{
    public class MobileUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string Name { get; set; } = "";
        public string SecondName { get; set; } = "";
        public string LastName { get; set; } = "";
        public int Course { get; set; }
        public DateTime Birthday { get; set; }
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = string.Empty;
        public string WorkingPlace { get; set; } = string.Empty;

        public void SetImageBytes(byte[] imageBytes)
        {
            Image = Convert.ToBase64String(imageBytes);
        }
        public byte[] GetDecodedImage()
        {
            if (string.IsNullOrEmpty(Image))
                return Array.Empty<byte>();

            return Convert.FromBase64String(Image);
        }
    }
}
