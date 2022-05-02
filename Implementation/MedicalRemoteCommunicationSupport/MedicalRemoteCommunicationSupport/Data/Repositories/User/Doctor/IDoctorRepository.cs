namespace MedicalRemoteCommunicationSupport.Data.Repositories;
public interface IDoctorRepository: IUserRepository<Doctor>
{
    Task<Doctor> UpdateDoctor(Doctor doctor);
}
