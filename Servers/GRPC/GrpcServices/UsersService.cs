using Google.Protobuf;
using Grpc.Core;
using GrpcServer;
using Shared;
using GrpcServer.Logs;

namespace GrpcServer.Services;

public class UsersService : Users.UsersBase
{
    public override Task<UserResponse> Add(AddUserRequest request, ServerCallContext context)
    {
        string resultMessage = "";

        List<User> users = Persistence.Instance.GetUsers();
        User? foundUser = users.Find((u) => u.Username == request.Username);
        if (foundUser != null)
        {
            resultMessage = "Ya existe un usuario con ese nombre de usuario";
            Logger.Instance.WriteWarning(resultMessage);
            return Task.FromResult(new UserResponse
            {
                Code = 403,
                Message = resultMessage
            });
        }

        int id = Persistence.Instance.AddUser(new User
        {
            Username = request.Username,
            Password = request.Password
        });

        resultMessage = "Agregado correctamente id: " + id;
        Logger.Instance.WriteInfo(resultMessage);
        return Task.FromResult(new UserResponse
        {
            Code = 1,
            Message = resultMessage
        });
    }

    public override Task<UserResponse> Update(UpdateUserRequest request, ServerCallContext context)
    {
        string resultMessage = "";

        List<User> users = Persistence.Instance.GetUsers();
        User? foundUser= users.Find((u) => u.Id == request.Id);
        if (foundUser == null)
        {
            resultMessage = "El usuario no existe";
            Logger.Instance.WriteWarning(resultMessage);
            return Task.FromResult(new UserResponse
            {
                Code = 404,
                Message = resultMessage
            });
        }

        Persistence.Instance.UpdateUser(new User
        {
            Id = request.Id,
            Username = request.Username,
            Password = request.Password
        });

        resultMessage = "Actualizado correctamente";
        Logger.Instance.WriteInfo(resultMessage);
        return Task.FromResult(new UserResponse
        {
            Code = 200,
            Message = resultMessage
        });
    }

    public override Task<UserResponse> Remove(RemoveUserRequest request, ServerCallContext context)
    {
        string resultMessage = "";

        List<User> users = Persistence.Instance.GetUsers();
        User? foundUser = users.Find((u) => u.Id == request.Id);
        if (foundUser == null)
        {
            resultMessage = "El usuario no existe";
            Logger.Instance.WriteWarning(resultMessage);
            return Task.FromResult(new UserResponse
            {
                Code = 404,
                Message = resultMessage
            });
        }

        List<Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.UserId == request.Id);
        if (foundProfile != null)
        {
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
                return Task.FromResult(new UserResponse
                {
                    Code = 500,
                    Message = resultMessage
                });
            }

            Persistence.Instance.RemoveProfile(foundProfile);
        }

        Persistence.Instance.RemoveUser(foundUser);

        resultMessage = "Eliminado correctamente";
        Logger.Instance.WriteInfo(resultMessage);
        return Task.FromResult(new UserResponse
        {
            Code = 200,
            Message = resultMessage
        });
    }
}
