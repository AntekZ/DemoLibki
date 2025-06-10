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
using System.Text.Json;


namespace DatabaseAccess.Services
{
    public sealed class DatabaseAccessService : IDatabaseAccess
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
       
        public async Task<int> ExecuteAsync(string procedureName,  object? parameters = null)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            
            try
            {
                await connection.OpenAsync();
                //Returns how many rows were changed 
                var rows = await connection.ExecuteAsync(
                    sql:procedureName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _commandTimeout
                    );

                return rows;
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error while executing command {p}: {e}, {i}", procedureName, e.Message,
                    e.InnerException is null ? "" : e.InnerException.Message);
                return -1;
            }
            finally
            {
                connection.Dispose();
            }
            


        }
        
        public async Task<List<T>?> GetListAsync<T>(string procedureName, object? parameters = null)
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            try
            {
                await connection.OpenAsync();


                //returns an enumerable of the type specified by the T parameter
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
                return default;
            }
            finally
            {
                connection.Dispose();
            }
        }

        public async Task<List<T>?> GetListFromJsonAsync<T>(string procedureName, object? prameters = null)
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            try
            {
                await connection.OpenAsync();

                //Zero or one row is expected to be returned. Returns an instance of the string type or null.
                var jsonData = await connection.QuerySingleOrDefaultAsync<string>(
                    sql: procedureName,
                    param: prameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _commandTimeout
                    );



                //if stored procedure returned zero rows method returns empty list. 

                if (jsonData == null)
                {
                    return new List<T>();
                }

                var result = JsonSerializer.Deserialize<List<T>>(jsonData);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error while executing command {p}: {e}, {i}", procedureName, e.Message,
                    e.InnerException is null ? "" : e.InnerException.Message);
                //Return null if error occured.
                return default;
            }
            finally
            {
                connection.Dispose();
            }
        }

        public async Task<T?> GetSingleAsync<T>(string procedureName,  object? parameters = null)
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            try
            {
                await connection.OpenAsync();

                //Expects zero or one row to be returned. Returns an instance of the specified by the T type or null
                var result = await connection.QuerySingleOrDefaultAsync<T>(
                    sql: procedureName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _commandTimeout
                    );

                //if zero rows, return null since no data to get
                if(result == null)
                {
                    return default;
                }
                    
                return result;
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error while executing command {p}: {e}, {i}", procedureName, e.Message,
                    e.InnerException is null ? "" : e.InnerException.Message);
                return default;
            }
            finally
            {
                connection.Dispose();  
            }
        }
    }
}
