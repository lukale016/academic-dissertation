namespace MedicalRemoteCommunicationSupport.Handlers;

public interface IHandler<HANDLED>
{
    Task Handle(HANDLED handled, params object[] additionalParams);
}
