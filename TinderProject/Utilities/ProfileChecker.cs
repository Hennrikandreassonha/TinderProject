using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;

namespace TinderProject.Utilities
{
    public class ProfileChecker
    {
        public static bool ProfileIsComplete(User user)
        {
            return user != null && (string.IsNullOrWhiteSpace(user.FirstName)
                || string.IsNullOrWhiteSpace(user.LastName)
                || (user.DateOfBirth == DateTime.MinValue)
                || user.Gender == null
                || user.Preference == null
                || string.IsNullOrWhiteSpace(user.ProfilePictureUrl)
                || string.IsNullOrWhiteSpace(user.Description))
                || user.Interests.Count == 0;
        }
    }
}
