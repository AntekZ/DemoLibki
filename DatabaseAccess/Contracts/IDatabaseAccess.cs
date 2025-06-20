﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Contracts
{
    public interface IDatabaseAccess
    {
        /// <summary>
        /// Opens and dispose database connection. | if sql connetion is passed as parameter it is mandatory to mannualy open and dispose database connetiion
        /// Executes a stored procedure with specified name and optional parameters.
        /// Returns procedure result (precisely one record), null if error occured.
        /// </summary>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Optional parameters for procedure</param>
        /// <returns>Procedure result (precisely one record), null if error occured or zero records returned by the procedure</returns>
        Task<T?> GetSingleAsync<T>(string procedureName, 
            object? parameters = null,
            IDbConnection? connection = null
            );



        /// <summary>
        /// Opens and dispose database connection. | if sql connetion is passed as parameter it is mandatory to mannualy open and dispose database connetiion
        /// Executes a stored procedure with specified name and optional parameters.
        /// Returns procedure result, null if error occured.
        /// </summary>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Optional parameters for procedure</param>
        /// <returns>Procedure result, null if error occured</returns>
        Task<List<T>?> GetListAsync<T>(string procedureName,  
            object? parameters = null,
            IDbConnection? connection = null
            );


        /// <summary>
        /// Opens and dispose database connection. | if sql connetion is passed as parameter it is mandatory to mannualy open and dispose database connetiion
        /// Executes a stored procedure with specified name and optional parameters.
        /// Returns number of affected rows or -1 if error occured.
        /// </summary>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Optional parameters for procedure</param>
        /// <returns>Number of affected rows or -1 if error occured</returns>
        Task<int> ExecuteAsync(string procedureName,  
            object? parameters = null,
            IDbConnection? connection = null
            );


        /// <summary>
        /// Opens and dispose database connection | if sql connetion is passed as parameter it is mandatory to mannualy open and dispose database connetiion
        /// Executes a stored procedure with specified name and optional parameters.
        /// Deserialize Json into a list of objects of type and returns the list.
        /// 
        /// The stored procedure is expected to return:
        /// A signle row containing a Json array in a single column (SELECT (SELECT ... FOR JSON PATH) AS JsonData)  or no rows at all
        /// </summary>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Optional parameters for procedure</param>
        /// <returns>A List<T> if the procedure returns valid Json, Null if an error ocurred.</returns>
        Task<List<T>?> GetListFromJsonAsync<T>(string procedureName,
            object? parameters = null,
            IDbConnection? connection = null
            );

    }
}
