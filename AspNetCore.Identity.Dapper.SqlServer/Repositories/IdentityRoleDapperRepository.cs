using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.SqlServer.Repositories
{
    public interface IIdentityRoleDapperRepository<TRole, in TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        Task AddAsync(TRole entity);
        Task UpdateAsync(TRole entity);
        Task RemoveAsync(TRole entity);
        Task<TRole> FindByNormalizedNameAsync(string normalizedRoleName);
        Task<TRole> FindAsync(TKey id);
    }

    public class IdentityRoleDapperRepository<TRole, TKey> : IIdentityRoleDapperRepository<TRole, TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IDbConnection _connection;

        public IdentityRoleDapperRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task AddAsync(TRole entity)
        {
            return _connection.InsertAsync(entity);
        }

        public Task UpdateAsync(TRole entity)
        {
            return _connection.UpdateAsync(entity);
        }

        public Task RemoveAsync(TRole entity)
        {
            return _connection.DeleteAsync(entity);
        }

        public Task<TRole> FindByNormalizedNameAsync(string normalizedRoleName)
        {
            var sqlQuery =
                $"SELECT * FROM {IdentityDapperTables.DbTable.Roles} WHERE {Utility.GetPropertyName((IdentityRole r) => r.NormalizedName)} = @sNormalizedName;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@sNormalizedName", normalizedRoleName, DbType.String, ParameterDirection.Input,
                normalizedRoleName.Length);

            return _connection.QueryFirstOrDefaultAsync<TRole>(sqlQuery, sqlParams);
        }

        public Task<TRole> FindAsync(TKey id)
        {
            var sqlQuery = $"SELECT * FROM {IdentityDapperTables.DbTable.Roles} WHERE Id = @IdParam;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@IdParam", id);

            return _connection.QueryFirstOrDefaultAsync<TRole>(sqlQuery, sqlParams);
        }
    }
}
