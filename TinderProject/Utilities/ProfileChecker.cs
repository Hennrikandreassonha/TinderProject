using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;

namespace TinderProject.Utilities
{
    public class ProfileChecker
    {
        public static bool ProfileIsComplete(User user)
        {
            bool notComplete = user != null;

            if (user == null)
            {
                return false;
            }


            if (user.Interests == null || user.PersonalityType == null || user.Cuisines == null)
            {
                return false;
            }

            if (user.Interests.Count == 0 || user.PersonalityType?.Length == 0 || user.Cuisines.Count == 0)
            {
                return false;
            }
            if (string.IsNullOrEmpty(user.ProfilePictureUrl.Trim()))
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(user.FirstName)
                || !string.IsNullOrWhiteSpace(user.LastName)
                || (user.DateOfBirth != DateTime.MinValue)
                || user.Gender != null
                || user.Preference != null
                || !string.IsNullOrWhiteSpace(user.ProfilePictureUrl)
                || !string.IsNullOrWhiteSpace(user.Description);
        }
    }
}
