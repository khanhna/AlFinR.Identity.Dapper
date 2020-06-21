using System;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Entities
{
    [Table(IdentityDapperTables.DbTable.RoleClaims)]
    public class IdentityRoleClaimDapper<TKey> : IdentityRoleClaim<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    [Table(IdentityDapperTables.DbTable.RoleClaims)]
    public class IdentityRoleClaimDapper : IdentityRoleClaim<Guid>
    {
        [Key]
        public override int Id { get; set; }
    }
}
