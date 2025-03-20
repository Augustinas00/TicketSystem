namespace TicketSystem.Api.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public int Row { get; set; }
        public int Seat { get; set; }
        public int CategoryId { get; set; }
        public TicketCategory Category { get; set; }
        public bool IsSold { get; set; }
    }
}