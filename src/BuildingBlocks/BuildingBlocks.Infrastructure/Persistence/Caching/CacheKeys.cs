namespace BuildingBlocks.Infrastructure.Persistence.Caching
{
    public static class CacheKeys
    {
        public static string Users() => "users:all";
        public static string UserById(Guid userId) => $"users:{userId}";
        public static string Roles() => "roles:all";
        public static string RoleById(Guid roleId) => $"roles:{roleId}";
        public static string UserAddresses(Guid userId) => $"users:{userId}:addresses";
        public static string UserAddressById(Guid userId, Guid addressId) => $"users:{userId}:addresses:{addressId}";
        public static string Products() => "products:all";
        public static string ProductById(Guid productId) => $"products:{productId}";
        public static string ProductsByCategoryId(Guid categoryId) => $"products:category:{categoryId}";
        public static string Categories() => "categories:all";
        public static string CategoryById(Guid categoryId) => $"categories:{categoryId}";
    }
}