using Ardalis.GuardClauses;
using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Handlers;

public class RequestAcceptedHandler : IHandler<string, MyConnection>
{
    private readonly ConnectionMultiplexer redis;
    private readonly UnitOfWork unitOfWork;

    public RequestAcceptedHandler(ConnectionMultiplexer redis, UnitOfWork unit)
    {
        this.redis = redis;
        this.unitOfWork = unit;
    }

    public async Task<MyConnection> Handle(string handled, params object[] additionalParams)
    {
        Guard.Against.NullOrEmpty(handled, nameof(handled));
        Guard.Against.NullOrEmpty<object>(additionalParams, nameof(additionalParams));

        if (additionalParams[0] is not string)
            throw new ArgumentException($"{nameof(RequestAcceptedHandler)} needs patient username as additional parameter", nameof(additionalParams));

        Doctor doctor = await unitOfWork.DoctorRepository.GetUser(handled);
        Patient patient = await unitOfWork.PatientRepository.GetUser(additionalParams[0] as string);

        IDatabase db = redis.GetDatabase();

        var patientConnection = new MyConnection { Username = doctor.Username, FullName = doctor.FullName };
        var doctorConnection = new MyConnection { Username = patient.Username, FullName = patient.FullName };

        await db.ListLeftPushAsync(doctor.PatientListKey,
            JsonSerializer.Serialize<MyConnection>(patientConnection));

        await db.ListLeftPushAsync(patient.MyDoctorsListKey,
            JsonSerializer.Serialize<MyConnection>(doctorConnection));

        return patientConnection;
    }
}
