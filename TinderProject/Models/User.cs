using System.ComponentModel.DataAnnotations;
using TinderProject.Models.ModelEnums;

namespace TinderProject.Models
{
    public class User
    {
        //Blir PK sen.
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //Kanske endast behöver årtal senare.
        public DateTime DateOfBirth { get; set; }
        //Funderade på om man ska ha en enum för Gender?!
        // public GenderType Gender { get; set; }
        public string Gender { get; set; }
        public SwipePreference Preference { get; set; }
        public string ProfilePictureUrl { get; set; }
        //Begränsa antalet ord man kan lägga till för Description?!
        //[RegularExpression(@"^(?:\S+\s+){0,49}\S+$", ErrorMessage = "Description cannot exceed 50 words.")]
        public string Description { get; set; }
        public string? OpenIDIssuer { get; set; }
        public string? OpenIDSubject { get; set; }

        public List<Interests> Interests { get; set; }
        public ICollection<Interaction> LikedByUsers { get; set; }
        public ICollection<Interaction> LikedUsers { get; set; }
        public ICollection<Match> Matcher1 { get; set; }
        public ICollection<Match> Matcher2 { get; set; }
    }
}