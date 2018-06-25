namespace WitSharp.Models
{
    public enum AudioType
    {
        /// <summary>
        /// RIFF WAVE is supported. (Linear, Little-Endian)
        /// </summary>
        Wav,
        /// <summary>
        /// For MP3 File or Stream.
        /// </summary>
        Mpeg3,
        /// <summary>
        /// For G.711 U-Law or stream. Sampling rate must bt 8khz.
        /// </summary>
        Ulaw
    }
}