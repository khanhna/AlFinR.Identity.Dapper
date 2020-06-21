using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.SqlServer.Repositories
{
    public interface IIdentityUserDapperRepository<TUser, in TKey> 
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        Task AddAsync(TUser entity);
        Task UpdateAsync(TUser entity);
        Task RemoveAsync(TUser entity);
        Task<TUser> FindAsync(TKey id);
        Task<TUser> FindByNormalizedNameAsync(string normalizedUserName);
        Task<TUser> FindByNormalizedEmailAsync(string normalizedEmail);
    }

    public class IdentityUserDapperRepository<TUser, TKey> : IIdentityUserDapperRepository<TUser, TKey> 
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IDbConnection _connection;

        public IdentityUserDapperRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task AddAsync(TUser entity)
        {
            return _connection.InsertAsync(entity);
        }

        public Task UpdateAsync(TUser entity)
        {
            return _connection.UpdateAsync(entity);
        }

        public Task RemoveAsync(TUser entity)
        {
            return _connection.DeleteAsync(entity);
        }

        public Task<TUser> FindAsync(TKey id)
        {
            var sqlQuery = $"SELECT * FROM {IdentityDapperTables.DbTable.Users} WHERE Id = @IdParam;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@IdParam", id);

            return _connection.QueryFirstOrDefaultAsync<TUser>(sqlQuery, sqlParams);
        }

        public Task<TUser> FindByNormalizedNameAsync(string normalizedUserName)
        {
            var sqlQuery =
                $"SELECT * FROM {IdentityDapperTables.DbTable.Users} WHERE {Utility.GetPropertyName((IdentityUser u) => u.NormalizedUserName)} = @sNormalizedUserName;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@sNormalizedUserName", normalizedUserName, DbType.String, ParameterDirection.Input,
                normalizedUserName.Length);

            return _connection.QueryFirstOrDefaultAsync<TUser>(sqlQuery, sqlParams);
        }

        public Task<TUser> FindByNormalizedEmailAsync(string normalizedEmail)
        {
            var sqlQuery =
                $"SELECT * FROM {IdentityDapperTables.DbTable.Users} WHERE {Utility.GetPropertyName((IdentityUser u) => u.NormalizedEmail)} = @sNormalizedEmail;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@sNormalizedEmail", normalizedEmail, DbType.String, ParameterDirection.Input,
                normalizedEmail.Length);

            return _connection.QueryFirstOrDefaultAsync<TUser>(sqlQuery, sqlParams);
        }
    }
}
