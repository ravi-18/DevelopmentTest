namespace DevelopmentTest.ViewModels
{
    public class CreateSportEvent
    {
        public int Id { get; set; }
        public DateTime EventDate { get; set; }
        public string EventType { get; set; }
        public string EventName { get; set; }
        public int OrganizerId { get; set; }
    }
}
