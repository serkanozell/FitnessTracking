namespace FitnessTracking.Web.Models
{
    public sealed class AddSplitRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}