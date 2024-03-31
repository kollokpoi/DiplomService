namespace DiplomService.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int OrganizationsCount { get; set; }
        public int UsersCount { get; set; }
        public int EventsCount { get; set; }
        public string? ClosestEvent { get; set; }
        public int FutureEventsCount { get; set; }
        public string? LastNewsName { get; set; }
        public int EnjoyersCount { get; set; }
        public int ApplicationsCount { get; set; }
    }
}
