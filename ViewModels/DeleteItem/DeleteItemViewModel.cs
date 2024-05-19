namespace DiplomService.ViewModels.DeleteItem
{
    public class DeleteItemViewModel
    {
        public enum ItemTypes
        {
            Event,
            Organization,
        }
        public ItemTypes ItemType { get; set; }
        public string Reason { get; set; }
        public string ItemName { get; set; }
        public string EmailToSend { get; set; }
    }
}
