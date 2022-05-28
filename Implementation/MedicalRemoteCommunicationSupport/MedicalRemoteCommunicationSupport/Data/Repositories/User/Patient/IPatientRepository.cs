using MedicalRemoteCommunicationSupport.Filtering;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public interface IPatientRepository: IUserRepository<Patient>
{
    Task<Patient> UpdatePatient(Patient patient);

    Task<IEnumerable<Patient>> Search(PatientCriteria criteria);

    Task RegisterTopicForUser(string username, int id);

    Task UnregisterTopicForUser(string username, int id);
}
