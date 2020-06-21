using System;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Entities
{
    [Table(IdentityDapperTables.DbTable.UserTokens)]
    public class IdentityUserTokenDapper<TKey> : IdentityUserToken<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    [Table(IdentityDapperTables.DbTable.UserTokens)]
    public class IdentityUserTokenDapper : IdentityUserToken<Guid>
    {
    }
}
