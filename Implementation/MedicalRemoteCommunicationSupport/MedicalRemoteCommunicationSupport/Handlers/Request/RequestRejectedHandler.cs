using Ardalis.GuardClauses;
using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Handlers;

public class RequestRejectedHandler : IHandler<string, RequestRejectionData>
{
    private readonly ConnectionMultiplexer redis;
    private readonly UnitOfWork unitOfWork;

    public RequestRejectedHandler(ConnectionMultiplexer redis, UnitOfWork unit)
    {
        this.redis = redis;
        this.unitOfWork = unit;
    }

    public async Task<RequestRejectionData> Handle(string handled, params object[] additionalParams)
    {
        Guard.Against.NullOrEmpty(handled, nameof(handled));
        Guard.Against.NullOrEmpty<object>(additionalParams, nameof(additionalParams));
        if (additionalParams[0] is not string)
            throw new ArgumentException("First parameter must be doctor username", nameof(additionalParams));

        Patient patient = await unitOfWork.PatientRepository.GetUser(handled);
        Doctor doctor = await unitOfWork.DoctorRepository.GetUser(additionalParams[0] as string);

        IDatabase db = redis.GetDatabase();
        await db.ListRemoveAsync(patient.SentRequestsListKey, doctor.Username);
        await db.ListRemoveAsync(doctor.RequestListKey, JsonSerializer.Serialize<RequestDto>(
            new RequestDto { Username = patient.Username, FullName = patient.FullName }));

        return new RequestRejectionData { RejectedUsername = patient.Username, DoctorFullName = doctor.FullName };
    }
}
