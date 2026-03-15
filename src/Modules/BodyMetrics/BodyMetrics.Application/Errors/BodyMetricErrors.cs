namespace BodyMetrics.Application.Errors
{
    public static class BodyMetricErrors
    {
        public static Error NotFound(Guid id) =>
            new("BodyMetric.NotFound", $"Body metric with ID '{id}' was not found.");

        public static Error AlreadyActive(Guid id) =>
            new("BodyMetric.AlreadyActive", $"Body metric with ID '{id}' is already active.");

        public static Error AlreadyDeleted(Guid id) =>
            new("BodyMetric.AlreadyDeleted", $"Body metric with ID '{id}' is already deleted.");
    }
}