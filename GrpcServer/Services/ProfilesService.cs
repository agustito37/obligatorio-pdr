using Grpc.Core;
using GrpcServer;

namespace GrpcServer.Services;

public class ProfilesService : Profiles.ProfilesBase
{
    private readonly ILogger<ProfilesService> _logger;
    public ProfilesService(ILogger<ProfilesService> logger)
    {
        _logger = logger;
    }

    public override Task<ProfileResponse> Add(AddProfileRequest request, ServerCallContext context)
    {
        return Task.FromResult(new ProfileResponse
        {
            Code = 1,
            Message = "Add"
        });
    }

    public override Task<ProfileResponse> Update(UpdateProfileRequest request, ServerCallContext context)
    {
        return Task.FromResult(new ProfileResponse
        {
            Code = 1,
            Message = "Update"
        });
    }

    public override Task<ProfileResponse> Remove(RemoveProfileRequest request, ServerCallContext context)
    {
        return Task.FromResult(new ProfileResponse
        {
            Code = 1,
            Message = "Remove"
        });
    }

    public override Task<ProfileResponse> RemovePhoto(RemoveProfilePhotoRequest request, ServerCallContext context)
    {
        return Task.FromResult(new ProfileResponse
        {
            Code = 1,
            Message = "RemovePhoto"
        });
    }
}

