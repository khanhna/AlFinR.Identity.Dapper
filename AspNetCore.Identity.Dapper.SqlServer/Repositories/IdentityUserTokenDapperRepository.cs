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
    public interface IIdentityUserTokenDapperRepository<TUser, in TKey, TUserToken>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        Task<TUserToken> FindAsync(TKey userId, string loginProvider, string name);
        Task AddAsync(TUserToken entry);
        Task RemoveAsync(TUserToken entry);
    }

    public class IdentityUserTokenDapperRepository<TUser, TKey, TUserToken> : IIdentityUserTokenDapperRepository<TUser, TKey, TUserToken>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        private static readonly PropertyInfo[] EntityProperties = typeof(TUserToken)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        private static readonly IEnumerable<string> Columns = EntityProperties.Select(p => p.Name);
        private static readonly IEnumerable<string> ColumnParams = Columns.Select(c => $@"p{c}");
        private static readonly string ColumnStringQuery = string.Join(", ", Columns);
        private static readonly string ColumnStringQueryParams = string.Join(", ", ColumnParams);

        private static readonly string UserIdColumnName =
            Utility.GetPropertyName((IdentityUserToken<TKey> ut) => ut.UserId);
        private static readonly string LoginProviderColumnName =
            Utility.GetPropertyName((IdentityUserToken<TKey> ut) => ut.LoginProvider);
        private static readonly string NameColumn =
            Utility.GetPropertyName((IdentityUserToken<TKey> ut) => ut.Name);
        private readonly IDbConnection _connection;

        public IdentityUserTokenDapperRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<TUserToken> FindAsync(TKey userId, string loginProvider, string name)
        {
            var sqlQuery =
                $"SELECT * FROM {IdentityDapperTables.DbTable.UserTokens} WHERE {UserIdColumnName} = @pUserId AND {LoginProviderColumnName} = @pLoginProvider AND {NameColumn} = @pName;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pUserId", userId);
            sqlParams.Add("@pLoginProvider", loginProvider, DbType.String, ParameterDirection.Input,
                loginProvider.Length);
            sqlParams.Add("@pName", name, DbType.String, ParameterDirection.Input, name.Length);

            return _connection.QueryFirstOrDefaultAsync<TUserToken>(sqlQuery, sqlParams);
        }

        public Task AddAsync(TUserToken entry)
        {
            var sqlQuery =
                $"INSERT INTO {IdentityDapperTables.DbTable.UserTokens}({ColumnStringQuery}) VALUES({ColumnStringQueryParams});";
            var sqlParams = new DynamicParameters();
            var index = 0;
            foreach (var item in ColumnParams)
            {
                sqlParams.Add(item, EntityProperties[index].GetValue(entry, null));
                index++;
            }

            return _connection.ExecuteAsync(sqlQuery, sqlParams);
        }

        public Task RemoveAsync(TUserToken entry)
        {
            var sqlQuery =
                $"DELETE {IdentityDapperTables.DbTable.UserTokens} WHERE {UserIdColumnName} = @pUserId AND {LoginProviderColumnName} = @pLoginProvider AND {NameColumn} = @pName;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pUserId", entry.UserId);
            sqlParams.Add("@pLoginProvider", entry.LoginProvider, DbType.String, ParameterDirection.Input,
                entry.LoginProvider.Length);
            sqlParams.Add("@pName", entry.Name, DbType.String, ParameterDirection.Input, entry.Name.Length);

            return _connection.ExecuteAsync(sqlQuery, sqlParams);
        }
    }
}
