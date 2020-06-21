using System;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Entities
{
    [Table(IdentityDapperTables.DbTable.UserClaims)]
    public class IdentityUserClaimDapper<TKey> : IdentityUserClaim<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    [Table(IdentityDapperTables.DbTable.UserClaims)]
    public class IdentityUserClaimDapper : IdentityUserClaim<Guid>
    {
        [Key]
        public override int Id { get; set; }
    }
}
