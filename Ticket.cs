// Entities > Ticket.cs
using Solution.Ticket;
namespace Solution.Ticket
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime Create_at { get; set; }
    }
}