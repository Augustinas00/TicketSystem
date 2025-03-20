namespace TicketSystem.Api.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Event> Events { get; set; } = new List<Event>();
    }
}