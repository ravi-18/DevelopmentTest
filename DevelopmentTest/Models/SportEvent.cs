namespace DevelopmentTest.Models
{
    public class SportEvent
    {
        public int Id { get; set; }
        public DateTime EventDate { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public virtual Organizer Organizers { get; set; }
    }
}
