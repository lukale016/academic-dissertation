namespace MedicalRemoteCommunicationSupport.Handlers;

public interface IHandler<HANDLED, RETURNED>
{
    Task<RETURNED> Handle(HANDLED handled, params object[] additionalParams);
}
