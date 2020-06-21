using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.SqlServer.Repositories
{
    public interface IIdentityUserLoginDapperRepository<TUser, in TKey, TUserLogin>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLogin<TKey>, new()
    {
        Task AddAsync(TUserLogin entry);
        Task<TUserLogin> FindAsync(TKey userId, string loginProvider, string providerKey);
        Task<TUserLogin> FindAsync(string loginProvider, string providerKey);
        Task<IEnumerable<TUserLogin>> FindAsync(TKey userId);
        void Remove(TUserLogin entry);
    }

    public class IdentityUserLoginDapperRepository<TUser, TKey, TUserLogin> 
        : IIdentityUserLoginDapperRepository<TUser, TKey, TUserLogin>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLogin<TKey>, new()
    {
        private static readonly PropertyInfo[] EntityProperties = typeof(TUserLogin)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        private static readonly IEnumerable<string> Columns = EntityProperties.Select(p => p.Name);
        private static readonly IEnumerable<string> ColumnParams = Columns.Select(c => $@"p{c}");
        private static readonly string ColumnStringQuery = string.Join(", ", Columns);
        private static readonly string ColumnStringQueryParams = string.Join(", ", ColumnParams);

        private static readonly string UserIdColumnName =
            Utility.GetPropertyName((IdentityUserLogin<TKey> ul) => ul.UserId);
        private static readonly string LoginProviderColumnName =
            Utility.GetPropertyName((IdentityUserLogin<TKey> ul) => ul.LoginProvider);
        private static readonly string ProviderKeyColumnName =
            Utility.GetPropertyName((IdentityUserLogin<TKey> ul) => ul.ProviderKey);
        private readonly IDbConnection _connection;

        public IdentityUserLoginDapperRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task AddAsync(TUserLogin entry)
        {
            var sqlQuery =
                $"INSERT INTO {IdentityDapperTables.DbTable.UserLogins}({ColumnStringQuery}) VALUES({ColumnStringQueryParams});";
            var sqlParams = new DynamicParameters();
            var index = 0;
            foreach (var item in ColumnParams)
            {
                sqlParams.Add(item, EntityProperties[index].GetValue(entry, null));
                index++;
            }

            return _connection.ExecuteAsync(sqlQuery, sqlParams);
        }

        public Task<TUserLogin> FindAsync(TKey userId, string loginProvider, string providerKey)
        {
            var sqlQuery =
                $"SELECT * FROM {IdentityDapperTables.DbTable.UserLogins} WHERE {UserIdColumnName} = @pUserId AND {LoginProviderColumnName} = @pLoginProvider AND {ProviderKeyColumnName} = @pProviderKey;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pUserId", userId);
            sqlParams.Add("@pLoginProvider", loginProvider);
            sqlParams.Add("@pProviderKey", providerKey);

            return _connection.QueryFirstOrDefaultAsync<TUserLogin>(sqlQuery, sqlParams);
        }

        public Task<TUserLogin> FindAsync(string loginProvider, string providerKey)
        {
            var sqlQuery =
                $"SELECT * FROM {IdentityDapperTables.DbTable.UserLogins} WHERE {LoginProviderColumnName} = @pLoginProvider AND {ProviderKeyColumnName} = @pProviderKey;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pLoginProvider", loginProvider);
            sqlParams.Add("@pProviderKey", providerKey);

            return _connection.QueryFirstOrDefaultAsync<TUserLogin>(sqlQuery, sqlParams);
        }

        public Task<IEnumerable<TUserLogin>> FindAsync(TKey userId)
        {
            var sqlQuery = $"SELECT * FROM {IdentityDapperTables.DbTable.UserLogins} WHERE {UserIdColumnName} = @pUserId;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pUserId", userId);

            return _connection.QueryAsync<TUserLogin>(sqlQuery, sqlParams);
        }

        public void Remove(TUserLogin entry)
        {
            var sqlQuery =
                $"DELETE {IdentityDapperTables.DbTable.UserLogins} WHERE {LoginProviderColumnName} = @pLoginProvider AND {ProviderKeyColumnName} = @pProviderKey;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pLoginProvider", entry.LoginProvider, DbType.String, ParameterDirection.Input,
                entry.LoginProvider.Length);
            sqlParams.Add("@pProviderKey", entry.ProviderKey, DbType.String, ParameterDirection.Input,
                entry.ProviderKey.Length);

            _connection.Execute(sqlQuery, sqlParams);
        }
    }
}
