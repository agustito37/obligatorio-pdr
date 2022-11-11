using Grpc.Core;
using GrpcServer;

namespace GrpcServer.Services;

public class UsersService : Users.UsersBase
{
    private readonly ILogger<UsersService> _logger;
    public UsersService(ILogger<UsersService> logger)
    {
        _logger = logger;
    }

    public override Task<UserResponse> Add(AddUserRequest request, ServerCallContext context)
    {
        return Task.FromResult(new UserResponse
        {
            Code = 1,
            Message = "Add"
        });
    }

    public override Task<UserResponse> Update(UpdateUserRequest request, ServerCallContext context)
    {
        return Task.FromResult(new UserResponse
        {
            Code = 1,
            Message = "Update"
        });
    }

    public override Task<UserResponse> Remove(RemoveUserRequest request, ServerCallContext context)
    {
        return Task.FromResult(new UserResponse
        {
            Code = 1,
            Message = "Remove"
        });
    }
}

