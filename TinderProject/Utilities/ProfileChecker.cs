using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;

namespace TinderProject.Utilities
{
	public class ProfileChecker
	{
        public static HtmlString GetProfileCompletionMessage(User user, IHttpContextAccessor httpContextAccessor)
        {
            string message = null;

            if (user != null && (string.IsNullOrWhiteSpace(user.FirstName)
                || string.IsNullOrWhiteSpace(user.LastName)
                || (user.DateOfBirth == DateTime.MinValue)
                || user.Gender == null
                || user.Preference == null
                || string.IsNullOrWhiteSpace(user.ProfilePictureUrl)
                || string.IsNullOrWhiteSpace(user.Description)))
            {
                var editLink = $"<a href='/UserPage/Edit'><span class='edit-link'>{HtmlEncoder.Default.Encode("here")}</span></a>";
                message = $"Your profile is not complete. Please fill in your profile information by clicking {editLink} to start the matchmaking.";
            }

            return new HtmlString(message);
        }
    }
}
