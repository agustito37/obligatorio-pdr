using Grpc.Core;
using GrpcServer;
using Shared;

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
        List<Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.UserId == request.UserId);
        if (foundProfile != null) {
            return Task.FromResult(new ProfileResponse
            {
                Code = 403,
                Message = "El usuario ya tiene un profile asignado"
            });
        }

        Persistence.Instance.AddProfile(new Profile {
            UserId = request.UserId,
            Description = request.Description,
            Abilites = request.Abilities.ToList(),
        });

        return Task.FromResult(new ProfileResponse
        {
            Code = 200,
            Message = "Agregado correctamente"
        });
    }

    public override Task<ProfileResponse> Update(UpdateProfileRequest request, ServerCallContext context)
    {
        List<Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.Id == request.Id);
        if (foundProfile == null)
        {
            return Task.FromResult(new ProfileResponse
            {
                Code = 404,
                Message = "El perfil no existe"
            });
        }

        Persistence.Instance.UpdateProfile(new Profile
        {
            Id = request.Id,
            UserId = request.UserId,
            Description = request.Description,
            Abilites = request.Abilities.ToList(),
        });

        return Task.FromResult(new ProfileResponse
        {
            Code = 200,
            Message = "Actualizado correctamente"
        });
    }

    public override Task<ProfileResponse> Remove(RemoveProfileRequest request, ServerCallContext context)
    {
        List<Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.Id == request.Id);
        if (foundProfile == null)
        {
            return Task.FromResult(new ProfileResponse
            {
                Code = 404,
                Message = "El perfil no existe"
            });
        }

        try
        {
            if (foundProfile.ImagePath != "")
            {
                FileHandler.RemoveFile(foundProfile.ImagePath);
            }
        }
        catch (Exception) {
            return Task.FromResult(new ProfileResponse
            {
                Code = 500,
                Message = "Error al eliminar foto"
            });
        }

        Persistence.Instance.RemoveProfile(foundProfile);

        return Task.FromResult(new ProfileResponse
        {
            Code = 200,
            Message = "Eliminado correctamente"
        });
    }

    public override Task<ProfileResponse> RemovePhoto(RemoveProfilePhotoRequest request, ServerCallContext context)
    {
        List<Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.Id == request.Id);
        if (foundProfile == null)
        {
            return Task.FromResult(new ProfileResponse
            {
                Code = 404,
                Message = "El perfil no existe"
            });
        }

        try
        {
            if (foundProfile.ImagePath != "")
            {
                FileHandler.RemoveFile(foundProfile.ImagePath);
            }
        }
        catch (Exception)
        {
            return Task.FromResult(new ProfileResponse
            {
                Code = 500,
                Message = "Error al eliminar foto"
            });
        }

        Persistence.Instance.RemoveProfilePhoto(foundProfile.Id);

        return Task.FromResult(new ProfileResponse
        {
            Code = 200,
            Message = "Eliminada correctamente"
        });
    }
}

