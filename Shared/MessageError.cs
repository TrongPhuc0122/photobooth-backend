namespace Shared
{
    public class MessageError
    {
        private readonly string _errorCode;

        private static readonly Dictionary<string, string> _messages = new()
        {
            
        };

        public MessageError(string message)
        {
            _errorCode = _messages.TryGetValue(message, out var code)
                ? code
                : "UNKNOWN";
        }

        public static implicit operator string(MessageError msg)
        {
            return msg._errorCode;
        }
    }
}