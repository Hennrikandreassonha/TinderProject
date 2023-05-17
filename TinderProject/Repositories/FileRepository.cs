using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TinderProject.Repositories
{
	public class FileRepository
	{
		private readonly IConfiguration configuration;
		private readonly IWebHostEnvironment environment;

		public FileRepository(IConfiguration configuration, IWebHostEnvironment environment)
		{
			this.configuration = configuration;
			this.environment = environment;

			string relativeFolderPath = Path.Combine(
				environment.ContentRootPath,
				configuration["Uploads:FolderPath"]
			);
			FolderPath = Path.GetFullPath(relativeFolderPath) + Path.DirectorySeparatorChar;
		}

		public string FolderPath { get; set; }

		/// <summary>
		/// Save the specified file in the uploads folder of the website. The path of this folder is defined in the configuration file.
		/// </summary>
		public async Task SaveFileAsync(IFormFile formFile, string path)
		{
			// First get the absolute path of the uploaded file and make sure that it's in the uploads folder.
			string relativeFilePath = Path.Combine(FolderPath, path);
			string absoluteFilePath = Path.GetFullPath(relativeFilePath);
			if (!absoluteFilePath.StartsWith(FolderPath))
			{
				// Throw an exception if the file is not in the uploads folder, because that's a security risk.
				throw new ArgumentException(
					"File cannot be uploaded to path outside of the uploads folder."
					+ Environment.NewLine
					+ "Uploads folder: " + FolderPath
					+ Environment.NewLine
					+ "File path: " + absoluteFilePath
				);
			}

			// Create all the directories in the path if they don't already exist.
			Directory.CreateDirectory(Path.GetDirectoryName(absoluteFilePath));

			// Save the file in the uploads folder.
			using var stream = new FileStream(absoluteFilePath, FileMode.Create);
			await formFile.CopyToAsync(stream);
		}

		/// <summary>
		/// Given the path to a file in the uploads folder, return the relative URL to that file, for use in (for example) HTML code to link to the file.
		/// </summary>
		public string GetFileURL(string path)
		{
			string fileSystemPath = configuration["Uploads:URLPath"]
				+ "/"
				+ Path.GetRelativePath(FolderPath, path);
			string urlPath = fileSystemPath.Replace(Path.DirectorySeparatorChar, '/');
			string encodedURLPath = HttpUtility.UrlPathEncode(urlPath);
			return encodedURLPath;
		}

		public void ClearDirectory(User user)
		{
			string userFolderPath = Path.Combine(
				FolderPath,
				user.Id.ToString()
				);

			string[] files = Directory.GetFiles(userFolderPath);
			foreach (string file in files)
			{
				File.Delete(file);
				Console.WriteLine($"{file} is deleted.");
			}
		}
		public string GetProfilePic(User user)
		{
			string userFolderPath = Path.Combine(
				FolderPath,
				user.Id.ToString()
				);

			var pics = Directory.GetFiles(userFolderPath);

			return pics[0];
		}
	}
}