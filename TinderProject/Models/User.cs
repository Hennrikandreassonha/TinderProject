using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinderProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PersonalityType { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [MinimumAge(18, ErrorMessage = "You must be at least 18 years old.")]
        public DateTime DateOfBirth { get; set; }
        public GenderType? Gender { get; set; }
        public SwipePreference? Preference { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string Description { get; set; }
        public bool PremiumUser { get; set; }
        public bool AgeFormula { get; set; } = true;
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? OpenIDIssuer { get; set; }
        public string? OpenIDSubject { get; set; }

        public int Age
        {
            get
            {
                int age = DateTime.Today.Year - DateOfBirth.Year;
                if (DateOfBirth > DateTime.Today.AddYears(-age))
                {
                    age--;
                }
                return age;
            }
        }

        public List<Message>? SentTo { get; set; }
        public List<Message>? SentFrom { get; set; }
        public List<Cuisines>? Cuisines { get; set; }
        public List<Interests>? Interests { get; set; }
        public ICollection<Interaction>? LikedByUsers { get; set; }
        public ICollection<Interaction>? LikedUsers { get; set; }
        public ICollection<Match>? Matcher1 { get; set; }
        public ICollection<Match>? Matcher2 { get; set; }
    }
}