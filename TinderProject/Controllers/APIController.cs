using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinderProject.Data;

namespace TinderProject.Controllers
{
    [Route("/api")]
    [ApiController]
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
                var user = _userRepo.GetUserApi(interest);

                if (user == null)
                {
                    return NotFound("No users found.");
                }

                return Ok(user);
            }
            return NotFound("Add an interest.");
        }
    }
}
