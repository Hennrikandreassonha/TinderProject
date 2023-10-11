using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace TinderProject.Repositories
{
    public class BlobRepo
    {
        private readonly string _storageAccount = "mingrupp9f51";
        private readonly string _accessKey = "UvwvbjCI0jOYhR6EeLn+2ao+v1AC0qaA6nXsrGcgnCXFT7uQLUZQSBN1roxq+v9+NqWrZSUKlCCD+AStg+ROfA==";
        private readonly BlobContainerClient _filesContainer;

        public BlobRepo()
        {
            var credential = new StorageSharedKeyCredential(_storageAccount, _accessKey);

            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _filesContainer = blobServiceClient.GetBlobContainerClient("pictures");
        }
        public async Task<string> GetBlobUrlAsync(string blobName)
        {
            BlobClient blobClient = _filesContainer.GetBlobClient("BingImageOfTheDay.jpg");
            BlobBaseClient blobBaseClient = blobClient as BlobBaseClient;


            if (blobBaseClient != null)
            {
                var picUrl = blobBaseClient.Uri.ToString();
                return picUrl;
            }

            return null;
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
