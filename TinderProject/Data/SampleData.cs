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
        private static readonly string[]? InterestsArray = File.ReadAllLines("./Data/DataToUsers/Interests.txt");
        private static readonly string[]? PersonalTypes = File.ReadAllLines("./Data/DataToUsers/Adjectives.txt");

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
                var personalTypes = GeneratePersonalTypes();
                var description = GenerateDescription();
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

                List<Interests> intrestsToAdd = new();

                foreach (var item in interests)
                {
                    Interests interest = new()
                    {
                        UserId = personAdded.Id,
                        Interest = item
                    };

                    intrestsToAdd.Add(interest);
                }

                List<PersonalType> personalTypesToAdd = new();

                foreach (var item in personalTypes)
                {
                    PersonalType interest = new()
                    {
                        UserId = personAdded.Id,
                        Type = item
                    };

                    personalTypesToAdd.Add(interest);
                }
                database.AddRange(personalTypesToAdd);
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
            //Users will have atleast 3 and and a maximum of 6 interests.

            int randomNumber = random.Next(2, 5);
            List<string> interests = new();

            for (int i = 0; i < randomNumber; i++)
            {
                int interestIndex = random.Next(0, InterestsArray.Length);

                while (interests.Contains(InterestsArray![interestIndex]))
                {
                    if (interestIndex != InterestsArray.Count())
                    {
                        interestIndex++;
                    }
                    else
                    {
                        interestIndex--;
                    }
                }
                interests.Add(InterestsArray![interestIndex]);
            }

            return interests.ToArray();
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
        public static string GenerateDescription()
        {
            int stringStart = random.Next(0, 2);
            var nouns = File.ReadAllLines("./Data/DataToUsers/Nouns.txt");
            var description = "";

            if (stringStart == 1)
            {
                description += "I am a ";
            }

            var firstAdjectiveIndex = random.Next(0, PersonalTypes.Length);
            description += $"{PersonalTypes[firstAdjectiveIndex]}";

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

            var secondAdjectiveIndex = random.Next(0, PersonalTypes.Length);

            description += $"{PersonalTypes[secondAdjectiveIndex]}";

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

        public static string[] GeneratePersonalTypes()
        {
            int randomNumber = random.Next(2, 5);

            List<string> personalTypes = new();

            for (int i = 0; i < randomNumber; i++)
            {
                int personalTypeIndex = random.Next(0, PersonalTypes.Length);

                //Using this look to not add same personaltype 2 times.
                while (personalTypes.Contains(InterestsArray![personalTypeIndex]))
                {
                    if (personalTypeIndex != personalTypes.Count())
                    {
                        personalTypeIndex++;
                    }
                    else
                    {
                        personalTypeIndex--;
                    }
                }

                personalTypes.Add(InterestsArray![personalTypeIndex]);
            }

            return personalTypes.ToArray();
        }
        public static bool GeneratePremium()
        {
            int randomNumber = random.Next(1, 3);

            return randomNumber == 1;
        }
    }
}