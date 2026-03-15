using BuildingBlocks.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BuildingBlocks.Infrastructure.Services
{
    public sealed class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string? _systemActorOverride;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId
        {
            get
            {
                if (_systemActorOverride is not null)
                    return _systemActorOverride;

                if (!IsAuthenticated)
                    return null;
                return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
        }

        public bool IsAuthenticated =>
            _systemActorOverride is not null ||
            (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false);

        public bool IsAdmin => _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;

        public void SetSystemActor(string actorId) => _systemActorOverride = actorId;

        public void ClearSystemActor() => _systemActorOverride = null;
    }
}