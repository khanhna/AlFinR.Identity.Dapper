using System;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Entities
{
    [Table(IdentityDapperTables.DbTable.Roles)]
    public class IdentityRoleDapper<TKey> : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    [Table(IdentityDapperTables.DbTable.Roles)]
    public class IdentityRoleDapper : IdentityRole<Guid>
    {
        [Key]
        public override Guid Id { get; set; }
    }
}
