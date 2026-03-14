namespace BuildingBlocks.Application.Abstractions
{
    public interface ICurrentUser
    {
        string? UserId { get; }
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
    }
}