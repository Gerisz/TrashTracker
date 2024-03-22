namespace TrashTracker.Data.Models.DTOs.Out
{
    public class Error
    {
        public String? RequestId { get; set; }
        public Boolean ShowRequestId => !String.IsNullOrEmpty(RequestId);
    }
}
