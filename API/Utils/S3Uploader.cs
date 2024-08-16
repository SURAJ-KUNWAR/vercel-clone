using Amazon.S3;
using Amazon.S3.Model;

namespace API.Utils
{
    public class S3Uploader
    {
        private AmazonS3Client _s3Client;
        private IConfiguration _configuration;

        public S3Uploader(IConfiguration configuration) 
        {

            _configuration = configuration;
            var s3Config = new AmazonS3Config
            {
                ServiceURL = "https://b08d84ccba335c64e9a6d752c8ca03a5.r2.cloudflarestorage.com",
  
               
            };

            _s3Client = new AmazonS3Client(
             _configuration["S3:ACCESS_KEY"],
            _configuration["S3:SECRET_KEY"],
             s3Config );

        
        }

        public async Task UploadFileAsync(string fileName, string localFilePath)
        {

            try
            {
                var fileContent = await File.ReadAllBytesAsync(localFilePath);
                var putRequest = new PutObjectRequest
                {
                    BucketName = "vercel-clone",
                    Key = fileName,
                    InputStream = new MemoryStream(fileContent),
                    DisablePayloadSigning = true
                };
                var response = await _s3Client.PutObjectAsync(putRequest);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown error encountered on server. Message:'{0}' when writing an object", e.Message);
            }


        }
    }
}
