using Microsoft.AspNetCore.Mvc;
using Shared;
using Grpc.Net.Client;
using Administration;

namespace AdministrationWebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private Administration.Users.UsersClient? client;

        private string grpcUrl;
        static readonly SettingsManager settingsMgr = new SettingsManager();

        public UserController()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            grpcUrl = settingsMgr.ReadSettings(ServerConfig.GrpcAddress);
        }

        [HttpPost()]
        public async Task<ActionResult> Add([FromBody] AddUserRequest userToAdd)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Users.UsersClient(channel);
            var reply = await client.AddAsync(userToAdd);
            return StatusCode(reply.Code, reply.Message);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateUserRequest userToUpdate)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Users.UsersClient(channel);
            var reply = await client.UpdateAsync(userToUpdate);
            return StatusCode(reply.Code, reply.Message);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromBody] RemoveUserRequest userToDelete)
        {
            using var channel = GrpcChannel.ForAddress(grpcUrl);
            client = new Administration.Users.UsersClient(channel);
            var reply = await client.RemoveAsync(userToDelete);
            return StatusCode(reply.Code, reply.Message);
        }
    }      
}
