using MedicalRemoteCommunicationSupport.Filtering;

namespace MedicalRemoteCommunicationSupport.Data.Repositories;
public interface IDoctorRepository: IUserRepository<Doctor>
{
    Task<Doctor> UpdateDoctor(Doctor doctor);
    Task<IEnumerable<Doctor>> Search(DoctorCriteria criteria);
}
