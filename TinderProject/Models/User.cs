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
        public string Gender { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Description { get; set; }
        public string? OpenIDIssuer { get; set; }
        public string? OpenIDSubject { get; set; }
        public List<Interests> Interests { get; set; }
    }
}
