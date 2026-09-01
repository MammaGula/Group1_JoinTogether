using JoinTogether.Shared.DTOs;

namespace JoinTogether.BLL.Interfaces;

public interface IAuthService
{
    // US-06: register a new account
    Task<(bool Success, string? Error, AuthResponseDto? Result)> RegisterAsync(RegisterDto dto);

    // US-06: log in with an existing account
    Task<(bool Success, string? Error, AuthResponseDto? Result)> LoginAsync(LoginDto dto);
}