using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinderProject.Controllers
{
    public class Dish
    {
        public int id { get; set; }
        public string accountOwner { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string primaryIngredient { get; set; }
        public string country { get; set; }
        public string category { get; set; }
        public List<string> ingredient { get; set; }
    }
}