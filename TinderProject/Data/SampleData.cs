using System.Globalization;
using TinderProject.Models;
using TinderProject.Models.ModelEnums;

namespace TinderProject.Data
{
    public class SampleData
    {

        private static Random random = new();
        private static readonly string[]? MaleNames = File.ReadAllLines("./Data/DataToUsers/Men.txt");
        private static readonly string[]? FemaleNames = File.ReadAllLines("./Data/DataToUsers/Women.txt");
        private static readonly string[]? LastNames = File.ReadAllLines("./Data/DataToUsers/Lastnames.txt");
        public static readonly string[]? InterestsArray = File.ReadAllLines("./Data/DataToUsers/Interests.txt");
        public static readonly string[]? CuisineArray = File.ReadAllLines("./Data/DataToUsers/Cuisines.txt");
        public static readonly string[]? PersonalityTypes = File.ReadAllLines("./Data/DataToUsers/Personalitytypes.txt");

        public static List<int> TakenPicUrlIndices { get; set; } = new List<int>();

        public static void CreateData(AppDbContext database)
        {
            string fakeIssuer = "https://example.com";

            if (database.Users.Any())
            {
                return;
            }
            List<User> persons = new();

            for (int i = 0; i < 20; i++)
            {
                var genderType = GenerateGender();
                var firstName = GenerateFirstName(genderType);
                var lastName = GenerateLastName();

                var interests = GenerateInterests();
                var cuisines = GenerateCuisines();
                var personalType = GeneratePersonalType();
                var description = GenerateDescription(personalType);
                var swipePreference = GeneratePreference();
                var premium = GeneratePremium();
                var profilePicUrl = GenerateProfilePicUrl(genderType);

                var dateOfBirth = GenerateDateOfBirth();

                var subjectid = GenerateOpenIDSubject();

                //Fixa så att den lägger in i databasen och i interest tablet.
                User person = new()
                {
                    Gender = genderType,
                    FirstName = firstName,
                    LastName = lastName,
                    PersonalityType = personalType,
                    Description = description,
                    Preference = swipePreference,
                    ProfilePictureUrl = profilePicUrl,
                    DateOfBirth = dateOfBirth,
                    PremiumUser = premium,
                    OpenIDIssuer = fakeIssuer,
                    OpenIDSubject = subjectid
                };

                database.Add(person);

                database.SaveChanges();

                var personAdded = database.Users.OrderByDescending(x => x.Id).FirstOrDefault();

                List<Interests> interestsToAdd = new();

                foreach (var item in interests)
                {
                    Interests interest = new()
                    {
                        UserId = personAdded.Id,
                        Interest = item
                    };

                    interestsToAdd.Add(interest);
                }

                database.AddRange(interestsToAdd);

                
                List<Cuisines> cuisinesToAdd = new();

                foreach (var item in cuisines)
                {
                    Cuisines cuisine = new()
                    {
                        UserId = personAdded.Id,
                        Cuisine = item
                    };

                    cuisinesToAdd.Add(cuisine);
                }

                database.AddRange(cuisinesToAdd);
                database.SaveChanges();
            }
        }

        private static SwipePreference GeneratePreference()
        {
            int randomPreference = random.Next(0, 3);

            SwipePreference[] preferences = { SwipePreference.Male, SwipePreference.Female, SwipePreference.All };

            return preferences[randomPreference];
        }

        public static string GenerateOpenIDSubject()
        {
            string OpenIDSubject = "";
            for (int i = 0; i < 10; i++)
            {

                var numberToAdd = random.Next(0, 10);
                numberToAdd.ToString();
                OpenIDSubject += numberToAdd.ToString();

            }
            return OpenIDSubject;
        }

        public static GenderType GenerateGender()
        {
            int genderChoice = random.Next(0, 3);

            GenderType[] choice = { GenderType.Male, GenderType.Female, GenderType.Other };

            return choice[genderChoice];
        }
        public static string GenerateFirstName(GenderType genderType)
        {
            int nameIndex = random.Next(0, MaleNames.Length);

            if (genderType is GenderType.Male)
            {
                return MaleNames![nameIndex];
            }
            else if (genderType is GenderType.Female)
            {
                return FemaleNames![nameIndex];
            }
            else
            {
                List<string> allNames = new();

                allNames.AddRange(MaleNames);

                allNames.AddRange(FemaleNames);

                int allNamesIndex = random.Next(0, allNames.Count);

                return allNames[allNamesIndex];
            }
        }
        public static string GenerateLastName()
        {
            int nameIndex = random.Next(0, LastNames.Length);

            return LastNames![nameIndex];
        }
        public static string[] GenerateInterests()
        {
            int randomNumber = random.Next(2, 5);
            List<string> interests = new();

            for (int i = 0; i < randomNumber; i++)
            {
                int interestIndex = random.Next(0, InterestsArray.Length);

                while (interests.Contains(InterestsArray![interestIndex]))
                {
                    interestIndex = random.Next(0, InterestsArray.Length);
                }

                interests.Add(InterestsArray![interestIndex]);
            }

            return interests.ToArray();
        }
        public static string[] GenerateCuisines()
        {
            int randomNumber = random.Next(1, 4);
            List<string> cuisines = new();

            for (int i = 0; i < randomNumber; i++)
            {
                int cuisineIndex = random.Next(0, CuisineArray.Length);

                while (cuisines.Contains(CuisineArray![cuisineIndex]))
                {
                    cuisineIndex = random.Next(0, CuisineArray.Length);
                }

                cuisines.Add(CuisineArray![cuisineIndex]);
            }

            return cuisines.ToArray();
        }

        public static string GenerateProfilePicUrl(GenderType genderType)
        {
            string picUrl = "https://xsgames.co/randomusers/assets/avatars";

            int picIndex = random.Next(0, 76);

            int genderChoice = -1;

            while (TakenPicUrlIndices.Contains(picIndex))
            {
                picIndex = random.Next(0, 76);
            }

            if (genderType is GenderType.Other)
            {
                genderChoice = random.Next(0, 2);
            }

            if (genderType is GenderType.Male | genderChoice == 1)
            {
                picUrl += $"/male/{picIndex}.jpg";
            }
            else
            {
                picUrl += $"/female/{picIndex}.jpg";
            }

            return picUrl;
        }
        public static string GenerateDescription(string personalType)
        {
            //int stringStart = random.Next(0, 2);
            var nouns = File.ReadAllLines("./Data/DataToUsers/Nouns.txt");
            var description = "";

            /*if (stringStart == 1)
            {
                description += "I am a ";
            }*/

            // description += $"{personalType}";

            int searchingIndex = random.Next(0, 2);

            if (searchingIndex == 1)
            {
                description += "I am searching for a ";
            }
            else
            {
                description += "I am looking for a ";
            }

            var nounIndex = random.Next(0, nouns.Length);
            description += $"{nouns[nounIndex]}.";
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
        public static string GeneratePersonalType()
        {
            int personalIndex = random.Next(0, PersonalityTypes.Length);

            return PersonalityTypes![personalIndex];
        }

        public static bool GeneratePremium()
        {
            int randomNumber = random.Next(1, 3);

            return randomNumber == 1;
        }
    }
}