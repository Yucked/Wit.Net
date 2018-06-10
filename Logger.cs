namespace Wit.Net
{
    using System;
    public sealed class Logger
    {
        internal Logger() { }
        public event Action<string, Exception> Log;
        internal static Logger Logging { get; } = new Logger();
        internal void Send(string message = null, Exception exception = null)
        {
            switch (WitClient.Config.LogSeverity)
            {
                case LogSeverity.EXCEPTIONS: throw exception;
                case LogSeverity.INFO: Log?.Invoke(message, exception); break;
            }
        }
    }
}