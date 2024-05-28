namespace TrashTracker.Data.Models.DTOs.In
{
    /// <summary>
    /// DTO meant to contain a token received from Google when authenticating for TrashOut.
    /// </summary>
    public class TokenFromGoogle
    {
        /// <summary>
        /// The id token recieved from Google.
        /// </summary>
        public String IdToken { get; set; } = null!;
    }
}
