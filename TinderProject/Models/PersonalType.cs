using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinderProject.Models
{
    public class PersonalType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}