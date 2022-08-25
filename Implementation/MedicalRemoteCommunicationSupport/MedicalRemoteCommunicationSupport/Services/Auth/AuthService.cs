using Ardalis.GuardClauses;
using MedicalRemoteCommunicationSupport.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MedicalRemoteCommunicationSupport.Services;

public class AuthService : IAuthService
{
    private readonly UnitOfWork unitOfWork;
    private readonly IOptions<JwtSettings> jwtConfig;
    public AuthService(UnitOfWork unit, IOptions<JwtSettings> jwtSettings)
    {
        unitOfWork = unit;
        this.jwtConfig = jwtSettings;
    }

    public async Task<UserAndToken> Login(Credentials creds)
    {
        Guard.Against.NullOrWhiteSpace(creds.Username);
        Guard.Against.NullOrWhiteSpace(creds.Password);

        try
        {
            Patient patient = await unitOfWork.PatientRepository.GetUser(creds.Username);
            
            if(patient is not null && patient.Password != creds.Password)
            {
                throw new ResponseException(StatusCodes.Status401Unauthorized, "Passwords do not match");
            }

            return new UserAndToken(patient, BuildToken(patient));
        }
        catch(ResponseException ex)
        {
            if (ex.Status == StatusCodes.Status404NotFound || ex.Status == StatusCodes.Status500InternalServerError)
            {
                Doctor doctor = await unitOfWork.DoctorRepository.GetUser(creds.Username);

                if (doctor is not null && doctor.Password != creds.Password)
                {
                    throw new ResponseException(StatusCodes.Status401Unauthorized, "Passwords do not match");
                }

                return new UserAndToken(doctor, BuildToken(doctor));
            }
            else
            {
                throw;
            }
        }
    }

    private string BuildToken(UserBase user)
    {
        Guard.Against.Null(user, "No user to build token for");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Value.SigningKey));
        var signingCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(jwtConfig.Value.Issuer, 
            jwtConfig.Value.Audiance, 
            claims, 
            expires: DateTime.Now.AddHours(2), 
            signingCredentials: signingCreds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
