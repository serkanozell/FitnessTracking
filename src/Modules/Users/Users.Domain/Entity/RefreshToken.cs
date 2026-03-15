namespace Users.Domain.Entity
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiresOnUtc { get; private set; }
        public DateTime CreatedOnUtc { get; private set; }
        public DateTime? RevokedOnUtc { get; private set; }

        public bool IsExpired => DateTime.Now >= ExpiresOnUtc;
        public bool IsRevoked => RevokedOnUtc is not null;
        public bool IsActive => !IsExpired && !IsRevoked;

        private RefreshToken() { }

        public static RefreshToken Create(Guid userId, int expirationDays = 7)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = GenerateToken(),
                ExpiresOnUtc = DateTime.Now.AddDays(expirationDays),
                CreatedOnUtc = DateTime.Now
            };
        }

        public void Revoke()
        {
            RevokedOnUtc = DateTime.Now;
        }

        private static string GenerateToken()
        {
            var randomBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }
    }
}