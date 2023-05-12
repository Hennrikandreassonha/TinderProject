using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinderProject.Models
{
    public class User
    {
        //Blir PK sen.
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //Kanske endast behöver årtal senare.
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string? PersonalityType { get; set; }
        public DateTime DateOfBirth { get; set; }
        public GenderType? Gender { get; set; }
        public SwipePreference? Preference { get; set; }
        public string? ProfilePictureUrl { get; set; }
		//Begränsar antalet ord man kan lägga till för Description till 50
		[RegularExpression(@"^(\S+\s*){0,49}$", ErrorMessage = "Description cannot exceed 50 words and not begin with a space.")]
		public string Description { get; set; }
        public bool PremiumUser { get; set; }
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
        public List<Interests>? Interests { get; set; }
        public ICollection<Interaction>? LikedByUsers { get; set; }
        public ICollection<Interaction>? LikedUsers { get; set; }
        public ICollection<Match>? Matcher1 { get; set; }
        public ICollection<Match>? Matcher2 { get; set; }
    }
}