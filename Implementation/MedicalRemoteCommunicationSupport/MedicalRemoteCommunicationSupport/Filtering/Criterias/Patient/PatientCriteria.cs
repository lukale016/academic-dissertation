using MongoDB.Driver;

namespace MedicalRemoteCommunicationSupport.Filtering;

public class PatientCriteria: ICriteria<Patient>
{
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Gender { get; set; }

    public FilterDefinition<Patient> BuildFilter()
    {
        FilterDefinitionBuilder<Patient> builder = Builders<Patient>.Filter;
        var filter = builder.Eq(nameof(Patient.IsDoctor), false);

        if(!string.IsNullOrWhiteSpace(Username))
        {
            filter &= builder.Eq(nameof(Patient.Username), Username);
        }
        if (!string.IsNullOrWhiteSpace(Name))
        {
            filter &= builder.Eq(nameof(Patient.Name), Name);
        }
        if (!string.IsNullOrWhiteSpace(Surname))
        {
            filter &= builder.Eq(nameof(Patient.Surname), Surname);
        }
        if (!string.IsNullOrWhiteSpace(Gender))
        {
            filter &= builder.Eq(nameof(Patient.Gender), Gender);
        }

        return filter;
    }
}
