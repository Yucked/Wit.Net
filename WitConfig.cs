namespace Wit.Net
{
    /// <summary>Configuration for  Wit client.</summary>
    public class WitConfig
    {
        /// <summary>Your server access token.</summary>       
        public string AccessToken { get; set; }
        /// <summary>Throw when an exception occurs or log them in <see cref="Log"/> event. Some exceptions won't be swallowed.</summary>        
        public bool ThrowOnError { get; set; }        
    }
}