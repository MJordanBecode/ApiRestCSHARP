namespace Solution.Ticket.models;

 public class Ticket
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime Create_at { get; set; }
        
        public int UserId { get; set; }
    }