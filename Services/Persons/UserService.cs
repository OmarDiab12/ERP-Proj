using Azure;
using ERP.DTOs.User;
using ERP.Helpers.JWT;
using ERP.Models;
using ERP.Repositories.Interfaces.Persons;
using ERP.Services.Interfaces.Persons;
using Microsoft.AspNetCore.Identity;

namespace ERP.Services.Persons
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher<User> _hasher;
        private readonly IJWTHelper _jwt;
        private readonly IErrorRepository _errorRepository;

        public UserService(
            IUserRepository users,
            IPasswordHasher<User> hasher,
            IJWTHelper jwt,
            IErrorRepository errorRepository)
        {
            _users = users;
            _hasher = hasher;
            _jwt = jwt;
            _errorRepository = errorRepository;
        }

        public async Task<ResponseDTO> RegisterAsync(RegisterUserDto dto, int? currentUserId = null)
        {
            const string fn = nameof(RegisterAsync);
            try
            {
                if (await _users.EmailExistsAsync(dto.Email))
                    return new ResponseDTO { IsValid = false, Message = "Email already exists" };

                if (await _users.UserNameExistsAsync(dto.FullName))
                    return new ResponseDTO { IsValid = false, Message = "Username already exists" };

                var user = new User
                {
                    FullName = dto.FullName.Trim(),
                    DisplayName = dto.DisplayName.Trim(),
                    PhoneNumber = dto.Phone.Trim(),
                    Email = dto.Email.Trim().ToLower(),
                    CreatedAt = DateTime.UtcNow
                };

                user.PasswordHash = _hasher.HashPassword(user, dto.Password);

                await _users.CreateAsync(user,currentUserId ?? 0);

                return new ResponseDTO { IsValid = true, Message = "Registered successfully" };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", currentUserId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while registering user." };
            }
        }

        public async Task<ResponseDTO> LoginAsync(LoginDto dto, int? currentUserId = null)
        {
            const string fn = nameof(LoginAsync);
            try
            {
                var user = await _users.GetByEmailAsync(dto.Email.Trim().ToLower());
                if (user is null)
                    return new ResponseDTO { IsValid = false, Message = "Invalid credentials" };

                var check = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
                if (!check)
                    return new ResponseDTO { IsValid = false, Message = "Invalid credentials" };

                var token = _jwt.GenerateToken(user);
                var userDto = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    DisplayName = user.DisplayName,
                    Phone = user.PhoneNumber,
                    Email = user.Email,
                };

                return new ResponseDTO
                {
                    IsValid = true,
                    Data = new { token, user = userDto },
                    Message = "Login successful"
                };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", currentUserId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while logging in." };
            }
        }

        public async Task<ResponseDTO> GetByIdAsync(int id, int? currentUserId = null)
        {
            const string fn = nameof(GetByIdAsync);
            try
            {
                var u = await _users.GetByIdAsync(id);
                if (u is null)
                    return new ResponseDTO { IsValid = false, Message = "User not found" };

                var dto = new UserDto { Id = u.Id, FullName = u.FullName, Email = u.Email, DisplayName =u.DisplayName , Phone = u.PhoneNumber };
                return new ResponseDTO { IsValid = true, Data = dto };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", currentUserId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while fetching user." };
            }
        }

        public async Task<ResponseDTO> GetPagedAsync(int page, int pageSize, int? currentUserId = null)
        {
            const string fn = nameof(GetPagedAsync);
            try
            {
                var list = await _users.GetPagedAsync(page, pageSize);
                var data = list.Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    DisplayName = u.DisplayName,
                    Phone = u.PhoneNumber,
                    Email = u.Email,
                });

                return new ResponseDTO { IsValid = true, Data = data };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", currentUserId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while listing users." };
            }
        }
    }
}
