namespace AspNetCore.Identity.Dapper
{
    public static class IdentityDapperTables
    {
        public static class DbTable
        {
            public const string Users = "AspNetUsers";
            public const string UserRoles = "AspNetUserRoles";
            public const string UserClaims = "AspNetUserClaims";
            public const string UserLogins = "AspNetUserLogins";
            public const string UserTokens = "AspNetUserTokens";
            
            public const string Roles = "AspNetRoles";
            public const string RoleClaims = "AspNetRoleClaims";
        }
    }
}
