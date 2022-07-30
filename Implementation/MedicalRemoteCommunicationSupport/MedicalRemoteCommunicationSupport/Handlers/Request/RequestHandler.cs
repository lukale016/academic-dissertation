using Ardalis.GuardClauses;
using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Handlers;

public class RequestHandler : IHandler<string, RequestDto>
{
    private readonly ConnectionMultiplexer redis;
    private readonly UnitOfWork unitOfWork;

    public RequestHandler(ConnectionMultiplexer redis, UnitOfWork unit)
    {
        this.redis = redis;
        this.unitOfWork = unit;
    }

    public async Task<RequestDto> Handle(string handled, params object[] additionalParams)
    {
        Guard.Against.NullOrEmpty<object>(additionalParams, nameof(additionalParams));
        Guard.Against.NullOrWhiteSpace(handled, nameof(handled));
        if (additionalParams[0] is not string)
            throw new ArgumentException("Request handler takes sender as additional parameter", nameof(additionalParams));

        string senderUsername = additionalParams[0] as string;

        Patient patient = await unitOfWork.PatientRepository.GetUser(senderUsername);
        Doctor doctor = await unitOfWork.DoctorRepository.GetUser(handled);

        var patientRequest = new RequestDto { Username = doctor.Username, FullName = doctor.FullName };
        var doctorRequest = new RequestDto { Username = patient.Username, FullName = patient.FullName };

        IDatabase db = redis.GetDatabase();
        await db.ListLeftPushAsync(patient.MyDoctorsListKey, JsonSerializer.Serialize<RequestDto>(patientRequest));
        await db.ListLeftPushAsync(doctor.PatientListKey, JsonSerializer.Serialize<RequestDto>(doctorRequest));

        return doctorRequest;
    }
}
