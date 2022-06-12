namespace MedicalRemoteCommunicationSupport.Services;

public class AuthService : IAuthService
{
    private readonly UnitOfWork unitOfWork;
    public AuthService(UnitOfWork unit)
    {
        unitOfWork = unit;
    }

    public async Task<object> LogIn(Credentials creds)
    {
        if(string.IsNullOrEmpty(creds.Username) || string.IsNullOrEmpty(creds.Password))
        {
            throw new ResponseException(StatusCodes.Status400BadRequest, "Parameters not set");
        }

        try
        {
            Patient patient = await unitOfWork.PatientRepository.GetUser(creds.Username);
            
            if(patient is not null && patient.Password != creds.Password)
            {
                throw new ResponseException(StatusCodes.Status401Unauthorized, "Passwords do not match");
            }

            return patient;
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

                return doctor;
            }
            else
            {
                throw;
            }
        }

        return null;
    }
}
