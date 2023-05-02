using System.Globalization;
using TinderProject.Models;

namespace TinderProject.Data
{
    public class SampleData
    {

        private static Random random = new();
        private static readonly string[]? MaleNames = File.ReadAllLines("./Data/DataToUsers/Men.txt");
        private static readonly string[]? FemaleNames = File.ReadAllLines("./Data/DataToUsers/Women.txt");
        private static readonly string[]? LastNames = File.ReadAllLines("./Data/DataToUsers/Lastnames.txt");
        private static readonly string[]? InterestsArray = File.ReadAllLines("./Data/DataToUsers/Interests.txt");
        public static List<int> TakenPicUrlIndices { get; set; } = new List<int>();

        public static void CreateData(AppDbContext database)
        {

            if (database.Users.Any())
            {
                return;
            }
            List<User> persons = new();

            for (int i = 0; i < 10; i++)
            {
                var gender = GenerateGender();
                var firstName = GenerateFirstName(gender);
                var lastName = GenerateLastName();

                var interests = GenerateInterests();
                var description = GenerateDescription();

                var profilePicUrl = GenerateProfilePicUrl(gender);

                var dateOfBirth = GenerateDateOfBirth();

                //Fixa så att den lägger in i databasen och i interest tablet.
                User person = new()
                {
                    Gender = gender,
                    FirstName = firstName,
                    LastName = lastName,
                    Description = description,
                    ProfilePictureUrl = profilePicUrl,
                    DateOfBirth = dateOfBirth
                };

                database.Add(person);

                database.SaveChanges();

                var personAdded = database.Users.OrderByDescending(x => x.Id).FirstOrDefault();

                List<Interests> intrestsToAdd = new();

                //foreach (var item in interests)
                //{
                //    Interests interest = new()
                //    {
                //        Id = personAdded.Id,
                //        Interest = item
                //    };

                //    intrestsToAdd.Add(interest);
                //}

                //database.AddRange(intrestsToAdd);
                //database.SaveChanges();
            }

        }
        public static string GenerateGender()
        {
            int genderChoice = random.Next(0, 2);

            if (genderChoice == 0)
            {
                return "Male";
            }
            else
            {
                return "Female";
            }
        }
        public static string GenerateFirstName(string gender)
        {
            int nameIndex = random.Next(0, MaleNames.Length);

            if (gender == "Male")
            {
                return MaleNames![nameIndex];
            }
            else
            {
                return FemaleNames![nameIndex];
            }
        }
        public static string GenerateLastName()
        {

            int nameIndex = random.Next(0, LastNames.Length);

            return LastNames![nameIndex];
        }
        public static string[] GenerateInterests()
        {
            //Users will have atleast 3 and and a maximum of 6 interests.

            int randomNumber = random.Next(2, 5);
            List<string> interests = new();

            for (int i = 0; i < randomNumber; i++)
            {
                int interestIndex = random.Next(0, InterestsArray.Length);

                interests.Add(InterestsArray![interestIndex]);
            }

            return interests.ToArray();
        }

        public static string GenerateProfilePicUrl(string gender)
        {
            string picUrl = "https://xsgames.co/randomusers/assets/avatars";

            int picIndex = random.Next(0, 76);

            while (TakenPicUrlIndices.Contains(picIndex))
            {
                picIndex = random.Next(0, 76);
            }

            if (gender == "Male")
            {
                picUrl += $"/male/{picIndex}.jpg";
            }
            else
            {
                picUrl += $"/female/{picIndex}.jpg";
            }

            return picUrl;
        }
        public static string GenerateDescription()
        {
            int stringStart = random.Next(0, 2);
            var adjectives = File.ReadAllLines("./Data/DataToUsers/Adjectives.txt");
            var nouns = File.ReadAllLines("./Data/DataToUsers/Nouns.txt");
            var description = "";

            if (stringStart == 1)
            {
                description += "I am a ";
            }

            var firstAdjectiveIndex = random.Next(0, adjectives.Length);
            description += $"{adjectives[firstAdjectiveIndex]}";

            var firstNounIndex = random.Next(0, nouns.Length);
            description += $" {nouns[firstNounIndex]}"; 

            int searchingIndex = random.Next(0, 2);

            if (searchingIndex == 1)
            {
                description += " that is searching for a ";
            }
            else
            {
                description += " that is looking for a ";
            }

            var secondAdjectiveIndex = random.Next(0, adjectives.Length);

            description += $"{adjectives[secondAdjectiveIndex]}";

            var firstsecondNounIndex = random.Next(0, nouns.Length);

            description += $" {nouns[firstsecondNounIndex]}.";

            return description[..1].ToUpper() + description[1..].ToLower();
        }

        public static DateTime GenerateDateOfBirth()
        {
            int randomYear = random.Next(1960, 2005);
            int randomMonth = random.Next(1, 13);
            int randomDay = random.Next(1, 29);

            string? randomMonthString = randomMonth < 10 ? "0" + randomMonth.ToString() : randomMonth.ToString();

            string randomDayString = randomDay < 10 ? "0" + randomDay.ToString() : randomDay.ToString();

            var dateString = randomDayString + randomMonthString + randomYear.ToString();

            return DateTime.ParseExact(dateString, "ddMMyyyy", CultureInfo.InvariantCulture);
        }
    }
}