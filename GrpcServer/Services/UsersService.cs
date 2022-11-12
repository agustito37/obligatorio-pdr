using Google.Protobuf;
using Grpc.Core;
using GrpcServer;
using Shared;

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
        string message = "";

        List<User> users = Persistence.Instance.GetUsers();
        User? foundUser = users.Find((u) => u.Username == request.Username);
        if (foundUser != null)
        {
            message = "Ya existe un usuario con ese nombre de usuario";
            Logs.Logger.Instance.WriteError(message);
            return Task.FromResult(new UserResponse
            {
                Code = 403,
                Message = message
            });
        }

        Persistence.Instance.AddUser(new User
        {
            Username = request.Username,
            Password = request.Password
        });

        message = "Agregado correctamente";
        Logs.Logger.Instance.WriteMessage(message);
        return Task.FromResult(new UserResponse
        {
            Code = 1,
            Message = message
        });
    }

    public override Task<UserResponse> Update(UpdateUserRequest request, ServerCallContext context)
    {
        string message = "";

        List<User> users = Persistence.Instance.GetUsers();
        User? foundUser= users.Find((u) => u.Id == request.Id);
        if (foundUser == null)
        {
            message = "El usuario no existe";
            Logs.Logger.Instance.WriteError(message);
            return Task.FromResult(new UserResponse
            {
                Code = 404,
                Message = message
            });
        }

        Persistence.Instance.UpdateUser(new User
        {
            Id = request.Id,
            Username = request.Username,
            Password = request.Password
        });

        message = "Actualizado correctamente";
        Logs.Logger.Instance.WriteMessage(message);
        return Task.FromResult(new UserResponse
        {
            Code = 200,
            Message = message
        });
    }

    public override Task<UserResponse> Remove(RemoveUserRequest request, ServerCallContext context)
    {
        string message = "";

        List<User> users = Persistence.Instance.GetUsers();
        User? foundUser = users.Find((u) => u.Id == request.Id);
        if (foundUser == null)
        {
            message = "El usuario no existe";
            Logs.Logger.Instance.WriteError(message);
            return Task.FromResult(new UserResponse
            {
                Code = 404,
                Message = message
            });
        }

        List<Profile> profiles = Persistence.Instance.GetProfiles();
        Profile? foundProfile = profiles.Find((p) => p.Id == request.Id);
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
                message = "Error al eliminar foto";
                Logs.Logger.Instance.WriteError(message);
                return Task.FromResult(new UserResponse
                {
                    Code = 500,
                    Message = message
                });
            }

            Persistence.Instance.RemoveProfile(foundProfile);
        }

        Persistence.Instance.RemoveUser(foundUser);

        message = "Actualizado correctamente";
        Logs.Logger.Instance.WriteMessage(message);
        return Task.FromResult(new UserResponse
        {
            Code = 200,
            Message = message
        });
    }
}

