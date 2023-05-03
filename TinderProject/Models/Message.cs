using static System.Net.Mime.MediaTypeNames;


namespace TinderProject.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SentMessage { get; set; }
        public string? ReceivedMessages { get; set; }
        public DateTime SentTime { get; set; }
        public int SentToId { get; set; }


        public User User { get; set; }


    }
}
