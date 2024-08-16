
using API.DTO;
using API.Utils;
using dotenv.net;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Deploy : ControllerBase
    {
        private IConfiguration _configuration;
        private S3Uploader _uploader;

        public Deploy(IConfiguration configuration)
        {
            _configuration = configuration;
            _uploader = new S3Uploader(_configuration);
        }
        [HttpPost]
        public async Task<IActionResult> DeployRep([FromBody] DeployRequest request){
            string repoUrl  = request.RepoUrl;
            if (string.IsNullOrEmpty(repoUrl))
            {
                return BadRequest("repoUrl is required.");
            }
            string id = Utils.Utils.Generate();
            string output = Path.Combine("output", id);

            string accessToken = _configuration["Git:AccessToken"];

            if ( string.IsNullOrEmpty(accessToken))
            {
               return StatusCode(500, "Git credentials are not configured.");
             }

            var creds = new UsernamePasswordCredentials()
            {
                Username = "x-access-token",
                Password = accessToken,
            };
            CredentialsHandler credHandler = (_url, _user, _cred) => creds;
            var cloneOpts = new CloneOptions { CredentialsProvider = credHandler };
            try
            {
                await Task.Run(() =>
                {
                    Repository.Clone(repoUrl, output , cloneOpts);
                   
                });
                List<string> files = Utils.Utils.GetAllFiles(Path.Combine(Directory.GetCurrentDirectory(), "output", id));

               
                foreach (var file in files)
                {
                    var keyword = "output";
                    int startIndex = file.IndexOf(keyword)+7;
                    await _uploader.UploadFileAsync(file.Substring(startIndex).Replace("\\", "/"), file);
                }

                return Ok(new {id = id});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error has occured : {ex.Message}");
            }
           
        }
    }
}