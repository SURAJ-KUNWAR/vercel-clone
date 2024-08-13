
using API.DTO;
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

        public Deploy(IConfiguration configuration)
        {
            _configuration = configuration;
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
               
                return Ok(new {id = id});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error has occured : {ex.Message}");
            }
           
        }
    }
}