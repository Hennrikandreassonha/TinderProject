using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TinderProject.Data;
using TinderProject.Repositories;
using TinderProject.Repositories.Repositories_Interfaces;

namespace TinderProject.Pages.UserPages
{
	public class IndexModel : PageModel
	{
		private readonly IUserRepository _userRepository;
		private readonly AppDbContext _database;
		private readonly IConfiguration _configuration;
		public IndexModel(IUserRepository userRepository, AppDbContext database, BlobRepo blobRepo, IConfiguration config)
		{
			_userRepository = userRepository;
			_database = database;
			_configuration = config;
		}

		public User LoggedInUser { get; set; }
		public string PictureSasURI { get; set; }

		public void OnGet()
		{
			BlobRepo _blobRepo = new BlobRepo(_configuration);

			LoggedInUser = _userRepository.GetLoggedInUser();
			
			PictureSasURI = _blobRepo.GenerateSASLink(LoggedInUser);
		}
	}
}