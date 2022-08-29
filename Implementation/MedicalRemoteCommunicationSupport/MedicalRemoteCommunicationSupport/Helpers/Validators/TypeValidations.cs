namespace MedicalRemoteCommunicationSupport.Helpers.Validators;

public static class TypeValidations
{
    public static bool IsNullable<T>()
    {
        var type = typeof(T);
        return !type.IsValueType ? true : 
               Nullable.GetUnderlyingType(type) != null ? true : false;
    }
}
