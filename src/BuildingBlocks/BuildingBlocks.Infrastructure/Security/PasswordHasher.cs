using BuildingBlocks.Domain.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly int _workFactor;

    public PasswordHasher(int workFactor = 12)
    {
        _workFactor = workFactor;
    }

    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password, _workFactor);
    }

    public bool Verify(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}