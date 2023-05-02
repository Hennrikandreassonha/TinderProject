namespace TinderProject.Models
{
    public class Interaction
    {
        public int Id { get; set; }
        public int LikerId { get; set; }
        public int LikedId { get; set; }
        public DateTime DateLiked { get; set; }

        public User Liker { get; set; }
        public User Liked { get; set; }
    }
}
