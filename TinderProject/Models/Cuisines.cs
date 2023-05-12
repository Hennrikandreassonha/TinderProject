using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinderProject.Models
{
    public class Cuisines
    {
        public int Id { get; set; }
        public string Cuisine { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}