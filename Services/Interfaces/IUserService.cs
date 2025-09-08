using ERP.DTOs;
using ERP.DTOs.User;

namespace ERP.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDTO> RegisterAsync(RegisterUserDto dto, int? currentUserId = null);
        Task<ResponseDTO> LoginAsync(LoginDto dto, int? currentUserId = null);
        Task<ResponseDTO> GetByIdAsync(int id, int? currentUserId = null);
        Task<ResponseDTO> GetPagedAsync(int page, int pageSize, int? currentUserId = null);
    }
}
