using DatabaseAccess.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using Dapper;
using System.Text.Json;
using DatabaseAccess.Models;
using System.Data.SqlClient;
using System.Data.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace DatabaseAccess.Services
{
    public sealed class DatabaseAccessService : IDatabaseAccess
    {
        private readonly IConnectionFactory _factory;
        private readonly ILogger<DatabaseAccessService> _logger;
        private readonly int _commandTimeout;
        private readonly IMemoryCache _cache;
        public DatabaseAccessService(IConnectionFactory factory, IConfiguration configuration, 
            ILogger<DatabaseAccessService> logger,IMemoryCache cache)
        {
            _factory = factory;            
            _commandTimeout = int.TryParse(configuration["CommandTimeout"], out var res) ? res : 30;
            _logger = logger;
            _cache = cache;

            _logger.LogInformation("DB command timeout: {x}", _commandTimeout);
        }
        public event EventHandler<OnDisconnectEventArgs>? OnDisconnect;

        //if connection was passed to the metod by parameter
        //add StateChagne event to connection
        //if OnDisconnect was subsrcibed, invoke
        private void AttachDisconnectHandler(IDbConnection connection)
        {
            
            
            ((SqlConnection)connection).StateChange += (_, e) =>
            {
                
                
                if (e.CurrentState is ConnectionState.Broken or ConnectionState.Closed)
                {
                    DateTime when = DateTime.Now;
                    OnDisconnect?.Invoke(this, new OnDisconnectEventArgs
                    {
                        When = when
                    });
                    _logger.LogWarning("Connection to the database has been lost at {When}", when);
                }
                    
            };
        }
        public async Task<List<T>?> GetListWithCacheAsync<T>(string cacheKey, 
            Func<Task<List<T>?>>? fetch = null, TimeSpan? expiration = null)
        {
            try
            {
                //try to get a value from cache using cache key 
                if (_cache.TryGetValue(cacheKey, out List<T>? cached))
                {
                    _logger.LogInformation($"Returned from cache: {cacheKey}");
                    return cached;
                }
                

                if(fetch is null || expiration is null)
                {
                    _logger.LogWarning($"No data in cache for key: {cacheKey}");
                    return null;
                }
                
                //if there is nothing in cache, signe to data result of passed function
                var data = await fetch();

                if (data is null)
                {
                    _logger.LogWarning($"Fetch for {cacheKey} returned null — not caching.");
                    return null;
                }

                //setting up memory cache.
                _cache.Set(cacheKey, data, absoluteExpirationRelativeToNow: expiration.Value);
                _logger.LogInformation($"Data cached under key: {cacheKey}");
                return data;
                                
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetListFromCacheAsync failed for {cacheKey}: {e.Message}");
                return null;
            }

        }   
          
        public void ClearCache(string cacheKey)
        {
            bool cacheExists = _cache.TryGetValue(cacheKey, out _);
            if (cacheExists)
            {
                _cache.Remove(cacheKey);
                _logger.LogInformation($"Data signed to: {cacheKey} cleared.");
            }
            else
            {
                _logger.LogInformation($"Data signed to: {cacheKey} not found.");
            }
        }
        public async Task<int> ExecuteAsync(string procedureName,  object? parameters = null, IDbConnection? connection = null)
        {
            // flag to idnetify if conn was passed in parameter or not 
            //if passed connection: ownsConnetion = false
            bool ownsConnection = connection is null;
            
            
            try
            {
                //if connection was not passed, create connection
                if(connection==null)
                    connection = await _factory.CreateOpenConnectionAsync();


                // if passed connection was not opened throw exception
                if (!ownsConnection && connection.State == ConnectionState.Closed)
                    throw new InvalidOperationException("Passed connection is not opened.");

                //if passed connection, add StateChange event
                if (!ownsConnection)
                {
                    AttachDisconnectHandler(connection);
                }

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
                _logger.LogWarning("Error while executing procedure {p}: {e}, {i}", procedureName, e.Message,
                    e.InnerException is null ? "" : e.InnerException.Message);      
                              
                return -1;
            }
            finally
            {
                //if connection made by this method - dispose
                if (ownsConnection == true && connection is not null)
                {
                    connection.Dispose();
                }
            }
            
        }
        
        public async Task<List<T>?> GetListAsync<T>(string procedureName, object? parameters = null, IDbConnection? connection = null)
        {          
            // flag to idnetify if conn was passed in parameter or not 
            //if passed connection: ownsConnetion = false
            bool ownsConnection = connection is null;

            try
            {
                //if connection was not passed, create connection
                if (connection == null)
                    connection = await _factory.CreateOpenConnectionAsync();



                // if passed connection was not opened throw exception
                if (!ownsConnection && connection.State == ConnectionState.Closed)
                    throw new InvalidOperationException("Passed connection is not opened.");

                //if passed connection, add StateChange event
                if (!ownsConnection)
                {
                    AttachDisconnectHandler(connection);
                }

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
                _logger.LogWarning("Error while executing procedure {p}: {e}, {i}", procedureName, e.Message,
                    e.InnerException is null ? "" : e.InnerException.Message);
                
                return default;
            }
            finally
            {
                //if connection made by this method - dispose
                if (ownsConnection == true && connection is not null)
                {
                    connection.Dispose();
                }
            }
        }

        public async Task<List<T>?> GetListFromJsonAsync<T>(string procedureName, object? parameters = null, IDbConnection? connection = null)
        {
            // flag to idnetify if conn was passed in parameter or not 
            //if passed connection: ownsConnetion = false
            bool ownsConnection = connection is null;

            try
            {

                //if connection was not passed, create connection              
                if (connection == null)
                    connection = await _factory.CreateOpenConnectionAsync();



                // if passed connection was not opened throw exception
                if (!ownsConnection && connection.State == ConnectionState.Closed)
                    throw new InvalidOperationException("Passed connection is not opened.");

                //if passed connection, add StateChange event
                if (!ownsConnection)
                {
                    AttachDisconnectHandler(connection);
                }

                //Zero or one row is expected to be returned. Returns an instance of the string type or null.
                var jsonData = await connection.QuerySingleOrDefaultAsync<string>(
                    sql: procedureName,
                    param: parameters,
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
                _logger.LogWarning("Error while executing procedure {p}: {e}, {i}", procedureName, e.Message,
                    e.InnerException is null ? "" : e.InnerException.Message);
              
                //Return null if error occured.
                return default;
            }
            finally
            {
                //if connection made by this method - dispose
                if (ownsConnection == true && connection is not null)
                {
                    connection.Dispose();
                }
            }
        }

        public async Task<T?> GetSingleAsync<T>(string procedureName,  object? parameters = null, IDbConnection? connection = null)
        {
            // flag to idnetify if conn was passed in parameter or not 
            //if passed connection: ownsConnetion = false
            bool ownsConnection = connection is null;

            try
            {
                //if connection was not passed, create connection
                if (connection == null)
                    connection = await _factory.CreateOpenConnectionAsync();



                // if passed connection was not opened throw exception
                if (!ownsConnection && connection.State == ConnectionState.Closed)
                    throw new InvalidOperationException("Passed connection is not opened.");

                //if passed connection, add StateChange event
                if (!ownsConnection)
                {
                    AttachDisconnectHandler(connection);
                }

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
                _logger.LogWarning("Error while executing procedure {p}: {e}, {i}", procedureName, e.Message,
                    e.InnerException is null ? "" : e.InnerException.Message);
                
                return default;
            }
            finally
            {
                //if connection made by this method - dispose
                if (ownsConnection == true && connection is not null)
                {
                    connection.Dispose();
                }
            }
        }
    }
}
