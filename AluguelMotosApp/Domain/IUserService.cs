using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IUserService
    {
        Task<string> RegisterUserAsync(string email, string password, string role);
        Task<string?> LoginAsync(string email, string password);
    }
}
