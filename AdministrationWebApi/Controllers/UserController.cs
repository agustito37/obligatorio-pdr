using Microsoft.AspNetCore.Mvc;
using Shared;
using GrpcServer;
using GrpcServer.Services;
using Grpc.Net.Client;
using Administration;

namespace AdministrationWebApi.Controllers
{
    [ApiController]
    [Route("[users]")]
    public class UserController : ControllerBase
    {
        private Administration.Users.UsersClient client;

        private string grpcUrl;
        static readonly SettingsManager settingsMgr = new SettingsManager();

        public UserController()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            grpcUrl = settingsMgr.ReadSettings(ServerConfig.grpcUrl);
        }

        [HttpPost("users")]
        public async Task<ActionResult> PostUser([FromBody] AddUserRequest userToAdd)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Users.UsersClient(channel);
            var reply = await client.AddAsync(userToAdd);
            return Ok(reply.Message);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProfile([FromRoute] int id, [FromBody] UpdateUserRequest userToUpdate)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Users.UsersClient(channel);
            var reply = await client.Update(userToUpdate);
            return Ok(reply.Message);
        }
        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteProfile([FromBody] RemoveUserRequest userToDelete)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Users.UsersClient(channel);
            var reply = await client.RemoveAsync(userToDelete);
            return Ok(reply.Message);
        }
    }      
}
