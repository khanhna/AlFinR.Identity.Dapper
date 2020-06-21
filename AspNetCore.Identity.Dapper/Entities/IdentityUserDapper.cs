using System;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Entities
{
    [Table(IdentityDapperTables.DbTable.Users)]
    public class IdentityUserDapper<TKey> : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    [Table(IdentityDapperTables.DbTable.Users)]
    public class IdentityUserDapper : IdentityUser<Guid>
    {
        [Key]
        public override Guid Id { get; set; }
    }
}
