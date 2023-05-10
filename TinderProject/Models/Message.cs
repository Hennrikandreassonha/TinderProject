using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;


namespace TinderProject.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SentMessage { get; set; }
        public DateTime SentTime { get; set; }
        public int SentToId { get; set; }
        public int SentFromId { get; set; }
        public bool isRead { get; set; }
        
        public User SendTo{ get; set; }
        public User SentFrom { get; set; }
    }
}
