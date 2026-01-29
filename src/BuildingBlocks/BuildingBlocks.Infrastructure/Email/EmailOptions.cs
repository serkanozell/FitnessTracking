public sealed class EmailOptions
{
    public string Host { get; init; } = default!;
    public int Port { get; init; }
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
    public bool EnableSsl { get; init; }
    public string From { get; init; } = default!;
}