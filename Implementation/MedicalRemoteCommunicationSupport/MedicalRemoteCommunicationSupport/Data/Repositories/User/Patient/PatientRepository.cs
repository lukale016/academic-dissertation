using MongoDB.Driver;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public class PatientRepository : UserRepository<Patient>, IPatientRepository
{
    public PatientRepository(IMongoDatabase mongoDb): base(mongoDb)
    { }

    public Task<Patient> UpdatePatient(Patient patient)
    {
        throw new NotImplementedException();
    }
}
