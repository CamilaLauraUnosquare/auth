namespace Libelula.Authentication.DataAccess.Services;
using Dapper;
using System.Data;

using System.Data.SqlClient;
using Libelula.Authentication.DataAccess.Interfaces;
using Libelula.Authentication.Models;

public class AuthRepository : IAuthRepository
{
    private readonly string connectionString;

    public AuthRepository(string sqlConnection)
    {
        this.connectionString = sqlConnection;
    }

    public async Task<User?> GetUser(string userEmail)
    {
        using IDbConnection db = new SqlConnection(this.connectionString);
        string storeProcedure = "Get_User";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Email", userEmail, DbType.String, ParameterDirection.Input);
        var user = await db.QueryAsync<User>(storeProcedure, parameters, commandType: CommandType.StoredProcedure);
        return user.FirstOrDefault();
    }

    public async Task RegisterUser(User user)
    {
        using IDbConnection db = new SqlConnection(this.connectionString);
        string procedure = "Save_User";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@FirstName", user.FirstName, DbType.String, ParameterDirection.Input, 50);
        parameters.Add("@LastName", user.LastName, DbType.String, ParameterDirection.Input, 50);
        parameters.Add("@Email", user.Email, DbType.String, ParameterDirection.Input, 300);
        parameters.Add("@PasswordHash", user.PasswordHash, DbType.Binary, ParameterDirection.Input);
        parameters.Add("@PasswordSalt", user.PasswordSalt, DbType.Binary, ParameterDirection.Input);
        var userx = await db.QueryAsync<User>(procedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateUser(User user)
    {
        using IDbConnection db = new SqlConnection(this.connectionString);
        string procedure = "Update_User";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Email", user.Email, DbType.String, ParameterDirection.Input, 250);
        parameters.Add("@RefreshToken", user.RefreshToken, DbType.String, ParameterDirection.Input, 250);
        parameters.Add("@TokenCreated", user.TokenCreated, DbType.DateTime2, ParameterDirection.Input, 250);
        parameters.Add("@TokenExpires", user.TokenExpires, DbType.DateTime2, ParameterDirection.Input, 250);
        await db.QueryAsync<User>(procedure, parameters, commandType: CommandType.StoredProcedure);
    }
}