using System;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Entities
{
    [Table(IdentityDapperTables.DbTable.UserLogins)]
    public class IdentityUserLoginDapper<TKey> : IdentityUserLogin<TKey>
        where TKey : IEquatable<TKey>
    {
    }
    [Table(IdentityDapperTables.DbTable.UserLogins)]
    public class IdentityUserLoginDapper : IdentityUserLogin<Guid>
    {
    }
}
