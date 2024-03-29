﻿using Ardalis.GuardClauses;
using StackExchange.Redis;
using System.Text.Json;

namespace MedicalRemoteCommunicationSupport.Handlers;

public class RequestAcceptedHandler : IHandler<string, ((string fullName, string skypeId) pattientAdditionalData, MyConnection connection)>
{
    private readonly ConnectionMultiplexer redis;
    private readonly UnitOfWork unitOfWork;

    public RequestAcceptedHandler(ConnectionMultiplexer redis, UnitOfWork unit)
    {
        this.redis = redis;
        this.unitOfWork = unit;
    }

    public async Task<((string fullName, string skypeId) pattientAdditionalData, MyConnection connection)> Handle(string handled, params object[] additionalParams)
    {
        Guard.Against.NullOrEmpty(handled, nameof(handled));
        Guard.Against.NullOrEmpty<object>(additionalParams, nameof(additionalParams));

        if (additionalParams[0] is not string)
            throw new ArgumentException($"{nameof(RequestAcceptedHandler)} needs patient username as additional parameter", nameof(additionalParams));

        Doctor doctor = await unitOfWork.DoctorRepository.GetUser(additionalParams[0] as string);
        Patient patient = await unitOfWork.PatientRepository.GetUser(handled);

        IDatabase db = redis.GetDatabase();

        var patientConnection = new MyConnection
            { Username = doctor.Username, FullName = doctor.FullName, SkypeId = doctor.SkypeId };
        var doctorConnection = new MyConnection
            { Username = patient.Username, FullName = patient.FullName, SkypeId = patient.SkypeId };

        await db.ListLeftPushAsync(doctor.PatientListKey,
            JsonSerializer.Serialize<MyConnection>(doctorConnection));

        await db.ListLeftPushAsync(patient.MyDoctorsListKey,
            JsonSerializer.Serialize<MyConnection>(patientConnection));

        await db.ListRemoveAsync(patient.SentRequestsListKey, JsonSerializer.Serialize<PatientRequestDto>(
            new PatientRequestDto { Username = doctor.Username, Specialization = doctor.Specialization }));
        await db.ListRemoveAsync(doctor.RequestListKey, JsonSerializer.Serialize<DoctorRequestDto>(
            new DoctorRequestDto { Username = patient.Username, FullName = patient.FullName }));

        return ((patient.FullName, patient.SkypeId), patientConnection);
    }
}
