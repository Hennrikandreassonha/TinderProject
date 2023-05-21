using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinderProject.Data;

namespace TinderProject.Controllers
{
    [Route("/api")]
    [ApiController]
    [AllowAnonymous]
    public class APIController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly AppDbContext _database;

        public APIController(AppDbContext database, IUserRepository repo)
        {
            _database = database;
            _userRepo = repo;
        }

        [HttpGet("/Users/{interest}")]
        public IActionResult GetUserByInterest(string interest)
        {
            //Kollar först om användaren skickat med en parameter.
            if (interest != null)
            {
                //Making first char upper.
                interest = char.ToUpper(interest[0]) + interest.Substring(1);

                var user = _userRepo.GetUserApi(interest);

                if (user == null)
                {
                    return NotFound("No users found.");
                }

                return Ok(user);
            }
            return NotFound("Add an interest.");
        }
        [HttpGet("/Mexican")]
        public IActionResult FakeApiCall()
        {
            var fakeResponse = new
            {
                cuisine = "Mexican",
                dishname = "Tacos",
                calories = "500",
                isHealthy = true
            };

            return Ok(fakeResponse);
        }
    }
}
