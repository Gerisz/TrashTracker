namespace TrashTracker.Web
{

    /// <summary>
    /// Class to extract Trashout's login secrets into
    /// </summary>
    public class LoginSecrets
    {
        /// <summary>
        /// E-mail address used to log in to Trashout
        /// </summary>
        public static String Email { get; set; } = null!;

        /// <summary>
        /// Password used to log in to Trashout
        /// </summary>
        public static String Password { get; set; } = null!;

        /// <summary>
        /// Google API used to log in to Trashout
        /// </summary>
        public static String GoogleAPIKey { get; set; } = null!;

        /// <summary>
        /// Determines whether login secrets are complete.
        /// </summary>
        /// <returns><see langword="true"/> if login secrets are complete; otherwise, <see langword="false"/></returns>
        public static bool IncompleteLoginSecrets()
        {
            return Email == null || Password == null || GoogleAPIKey == null;
        }
    }
}
