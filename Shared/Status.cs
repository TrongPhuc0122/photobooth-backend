namespace Shared;

public enum Status
{
    ERROR=0,
    ONLINE,
    OFFLINE, 
    CONNECTED,
    DISCONNECTED
}

public class StatusExtenTion
{
    public string StatusToText(Status status)
    {
        return status switch
        {
            Status.ERROR => "Error",
            Status.ONLINE => "Online",
            Status.OFFLINE => "Offline",
            Status.CONNECTED => "Connected",
            Status.DISCONNECTED => "Disconnected",
            _ => "Undefined"
        };
    }
}