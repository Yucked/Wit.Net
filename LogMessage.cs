namespace Wit.Net
{
    using System;
    /// <summary>Event Log Message.</summary>
    public class LogMessage : EventArgs
    {
        public LogMessage(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }
        /// <summary>Standard message if <see cref="Exception"/> isn't null.</summary>
        public string Message { get; internal set; }
        /// <summary><see cref="Message"/> will be null if <see cref="Exception"/> isn't null.</summary>
        public Exception Exception { get; internal set; }
    }
}