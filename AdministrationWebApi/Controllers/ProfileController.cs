using Grpc.Net.Client;
using GrpcServer.Services;
using Microsoft.AspNetCore.Mvc;
using GrpcServer;
using Shared;
using System.Threading.Channels;
using Administration;

namespace AdministrationWebApi.Controllers
{
    [ApiController]
    [Route("[Administration/Profile]")]
    public class ProfileController : ControllerBase
    {
        private  Administration.Profiles.ProfilesClient client;

        private string grpcUrl;
        static readonly SettingsManager settingsMgr = new SettingsManager();

        public ProfileController()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            grpcUrl = settingsMgr.ReadSettings(ServerConfig.grpcUrl);
        }

        [HttpPost("users")]
        public async Task<ActionResult> PostProfile([FromBody] AddProfileRequest profileToAdd)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Profiles.ProfilesClient(channel);
            var reply = await client.AddAsync(profileToAdd);
            return Ok(reply.Message);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProfile([FromRoute] int id, [FromBody] UpdateProfileRequest profileToUpdate)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Profiles.ProfilesClient(channel);
            var reply = await client.Update(profileToUpdate);
            return Ok(reply.Message);
        }
        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteProfile([FromBody] RemoveProfileRequest profileToRemove)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Profiles.ProfilesClient(channel);
            var reply = await client.RemoveAsync(profileToRemove);
            return Ok(reply.Message);
        }
        [HttpDelete]
        public async Task<IActionResult> DeletePhoto([FromBody] RemoveProfilePhotoRequest photoToRemove)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Profiles.ProfilesClient(channel);
            var reply = await client.RemoveAsync(photoToRemove);
            return Ok(reply.Message);
        }
    };
   

}
