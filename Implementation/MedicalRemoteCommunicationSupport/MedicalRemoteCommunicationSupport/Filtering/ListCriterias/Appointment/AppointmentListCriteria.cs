using MedicalRemoteCommunicationSupport.Helpers.Validators;

namespace MedicalRemoteCommunicationSupport.Filtering;

public class AppointmentListCriteria : IListCriteria<Appointment>
{
    public string Patient { get; set; }
    public string Doctor { get; set; }
    public string ScheduledTime { get; set; }

    public IEnumerable<Appointment> Filter(IEnumerable<Appointment> data)
    {
        return data.Where(a => ApplyFilter<string>(a.Patient.FullName, Patient)
                    && ApplyFilter<string>(a.Doctor.FullName, Doctor));
    }

    private bool ApplyFilter<T>(T dataValue, T? criteriaValue) where T: IEquatable<T>
    {
        var type = typeof(T);
        if (TypeValidations.IsNullable<T>())
        {
            if (dataValue is string dataString)
            {
                return criteriaValue is null || dataString.ToLower().Contains((criteriaValue as string).ToLower());
            }
            return criteriaValue is null || dataValue.Equals(criteriaValue);
        }

        return criteriaValue.Equals(default(T)) || dataValue.Equals(criteriaValue);
    }
}
