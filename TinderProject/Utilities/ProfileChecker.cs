using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;

namespace TinderProject.Utilities
{
    public class ProfileChecker
    {
        public static bool ProfileIsComplete(User user)
        {
            bool notCompelete = user != null;

            if(user == null){
                return false;
            }

            if(user.Interests.Count == 0 || user.PersonalityType == null){
                return false;
            }

            notCompelete = (!string.IsNullOrWhiteSpace(user.FirstName)
                || !string.IsNullOrWhiteSpace(user.LastName)
                || (user.DateOfBirth != DateTime.MinValue)
                || user.Gender != null
                || user.Preference != null
                || !string.IsNullOrWhiteSpace(user.ProfilePictureUrl)
                || !string.IsNullOrWhiteSpace(user.Description))
                || user.Interests.Count != 0;


            return notCompelete;
        }
    }
}
