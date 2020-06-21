# Re-produce Demo project

Demo project was built on .Net Core 3.1, make sure you get the correct environment.

1. Create your own database (local or sqlserver works fine!).
2. Run the script from DatabaseScript/Database Structure_Default.sql.
3. Update your **ConnectionStrings** string in appsetting.json file:

```JSON
"ConnectionStrings": {
    "DefaultConnection": "My Lovely connection string;"
  }
```

4. You are good to go, just run the application, Register, sign-in, sign-out!

# Use it for your project:

1. Based on the Database DatabaseScript/Structure_DemoVersion.sql file, make your own rightful DB structure to use.
2. Change the table name corresponding with what you change in DB(SKIP THIS STEP IF TABLE NAME DOESN'T CHANGE), in IdentityDapperTables.cs file in AspNetCore.Identity.Dapper assembly
```csharp
public static class IdentityDapperTables
    {
        public static class DbTable
        {
            public const string Users = "AspNetUsers";
            public const string UserRoles = "AspNetUserRoles";
            public const string UserClaims = "AspNetUserClaims";
            public const string UserLogins = "AspNetUserLogins";
            public const string UserTokens = "AspNetUserTokens";
            
            public const string Roles = "AspNetRoles";
            public const string RoleClaims = "AspNetRoleClaims";
        }
    }
```

3. Create your own custom entities in your project as i did. Mostly we will change Key property data-type.
```csharp
[Table(IdentityDapperTables.DbTable.Users)]
    public class ApplicationUser : IdentityUserDapper<int>
    {
        [Key]
        public override int Id { get; set; }

        public string Image { get; set; }
        [Write(false)] public string Password { get; set; }
        [Write(false)] public bool IsAdministrator { get; set; }
    }
    ...
```

4. Change region Identity Dapper snippet on starup as it suit you.

```csharp
#region Identity Dapper snippet

            services.AddTransient<IDbConnection, SqlConnection>(e =>
                new SqlConnection(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddUserStore<UserStoreDapper<ApplicationUser, ApplicationRole, int>>()
                .AddRoleStore<RoleStoreDapper<ApplicationRole, int>>()
                .AddDefaultTokenProviders();

            services
                .AddTransient<IIdentityUserDapperRepository<ApplicationUser, int>,
                    IdentityUserDapperRepository<ApplicationUser, int>>();
            services
                .AddTransient<IIdentityUserLoginDapperRepository<ApplicationUser, int, ApplicationUserLogin>,
                    IdentityUserLoginDapperRepository<ApplicationUser, int, ApplicationUserLogin>>();
	    ...
#endregion
```

5. If you have additional entities to work coressponding with those entities, treat it as any normal entity you would process in your project. If Interactive data is required, write extension as you do with classic original Identity.Store package from Microsoft(recommended). Or just directly modified the Dapper project is just fine, but not recommended as it could lose your changes if furthur update arrived.
6. This project is simple enough for you guys to take the ideas for a while reading it through, when you took the spirit, fell free doing anything you like!

Happy coding!