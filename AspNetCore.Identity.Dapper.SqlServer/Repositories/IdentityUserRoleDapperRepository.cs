using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.SqlServer.Repositories
{
    public interface IIdentityUserRoleDapperRepository<TUser, in TKey, TUserRole>
        where TUser : IdentityUser<TKey>
        where TUserRole : IdentityUserRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<TUserRole> FindAsync(TKey userId, TKey roleId);
        Task<IEnumerable<string>> SearchRoleNamesByUserIdAsync(TKey userId);
        Task<IEnumerable<TUser>> SearchUsersByRoleIdAsync(TKey roleId);
        void Add(TUserRole entry);
        void Remove(TUserRole entry);
    }

    public class IdentityUserRoleDapperRepository<TUser, TKey, TUserRole> : IIdentityUserRoleDapperRepository<TUser, TKey, TUserRole>
        where TUser : IdentityUser<TKey>
        where TUserRole : IdentityUserRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private static readonly string UserIdColumnName =
            Utility.GetPropertyName((IdentityUserRole<TKey> ur) => ur.UserId);
        private static readonly string RoleIdColumnName =
            Utility.GetPropertyName((IdentityUserRole<TKey> ur) => ur.RoleId);
        private readonly IDbConnection _connection;

        public IdentityUserRoleDapperRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<TUserRole> FindAsync(TKey userId, TKey roleId)
        {
            var sqlQuery =
                $"SELECT * FROM {IdentityDapperTables.DbTable.UserRoles} WHERE {UserIdColumnName} = @pUserId AND {RoleIdColumnName} = @pRoleId;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pUserId", userId);
            sqlParams.Add("@pRoleId", roleId);

            return _connection.QueryFirstOrDefaultAsync<TUserRole>(sqlQuery, sqlParams);
        }

        public Task<IEnumerable<string>> SearchRoleNamesByUserIdAsync(TKey userId)
        {
            var sqlQuery =
                $@"SELECT r.Name FROM {IdentityDapperTables.DbTable.UserRoles} ur 
                    INNER JOIN {IdentityDapperTables.DbTable.Roles} r ON ur.RoleId = r.Id 
                    WHERE ur.UserId = @pUserId;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pUserId", userId);

            return _connection.QueryAsync<string>(sqlQuery, sqlParams);
        }

        public Task<IEnumerable<TUser>> SearchUsersByRoleIdAsync(TKey roleId)
        {
            var sqlQuery =
                $@"SELECT u.* FROM {IdentityDapperTables.DbTable.UserRoles} ur 
                    INNER JOIN {IdentityDapperTables.DbTable.Users} u ON ur.UserId = u.Id 
                    WHERE ur.RoleId = @pRoleId;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pRoleId", roleId);
            
            return _connection.QueryAsync<TUser>(sqlQuery, sqlParams);
        }

        public void Add(TUserRole entry)
        {
            var sqlQuery =
                $"INSERT INTO {IdentityDapperTables.DbTable.UserRoles}({UserIdColumnName}, {RoleIdColumnName}) VALUES(@pUserId, @pRoleId);";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pUserId", entry.UserId);
            sqlParams.Add("@pRoleId", entry.RoleId);

            _connection.Execute(sqlQuery, sqlParams);
        }

        public void Remove(TUserRole entry)
        {
            var sqlQuery =
                $"DELETE {IdentityDapperTables.DbTable.UserRoles}({UserIdColumnName}, {RoleIdColumnName}) WHERE {UserIdColumnName} = @pUserId AND {RoleIdColumnName} = @pRoleId;";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("@pUserId", entry.UserId);
            sqlParams.Add("@pRoleId", entry.RoleId);

            _connection.Execute(sqlQuery, sqlParams);
        }
    }
}
