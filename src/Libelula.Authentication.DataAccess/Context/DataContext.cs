namespace Libelula.Authentication.DataAccess.Context;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using Libelula.Authentication.Models;
using Microsoft.Extensions.Options;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml.Linq;

public class DataContext
{
    private readonly IConfiguration configuration;

    public DataContext(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(this.configuration.GetConnectionString("DefaultConnection"));
    }

    public async Task Init()
    {
        await this.InitDatabase();
        await this.InitTables();
    }

    private async Task InitDatabase()
    {
        using var connection = new SqlConnection(this.configuration.GetConnectionString("InitConnection"));
        string sql = $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Libelula_Auth_Conciliations') CREATE DATABASE [Libelula_Auth_Conciliations];";
        await connection.ExecuteAsync(sql);
    }

    private async Task InitTables()
    {
        using var connection = this.CreateConnection();
        await InitUsers();

        async Task InitUsers()
        {
            string createUserTable = """
                IF OBJECT_ID('[User]', 'U') IS NULL
                CREATE TABLE [User] (
                    UserId INT NOT NULL PRIMARY KEY IDENTITY,
                    FirstName NVARCHAR(50),
                    LastName NVARCHAR(50),
                    [Email] [nvarchar](250) NULL,
                    [PasswordHash] [varbinary](max) NULL,
                    [PasswordSalt] [varbinary](max) NULL,
                    [RefreshToken] [nvarchar](max) NULL,
                    [TokenCreated] [datetime2](7) NULL,
                    [tokenExpires] [datetime2](7) NULL
                );
            """;
            await connection.ExecuteAsync(createUserTable);
        }
    }
}