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
        List<User> users = Persistence.Instance.GetUsers();
        User? foundUser = users.Find((u) => u.Username == request.Username);
        if (foundUser != null)
        {
            return Task.FromResult(new UserResponse
            {
                Code = 403,
                Message = "Ya existe un usuario con ese nombre de usuario"
            });
        }

        Persistence.Instance.AddUser(new User
        {
            Username = request.Username,
            Password = request.Password
        });

        return Task.FromResult(new UserResponse
        {
            Code = 1,
            Message = "Agregado correctamente"
        });
    }

    public override Task<UserResponse> Update(UpdateUserRequest request, ServerCallContext context)
    {
        List<User> users = Persistence.Instance.GetUsers();
        User? foundUser= users.Find((u) => u.Id == request.Id);
        if (foundUser == null)
        {
            return Task.FromResult(new UserResponse
            {
                Code = 404,
                Message = "El usuario no existe"
            });
        }

        Persistence.Instance.UpdateUser(new User
        {
            Id = request.Id,
            Username = request.Username,
            Password = request.Password
        });

        return Task.FromResult(new UserResponse
        {
            Code = 200,
            Message = "Actualizado correctamente"
        });
    }

    public override Task<UserResponse> Remove(RemoveUserRequest request, ServerCallContext context)
    {
        List<User> users = Persistence.Instance.GetUsers();
        User? foundUser = users.Find((u) => u.Id == request.Id);
        if (foundUser == null)
        {
            return Task.FromResult(new UserResponse
            {
                Code = 404,
                Message = "El usuario no existe"
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
                return Task.FromResult(new UserResponse
                {
                    Code = 500,
                    Message = "Error al eliminar foto"
                });
            }

            Persistence.Instance.RemoveProfile(foundProfile);
        }

        Persistence.Instance.RemoveUser(foundUser);

        return Task.FromResult(new UserResponse
        {
            Code = 200,
            Message = "Actualizado correctamente"
        });
    }
}

