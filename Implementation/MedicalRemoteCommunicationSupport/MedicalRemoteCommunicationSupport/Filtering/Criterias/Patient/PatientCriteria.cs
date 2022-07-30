using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

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
            BsonRegularExpression exp = new BsonRegularExpression(new Regex($"^({Username}).*", RegexOptions.IgnoreCase));
            filter &= builder.Regex(nameof(Patient.Username), exp);
        }
        if (!string.IsNullOrWhiteSpace(Name))
        {
            BsonRegularExpression exp = new BsonRegularExpression(new Regex($"^({Name}).*", RegexOptions.IgnoreCase));
            filter &= builder.Regex(nameof(Patient.Name), exp);
        }
        if (!string.IsNullOrWhiteSpace(Surname))
        {
            BsonRegularExpression exp = new BsonRegularExpression(new Regex($"^({Surname}).*", RegexOptions.IgnoreCase));
            filter &= builder.Regex(nameof(Patient.Surname), exp);
        }
        if (!string.IsNullOrWhiteSpace(Gender))
        {
            filter &= builder.Eq(nameof(Patient.Gender), Gender);
        }

        return filter;
    }
}
