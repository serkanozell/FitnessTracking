namespace Users.Domain.Constants
{
    public static class RoleConstants
    {
        public static readonly Guid AdminRoleId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
        public static readonly Guid MemberRoleId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");

        public const string Admin = "Admin";
        public const string Member = "Member";
    }
}