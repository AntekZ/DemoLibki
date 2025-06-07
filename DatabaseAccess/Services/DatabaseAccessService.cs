using DatabaseAccess.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;


namespace DatabaseAccess.Services
{
    public class DatabaseAccessService : IDatabaseAccess
    {
        private readonly string? _connectionString;
        private readonly ILogger<DatabaseAccessService> _logger;
        private readonly int _commandTimeout;
        public DatabaseAccessService(IConfiguration configuration, ILogger<DatabaseAccessService> logger)
        {
            _connectionString = configuration["ConnectionString"];
            _commandTimeout = int.TryParse(configuration["CommandTimeout"], out var res) ? res : 30;
            _logger = logger;

            _logger.LogInformation("DB command timeout: {x}", _commandTimeout);

            if (string.IsNullOrEmpty(_connectionString))
            {
                _logger.LogError("Connection string is null or empty");
                throw new ArgumentNullException("Connection string is null or empty");
            }

        }
        public async Task<int> executeAsync(string procedureName,  object? parameters = null)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            
            try
            {
                await connection.OpenAsync();

                var rows = await connection.ExecuteAsync(
                    sql:procedureName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _commandTimeout
                    );

                if (rows == 0)
                {
                    return -1;
                }
                return rows;
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error while executing command {p}: {e}, {i}", procedureName, e.Message,
                    e.InnerException is null ? "" : e.InnerException.Message);
                return -1;
            }
            


        }

        public async Task<List<T>?> getListAsync<T>(string procedureName, object? parameters = null)
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            try
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<T>(
                    sql: procedureName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _commandTimeout
                    );

                return result.ToList();
            }
            catch(Exception e)
            {
                _logger.LogWarning("Error while executing command {p}: {e}, {i}", procedureName, e.Message,
                    e.InnerException is null ? "" : e.InnerException.Message);
                return null;
            }
            
        }

        public async Task<T?> getSingleAsync<T>(string procedureName,  object? parameters = null)
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            try
            {
                await connection.OpenAsync();

                var result = await connection.QuerySingleOrDefaultAsync<T>(
                    sql: procedureName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _commandTimeout
                    );


                return result;
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error while executing command {p}: {e}, {i}", procedureName, e.Message,
                    e.InnerException is null ? "" : e.InnerException.Message);
                return default;
            }
        }
    }
}
