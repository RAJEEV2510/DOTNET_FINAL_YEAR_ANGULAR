namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUserName { get; set; }
        public AppUser Sender { get; set; }
        public int RecipientId { get; set; }
        public string ReciverUserName { get; set; }
        public AppUser Recipient { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.Now.ToUniversalTime();
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
        public string content { get; set; }


    }
}
