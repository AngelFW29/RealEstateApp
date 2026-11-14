using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Core.Application;
using RealEstateApp.Core.Application.Dtos;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class AccountServiceForWebApi : IApiAccountService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtSettings _jwtSettings;

    public AccountServiceForWebApi(UserManager<AppUser> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResult> AuthenticateAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.UserNameOrEmail) ?? await _userManager.FindByNameAsync(dto.UserNameOrEmail);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return new AuthResult { StatusCode = 401, Message = "Los datos de acceso son inválidos." };

        if (!user.IsActive)
            return new AuthResult { StatusCode = 401, Message = "El usuario se encuentra inactivo y no puede autenticarse." };

        var roles = await _userManager.GetRolesAsync(user);
        // P1: Chequea esta validación, porque impide que el usuario con rol cliente pueda autenticarse en la Api y obtener JWT.
        if (!roles.Contains("Administrador") && !roles.Contains("Desarrollador"))
            return new AuthResult { StatusCode = 403, Message = "Acceso denegado. Su usuario no tiene permisos para acceder a la API." };

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("uid", user.Id)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new AuthResult
        {
            StatusCode = 200,
            Data = new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserName = user.UserName!,
                Roles = roles.ToList(),
                Expiration = expires
            }
        };
    }

    public async Task<Result> RegisterAsync(RegisterUserDto dto, string role)
    {
        var user = new AppUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IdCard = dto.IdCard,
            Email = dto.Email,
            UserName = dto.UserName,
            IsActive = true,
            EmailConfirmed = true
        };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return Result.Fail(result.Errors.Select(e => e.Description).ToArray());

        await _userManager.AddToRoleAsync(user, role);
        return Result.Ok("Usuario creado correctamente.");
    }
}