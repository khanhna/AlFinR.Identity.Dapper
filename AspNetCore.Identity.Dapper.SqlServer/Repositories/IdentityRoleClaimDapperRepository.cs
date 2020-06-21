using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.SqlServer.Repositories
{
    public interface IIdentityRoleClaimDapperRepository<TRole, in TKey, TRoleClaim>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        Task<IEnumerable<TRoleClaim>> SearchByRoleIdAsync(TKey roleId);
        Task<IEnumerable<TRoleClaim>> SearchAsync(TKey roleId, string claimType, string claimValue);
        void Add(TRoleClaim entity);
        void Remove(TRoleClaim entity);
    }

    public class IdentityRoleClaimDapperRepository<TRole, TKey, TRoleClaim> : 
        IIdentityRoleClaimDapperRepository<TRole, TKey, TRoleClaim>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        private readonly IDbConnection _connection;

        public IdentityRoleClaimDapperRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<IEnumerable<TRoleClaim>> SearchByRoleIdAsync(TKey roleId)
        {
            var sqlQuery = $"SELECT * FROM {IdentityDapperTables.DbTable.RoleClaims} WHERE RoleId = @pRoleId;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pRoleId", roleId);

            return _connection.QueryAsync<TRoleClaim>(sqlQuery, sqlParams);
        }

        public Task<IEnumerable<TRoleClaim>> SearchAsync(TKey roleId, string claimType, string claimValue)
        {
            var sqlQuery =
                $"SELECT * FROM {IdentityDapperTables.DbTable.RoleClaims} WHERE RoleId = @pRoleId AND ClaimType = @sClaimType AND ClaimValue = @sClaimValue;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pRoleId", roleId);
            sqlParams.Add("@sClaimType", claimType, DbType.String, ParameterDirection.Input, claimType.Length);
            sqlParams.Add("@sClaimValue", claimValue, DbType.String, ParameterDirection.Input, claimValue.Length);

            return _connection.QueryAsync<TRoleClaim>(sqlQuery, sqlParams);
        }

        public void Add(TRoleClaim entity)
        {
            _connection.Insert(entity);
        }

        public void Remove(TRoleClaim entity)
        {
            _connection.Delete(entity);
        }
    }
}
