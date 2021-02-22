namespace Sirius.Entities
{
    public class Message
    {
        public string Sender { get; set; }
        public int SenderId { get; set; }
        public string Receiver { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
    }
}
