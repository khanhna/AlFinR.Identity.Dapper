using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.SqlServer.Repositories
{
    public interface IIdentityUserClaimDapperRepository<TUser, in TKey, TUserClaim>
        where TUser : IdentityUser<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<IEnumerable<TUserClaim>> SearchAsync(TKey userId);
        Task<IEnumerable<TUserClaim>> SearchAsync(TKey userId, string claimType, string claimValue);
        Task<IEnumerable<TUser>> SearchUsers(Claim claim);
        void Add(TUserClaim entry);
        void Remove(TUserClaim entry);
    }

    public class IdentityUserClaimDapperRepository<TUser, TKey, TUserClaim> : IIdentityUserClaimDapperRepository<TUser, TKey, TUserClaim>
        where TUser : IdentityUser<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private static readonly string UserIdColumnName =
            Utility.GetPropertyName((IdentityUserClaim<TKey> uc) => uc.UserId);
        private static readonly string ClaimTypeColumnName =
            Utility.GetPropertyName((IdentityUserClaim<TKey> uc) => uc.ClaimType);
        private static readonly string ClaimValueColumnName =
            Utility.GetPropertyName((IdentityUserClaim<TKey> uc) => uc.ClaimValue);
        private readonly IDbConnection _connection;

        public IdentityUserClaimDapperRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<IEnumerable<TUserClaim>> SearchAsync(TKey userId)
        {
            var sqlQuery = $"SELECT * FROM {IdentityDapperTables.DbTable.UserClaims} WHERE {UserIdColumnName} = @pUserId;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pUserId", userId);

            return _connection.QueryAsync<TUserClaim>(sqlQuery, sqlParams);
        }

        public Task<IEnumerable<TUserClaim>> SearchAsync(TKey userId, string claimType, string claimValue)
        {
            var sqlQuery =
                $"SELECT * FROM {IdentityDapperTables.DbTable.UserClaims} WHERE {UserIdColumnName} = @pUserId AND {ClaimTypeColumnName} = @sClaimType AND {ClaimValueColumnName} = @sClaimValue;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pUserId", userId);
            sqlParams.Add("@sClaimType", claimType);
            sqlParams.Add("@sClaimValue", claimValue);

            return _connection.QueryAsync<TUserClaim>(sqlQuery, sqlParams);
        }

        public Task<IEnumerable<TUser>> SearchUsers(Claim claim)
        {
            var sqlQuery =
                $@"SELECT u.* FROM {IdentityDapperTables.DbTable.UserClaims} uc 
                    INNER JOIN {IdentityDapperTables.DbTable.Users} u ON uc.{UserIdColumnName} = u.Id 
                    WHERE uc.{ClaimTypeColumnName} = @sClaimType AND uc.{ClaimValueColumnName} = @sClaimValue;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@sClaimType", claim.Type, DbType.String, ParameterDirection.Input, claim.Type.Length);
            sqlParams.Add("@sClaimValue", claim.Value, DbType.String, ParameterDirection.Input, claim.Value.Length);

            return _connection.QueryAsync<TUser>(sqlQuery, sqlParams);
        }

        public void Add(TUserClaim entry)
        {
            _connection.Insert(entry);
        }

        public void Remove(TUserClaim entry)
        {
            _connection.Delete(entry);
        }
    }
}
