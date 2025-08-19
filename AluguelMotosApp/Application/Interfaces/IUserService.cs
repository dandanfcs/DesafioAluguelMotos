
namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<string> RegistrarUsuarioAsync(string email, string password, string role);
        Task<string?> LoginAsync(string email, string password);
    }
}
