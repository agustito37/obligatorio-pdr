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
        string message = "";

        List <Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.UserId == request.UserId);
        if (foundProfile != null) {
            message = "El usuario ya tiene un profile asignado";
            Logs.Logger.Instance.WriteWarning(message);
            return Task.FromResult(new ProfileResponse
            {
                Code = 403,
                Message = message
            });
        }

        Persistence.Instance.AddProfile(new Profile {
            UserId = request.UserId,
            Description = request.Description,
            Abilites = request.Abilities.ToList(),
        });

        message = "Agregado correctamente";
        Logs.Logger.Instance.WriteMessage(message);
        return Task.FromResult(new ProfileResponse
        {
            Code = 200,
            Message = message
        });
    }

    public override Task<ProfileResponse> Update(UpdateProfileRequest request, ServerCallContext context)
    {
        string message = "";

        List<Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.Id == request.Id);
        if (foundProfile == null)
        {
            message = "El perfil no existe";
            Logs.Logger.Instance.WriteWarning(message);
            return Task.FromResult(new ProfileResponse
            {
                Code = 404,
                Message = message
            });
        }

        Persistence.Instance.UpdateProfile(new Profile
        {
            Id = request.Id,
            UserId = request.UserId,
            Description = request.Description,
            Abilites = request.Abilities.ToList(),
        });

        message = "Actualizado correctamente";
        Logs.Logger.Instance.WriteMessage(message);
        return Task.FromResult(new ProfileResponse
        {
            Code = 200,
            Message = message
        });
    }

    public override Task<ProfileResponse> Remove(RemoveProfileRequest request, ServerCallContext context)
    {
        string message = "";

        List<Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.Id == request.Id);
        if (foundProfile == null)
        {
            message = "El perfil no existe";
            Logs.Logger.Instance.WriteWarning(message);
            return Task.FromResult(new ProfileResponse
            {
                Code = 404,
                Message = message
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
            message = "Error al eliminar foto";
            Logs.Logger.Instance.WriteError(message);
            return Task.FromResult(new ProfileResponse
            {
                Code = 500,
                Message = message
            });
        }

        Persistence.Instance.RemoveProfile(foundProfile);

        message = "Eliminado correctamente";
        Logs.Logger.Instance.WriteMessage(message);
        return Task.FromResult(new ProfileResponse
        {
            Code = 200,
            Message = message
        });
    }

    public override Task<ProfileResponse> RemovePhoto(RemoveProfilePhotoRequest request, ServerCallContext context)
    {
        string message = "";

        List<Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.Id == request.Id);
        if (foundProfile == null)
        {
            message = "El perfil no existe";
            Logs.Logger.Instance.WriteWarning(message);
            return Task.FromResult(new ProfileResponse
            {
                Code = 404,
                Message = message
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
            message = "Error al eliminar foto";
            Logs.Logger.Instance.WriteError(message);
            return Task.FromResult(new ProfileResponse
            {
                Code = 500,
                Message = message
            });
        }

        Persistence.Instance.RemoveProfilePhoto(foundProfile.Id);

        message = "Eliminada correctamente";
        Logs.Logger.Instance.WriteMessage(message);
        return Task.FromResult(new ProfileResponse
        {
            Code = 200,
            Message = message
        });
    }
}

