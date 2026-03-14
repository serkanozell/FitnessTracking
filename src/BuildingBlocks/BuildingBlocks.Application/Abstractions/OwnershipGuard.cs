using BuildingBlocks.Application.Results;

namespace BuildingBlocks.Application.Abstractions
{
    public static class OwnershipGuard
    {
        public static Error? CheckOwnership(ICurrentUser currentUser, Guid resourceUserId)
        {
            if (currentUser.IsAdmin)
                return null;

            var userId = Guid.Parse(currentUser.UserId!);

            if (userId != resourceUserId)
                return Error.Forbidden();

            return null;
        }
    }
}
