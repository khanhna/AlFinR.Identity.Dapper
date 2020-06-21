using System;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Entities
{
    [Table(IdentityDapperTables.DbTable.UserRoles)]
    public class IdentityUserRoleDapper<TKey> : IdentityUserRole<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    [Table(IdentityDapperTables.DbTable.UserRoles)]
    public class IdentityUserRoleDapper : IdentityUserRole<Guid>
    {
    }
}
