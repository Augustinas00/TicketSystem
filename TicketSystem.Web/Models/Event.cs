namespace TicketSystem.Web.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EventDate { get; set; }
        public string Venue { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<Ticket> Tickets { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Ticket
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int Row { get; set; }
        public int Seat { get; set; }
        public int CategoryId { get; set; }
        public TicketCategory Category { get; set; }
        public bool IsSold { get; set; }
    }

    public class TicketCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}