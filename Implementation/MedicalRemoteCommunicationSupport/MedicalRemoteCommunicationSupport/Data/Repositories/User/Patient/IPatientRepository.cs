namespace MedicalRemoteCommunicationSupport.Data.Repositories;

public interface IPatientRepository: IUserRepository<Patient>
{
    Task<Patient> UpdatePatient(Patient patient);
}
