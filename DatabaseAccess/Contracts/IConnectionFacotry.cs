using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Contracts
{
    public interface IConnectionFactory
    {
        /// <summary>
        /// Returns created and opened data base connection.
        /// </summary>
        /// <returns>Data base connection.</returns>
        Task<IDbConnection> CreateOpenConnectionAsync();

        
    }
}
