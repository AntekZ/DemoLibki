using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperTest
{
    public class Product
    {
        // ProductID - int (Primary Key, not nullable)
        public int ProductID { get; set; }

        // ProductName - nvarchar(40) (not nullable)
        public string ProductName { get; set; }

        // SupplierID - int (nullable)
        // Używamy int? (nullable int) ponieważ kolumna pozwala na NULLs
        public int? SupplierID { get; set; }

        // CategoryID - int (nullable)
        // Używamy int? (nullable int) ponieważ kolumna pozwala na NULLs
        public int? CategoryID { get; set; }

        // QuantityPerUnit - nvarchar(20) (nullable)
        public string QuantityPerUnit { get; set; }

        // UnitPrice - money (nullable)
        // Typ 'money' w SQL Server najlepiej mapować na 'decimal' w C#
        // Używamy decimal? (nullable decimal) ponieważ kolumna pozwala na NULLs
        public decimal? UnitPrice { get; set; }

        // UnitsInStock - smallint (nullable)
        // Typ 'smallint' w SQL Server mapuje się na 'short' w C#
        // Używamy short? (nullable short) ponieważ kolumna pozwala na NULLs
        public short? UnitsInStock { get; set; }

        // UnitsOnOrder - smallint (nullable)
        // Używamy short? (nullable short) ponieważ kolumna pozwala na NULLs
        public short? UnitsOnOrder { get; set; }

        // ReorderLevel - smallint (nullable)
        // Używamy short? (nullable short) ponieważ kolumna pozwala na NULLs
        public short? ReorderLevel { get; set; }

        // Discontinued - bit (not nullable)
        // Typ 'bit' w SQL Server mapuje się na 'bool' w C#
        public bool Discontinued { get; set; }
    }
}