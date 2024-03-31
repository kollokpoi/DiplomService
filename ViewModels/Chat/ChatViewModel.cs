namespace DiplomService.ViewModels.Chat
{
    public class ChatViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public byte[]? Image { get; set; }
        public int DivisionId { get; set; }
        public string opponentName { get; set; } = string.Empty;
        public List<MessageViewModel> Messages { get; set; } = new();
    }
}
