using Ardalis.GuardClauses;
using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Handlers;

public class RequestHandler : IHandler<string, (DoctorRequestDto request, string specialization)>
{
    private readonly ConnectionMultiplexer redis;
    private readonly UnitOfWork unitOfWork;

    public RequestHandler(ConnectionMultiplexer redis, UnitOfWork unit)
    {
        this.redis = redis;
        this.unitOfWork = unit;
    }

    public async Task<(DoctorRequestDto request, string specialization)> Handle(string handled, params object[] additionalParams)
    {
        Guard.Against.NullOrEmpty<object>(additionalParams, nameof(additionalParams));
        Guard.Against.NullOrWhiteSpace(handled, nameof(handled));
        if (additionalParams[0] is not string)
            throw new ArgumentException("Request handler takes sender as additional parameter", nameof(additionalParams));

        string senderUsername = additionalParams[0] as string;

        Patient patient = await unitOfWork.PatientRepository.GetUser(senderUsername);
        Doctor doctor = await unitOfWork.DoctorRepository.GetUser(handled);

        var doctorRequest = new DoctorRequestDto { Username = patient.Username, FullName = patient.FullName };
        var patientRequest = new PatientRequestDto { Username = doctor.Username, Specialization = doctor.Specialization };

        IDatabase db = redis.GetDatabase();
        await db.ListLeftPushAsync(patient.SentRequestsListKey, JsonSerializer.Serialize<PatientRequestDto>(patientRequest));
        await db.ListLeftPushAsync(doctor.RequestListKey, JsonSerializer.Serialize<DoctorRequestDto>(doctorRequest));

        return (doctorRequest, doctor.Specialization);
    }
}
