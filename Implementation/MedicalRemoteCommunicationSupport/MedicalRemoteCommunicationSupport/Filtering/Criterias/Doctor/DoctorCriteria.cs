using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

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
            BsonRegularExpression exp = new BsonRegularExpression(new Regex($"^({Username}).*", RegexOptions.IgnoreCase));
            filter &= builder.Regex(nameof(Doctor.Username), exp);
        }
        if (!string.IsNullOrWhiteSpace(Name))
        {
            BsonRegularExpression exp = new BsonRegularExpression(new Regex($"^({Name}).*", RegexOptions.IgnoreCase));
            filter &= builder.Regex(nameof(Doctor.Name), exp);
        }
        if (!string.IsNullOrWhiteSpace(Surname))
        {
            BsonRegularExpression exp = new BsonRegularExpression(new Regex($"^({Surname}).*", RegexOptions.IgnoreCase));
            filter &= builder.Regex(nameof(Doctor.Surname), exp);
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
