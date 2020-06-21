using AspNetCore.Identity.Dapper;
using AspNetCore.Identity.Dapper.Entities;
using Dapper.Contrib.Extensions;

namespace Demo.Identity.Dapper.Entities
{
    [Table(IdentityDapperTables.DbTable.Users)]
    public class ApplicationUser : IdentityUserDapper<int>
    {
        [Key]
        public override int Id { get; set; }

        public string Image { get; set; }
        [Write(false)] public string Password { get; set; }
        [Write(false)] public bool IsAdministrator { get; set; }
    }

    [Table(IdentityDapperTables.DbTable.UserRoles)]
    public class ApplicationUserRole : IdentityUserRoleDapper<int>
    {
    }

    [Table(IdentityDapperTables.DbTable.UserClaims)]
    public class ApplicationUserClaim : IdentityUserClaimDapper<int>
    {
    }

    [Table(IdentityDapperTables.DbTable.UserLogins)]
    public class ApplicationUserLogin : IdentityUserLoginDapper<int>
    {
    }

    [Table(IdentityDapperTables.DbTable.UserTokens)]
    public class ApplicationUserToken : IdentityUserTokenDapper<int>
    {
    }

    [Table(IdentityDapperTables.DbTable.Roles)]
    public class ApplicationRole : IdentityRoleDapper<int>
    {
        [Key]
        public override int Id { get; set; }
    }

    [Table(IdentityDapperTables.DbTable.RoleClaims)]
    public class ApplicationRoleClaim : IdentityRoleClaimDapper<int>
    {
    }
}
