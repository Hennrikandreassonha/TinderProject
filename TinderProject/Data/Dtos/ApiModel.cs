using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinderProject.Data.Dtos
{
    public class ApiModel
    {
        public ApiModel()
        {
            UserInterests = new List<string>(); // Initialize the list in the constructor
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Age { get; set; }
        public string PersonalityType { get; set; }
        public string Description { get; set; }
        public List<string> UserInterests { get; set; }
    }
}