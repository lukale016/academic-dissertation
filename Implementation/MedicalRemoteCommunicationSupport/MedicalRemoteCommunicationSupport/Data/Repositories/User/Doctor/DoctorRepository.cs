using MongoDB.Driver;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public class DoctorRepository : UserRepository<Doctor>, IDoctorRepository
{
    public DoctorRepository(IMongoDatabase mongoDb): base(mongoDb)
    { }

    public Task<Doctor> UpdateDoctor(Doctor doctor)
    {
        throw new NotImplementedException();
    }
}
