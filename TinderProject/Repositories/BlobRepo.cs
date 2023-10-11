using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace TinderProject.Repositories
{

    public class BlobRepo
    {
        private readonly IConfiguration _configuration;
        private readonly BlobContainerClient _filesContainer;

        public BlobRepo(IConfiguration configuration)
        {
            _configuration = configuration;
            string _storageGroup = _configuration["Authentication:StorageAccount:StorageGroup"];
            string _accessKey = _configuration["Authentication:StorageAccount:Accesskey"];
            
            var credential = new StorageSharedKeyCredential(_storageGroup, _accessKey);

            var blobUri = $"https://{_storageGroup}.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _filesContainer = blobServiceClient.GetBlobContainerClient("pictures");
        }

        public void UploadPhoto(IFormFile fileToPersist, string saveAsFileName, string userId)
        {
            //Skapar en CSV med namn och UserId. Då kan vi använda ID när vi queryar bilder sen.
            string nameIdCsv = $"{userId},{saveAsFileName}";

            try
            {
                //Skapar eller hittar den "path" som filen kommer lägga sig i.
                BlobClient blob = _filesContainer.GetBlobClient(nameIdCsv);

                using (Stream file = fileToPersist.OpenReadStream())
                {
                    //Med falseparametern kommer filen att skrivas över. Annnars ger den ex.
                    blob.Upload(file, true);
                }

                var uri = blob.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Den sprängdes: {ex}");
            }
        }
        //Borde vara Async men funkar inte just nu.
        public IEnumerable<string> GetUserPictures(string userId)
        {
            var blobItems = new List<string>();

            foreach (BlobItem blobItem in _filesContainer.GetBlobs())
            {
                var splittedName = blobItem.Name.Split(",");

                if (splittedName[0] == userId)
                {
                    var blobClient = _filesContainer.GetBlobClient(blobItem.Name);

                    blobItems.Add(blobClient.Uri.ToString());
                }
            }

            return blobItems;
        }

        public string DeleteBlob(string blobUrl)
        {
            try
            {
                var correctName = GetBlobName(blobUrl);
                BlobClient blobClient = _filesContainer.GetBlobClient(correctName);

                var response = blobClient.Delete();

                if (response.Status.ToString().StartsWith("2"))
                {
                    return "Pic deleted";
                }
                else
                {
                    Console.WriteLine($"Err: {response.Status}");
                    return "Blob deletion failed";
                }
            }
            catch (Exception ex)
            {
                return $"Err: {ex.Message}";
            }
        }
        public string GetBlobName(string blobLink)
        {
            //Turns link into blob name.
            var splitted = blobLink.Split("/");

            var blobName = splitted.Last().Replace("%2C", ",");

            return blobName;
        }
    }
}
