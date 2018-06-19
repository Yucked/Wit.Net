namespace WitSharp.Models
{
    public enum AudioType
    {
        /// <summary>
        /// RIFF WAVE is supported. (Linear, Little-Endian)
        /// </summary>
        WAV,
        /// <summary>
        /// For MP3 File or Stream.
        /// </summary>
        MPEG3,
        /// <summary>
        /// For G.711 U-Law or stream. Sampling rate must bt 8khz.
        /// </summary>
        ULAW
    }
}