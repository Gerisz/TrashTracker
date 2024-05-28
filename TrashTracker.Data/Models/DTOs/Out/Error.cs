namespace TrashTracker.Data.Models.DTOs.Out
{
    /// <summary>
    /// DTO meant to contain error messages.
    /// </summary>
    public class Error
    {
        public String? RequestId { get; set; }
        public Boolean ShowRequestId => !String.IsNullOrEmpty(RequestId);
    }
}
