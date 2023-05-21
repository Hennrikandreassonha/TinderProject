using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinderProject.Controllers
{
    public class Dish
    {
        public string Cuisine { get; set; }
        public string DishName { get; set; }
        public string Calories { get; set; }
        public bool IsHealthy { get; set; }
    }
}