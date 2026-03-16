using BuildingBlocks.Web;
using Dashboard.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Dashboard.Application.Features.Dashboard.GetDashboard
{
    public sealed class GetDashboardEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/dashboard", async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetDashboardQuery(), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Failed to load dashboard.");
            })
            .WithName("GetDashboard")
            .WithTags("Dashboard")
            .WithSummary("Gets dashboard summary for the current user")
            .WithDescription("Returns workout stats, active program, latest body metrics")
            .Produces<DashboardDto>(StatusCodes.Status200OK);
        }
    }
}