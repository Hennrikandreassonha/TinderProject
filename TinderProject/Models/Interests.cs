namespace TinderProject.Models
{
    public class Interests
    {
        public int Id { get; set; }
        public string Interest { get; set; }
        public int UserId{ get; set; }
        public User User { get; set; }
    }
}
