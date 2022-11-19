using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;
using Administration;
using Shared;
using System.Net;

namespace AdministrationWebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProfileController : ControllerBase
    {
        private  Administration.Profiles.ProfilesClient? client;
        private string grpcUrl;
        static readonly SettingsManager settingsMgr = new SettingsManager();

        public ProfileController()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            grpcUrl = settingsMgr.ReadSettings(ServerConfig.GrpcAddress);
        }

        [HttpPost("")]
        public async Task<ActionResult> Add([FromBody] Profile profileToAdd)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Profiles.ProfilesClient(channel);

            AddProfileRequest profile = new AddProfileRequest();
            profile.UserId = profileToAdd.UserId;
            profile.Description = profileToAdd.Description;
            profile.Abilities.Add(profileToAdd.Abilites);

            var reply = await client.AddAsync(profile);
            return StatusCode(reply.Code, reply.Message);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] Profile profileToUpdate)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Profiles.ProfilesClient(channel);

            UpdateProfileRequest profile = new UpdateProfileRequest();
            profile.Id = profileToUpdate.Id;
            profile.UserId = profileToUpdate.UserId;
            profile.Description = profileToUpdate.Description;
            profile.Abilities.Add(profileToUpdate.Abilites);

            var reply = await client.UpdateAsync(profile);
            return StatusCode(reply.Code, reply.Message);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromBody] RemoveProfileRequest profileToRemove)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Profiles.ProfilesClient(channel);
            var reply = await client.RemoveAsync(profileToRemove);
            return StatusCode(reply.Code, reply.Message);
        }
        [HttpDelete("photo/{id}")]
        public async Task<IActionResult> DeletePhoto([FromBody] RemoveProfilePhotoRequest photoToRemove)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Profiles.ProfilesClient(channel);
            var reply = await client.RemovePhotoAsync(photoToRemove);
            return StatusCode(reply.Code, reply.Message);
        }
    };
   

}
