using Dashboard.Application.Dtos;

namespace Dashboard.Application.Features.Dashboard.GetDashboard;

public sealed record GetDashboardQuery : IQuery<Result<DashboardDto>>;