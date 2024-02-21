namespace TrashTracker.Web.Models.EnumModels
{
    public abstract class EnumTable
    {
        public Int32 Id { get; set; }
        public String Value { get; set; } = null!;
    }
}
