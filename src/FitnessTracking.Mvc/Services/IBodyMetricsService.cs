using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public interface IBodyMetricsService
{
    Task<PagedResult<BodyMetricDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<BodyMetricDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(BodyMetricEditModel model, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, BodyMetricEditModel model, CancellationToken cancellationToken = default);
    Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
