namespace DiplomService.ViewModels.Chat
{
    public class MessageViewModel
    {
        public int id { get; set; }
        public int? chatId { get; set; } = null;
        public int divisionId { get; set; }
        public string message { get; set; } = string.Empty;
        public bool selfSend { get; set; }
        public DateTime dateTime { get; set; } = DateTime.Now;
    }
}
