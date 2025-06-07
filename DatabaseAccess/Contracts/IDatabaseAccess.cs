using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Contracts
{
    public interface IDatabaseAccess
    {
        /// <summary>
        /// Executes a stored procedure with specified name and optional parameters
        /// </summary>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Optional parameters for procedure</param>
        /// <returns>Procedure result, null if error </returns>
        /// zwraca zero albo jeden rekord, jesli blad to null
        Task<T?> getSingleAsync<T>(string procedureName, 
            object? parameters = null);



        /// <summary>
        /// Executes a stored procedure with specified name and optional parameters
        /// Returns number of affected rows.
        /// </summary>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Optional parameters for procedure</param>
        /// <returns>Procedure result | can be empty, null if error occured</returns>
        /// zwaraca liste | moze byc pusta lub jesli blad to null
        Task<List<T>?> getListAsync<T>(string procedureName,  
            object? parameters = null);


        /// <summary>
        /// Executes a stored procedure with specified name and optional parameters
        /// Returns number of affected rows.
        /// </summary>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Optional parameters for procedure</param>
        /// <returns>Number of affected rows or -1 if error occured</returns>
        Task<int> executeAsync(string procedureName,  
            object? parameters = null);
    }
}
