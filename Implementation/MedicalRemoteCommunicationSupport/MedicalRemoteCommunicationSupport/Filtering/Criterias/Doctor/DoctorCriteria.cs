using MongoDB.Driver;

namespace MedicalRemoteCommunicationSupport.Filtering;

public class DoctorCriteria: ICriteria<Doctor>
{
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Gender { get; set; }
    public string Specialization { get; set; }

    public FilterDefinition<Doctor> BuildFilter()
    {
        FilterDefinitionBuilder<Doctor> builder = Builders<Doctor>.Filter;
        var filter = builder.Eq(nameof(Doctor.IsDoctor), true);

        if (!string.IsNullOrWhiteSpace(Username))
        {
            filter &= builder.Eq(nameof(Doctor.Username), Username);
        }
        if (!string.IsNullOrWhiteSpace(Name))
        {
            filter &= builder.Eq(nameof(Doctor.Name), Name);
        }
        if (!string.IsNullOrWhiteSpace(Surname))
        {
            filter &= builder.Eq(nameof(Doctor.Surname), Surname);
        }
        if (!string.IsNullOrWhiteSpace(Gender))
        {
            filter &= builder.Eq(nameof(Doctor.Gender), Gender);
        }
        if (!string.IsNullOrWhiteSpace(Specialization))
        {
            filter &= builder.Eq(nameof(Doctor.Specialization), Specialization);
        }

        return filter;
    }
}
