using Grpc.Core;
using GrpcServer;
using Shared;
using GrpcServer.Logs;

namespace GrpcServer.Services;

public class ProfilesService : Profiles.ProfilesBase
{
    public override Task<ProfileResponse> Add(AddProfileRequest request, ServerCallContext context)
    {
        string resultMessage = "";

        List <Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.UserId == request.UserId);
        if (foundProfile != null) {
            resultMessage = "El usuario ya tiene un profile asignado";
            Logger.Instance.WriteWarning(resultMessage);
            return Task.FromResult(new ProfileResponse
            {
                Code = 403,
                Message = resultMessage
            });
        }

        int id = Persistence.Instance.AddProfile(new Profile {
            UserId = request.UserId,
            Description = request.Description,
            Abilites = request.Abilities.ToList(),
        });

        resultMessage = "Agregado correctamente id:" + id;
        Logger.Instance.WriteInfo(resultMessage);
        return Task.FromResult(new ProfileResponse
        {
            Code = 200,
            Message = resultMessage
        });
    }

    public override Task<ProfileResponse> Update(UpdateProfileRequest request, ServerCallContext context)
    {
        string resultMessage = "";

        List<Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.Id == request.Id);
        if (foundProfile == null)
        {
            resultMessage = "El perfil no existe";
            Logger.Instance.WriteWarning(resultMessage);
            return Task.FromResult(new ProfileResponse
            {
                Code = 404,
                Message = resultMessage
            });
        }

        Persistence.Instance.UpdateProfile(new Profile
        {
            Id = request.Id,
            UserId = request.UserId,
            Description = request.Description,
            Abilites = request.Abilities.ToList(),
        }); ;

        resultMessage = "Actualizado correctamente";
        Logger.Instance.WriteInfo(resultMessage);
        return Task.FromResult(new ProfileResponse
        {
            Code = 200,
            Message = resultMessage
        });
    }

    public override Task<ProfileResponse> Remove(RemoveProfileRequest request, ServerCallContext context)
    {
        string resultMessage = "";

        List<Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.Id == request.Id);
        if (foundProfile == null)
        {
            resultMessage = "El perfil no existe";
            Logger.Instance.WriteWarning(resultMessage);
            return Task.FromResult(new ProfileResponse
            {
                Code = 404,
                Message = resultMessage
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
            resultMessage = "Error al eliminar foto";
            Logger.Instance.WriteError(resultMessage);
            return Task.FromResult(new ProfileResponse
            {
                Code = 500,
                Message = resultMessage
            });
        }

        Persistence.Instance.RemoveProfile(foundProfile);

        resultMessage = "Eliminado correctamente";
        Logger.Instance.WriteInfo(resultMessage);
        return Task.FromResult(new ProfileResponse
        {
            Code = 200,
            Message = resultMessage
        });
    }

    public override Task<ProfileResponse> RemovePhoto(RemoveProfilePhotoRequest request, ServerCallContext context)
    {
        string resultMessage = "";

        List<Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.Id == request.Id);
        if (foundProfile == null)
        {
            resultMessage = "El perfil no existe";
            Logger.Instance.WriteWarning(resultMessage);
            return Task.FromResult(new ProfileResponse
            {
                Code = 404,
                Message = resultMessage
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
            resultMessage = "Error al eliminar foto";
            Logger.Instance.WriteError(resultMessage);
            return Task.FromResult(new ProfileResponse
            {
                Code = 500,
                Message = resultMessage
            });
        }

        Persistence.Instance.RemoveProfilePhoto(foundProfile.Id);

        resultMessage = "Eliminada correctamente";
        Logger.Instance.WriteInfo(resultMessage);
        return Task.FromResult(new ProfileResponse
        {
            Code = 200,
            Message = resultMessage
        });
    }
}

