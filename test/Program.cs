using DapperTest;
using DatabaseAccess;
using DatabaseAccess.Contracts;
using DatabaseAccess.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using test;
namespace DatabaseAccessTest
{
    public sealed class Program
    {

        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                //{ "ConnectionString", "Server=141.95.205.183;Database=NConnect2;User ID=azaprzala;Password=aZn8TbcP8b89;Encrypt=False"},
                { "ConnectionString","Server=ASUSANTEK\\SQLEXPRESS;Database=NORTHWND;Trusted_Connection=True;Encrypt=False;"},
                { "CommandTimeout", "30" }
            })
            .Build();

            
                using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                ILogger<DatabaseAccessService> logger = loggerFactory.CreateLogger<DatabaseAccessService>();
                IConnectionFactory factory = new SqlConnectionFactory(configuration);

                IDatabaseAccess DbA = new DatabaseAccessService(factory, configuration, logger);

                var employees = await DbA.GetListAsync<Employee>("GetAllEmployees");

                foreach (var employee in employees)
                {

                    Console.WriteLine($"{employee.FirstName}, {employee.LastName}");
                }


            var conn = new SqlConnection("Server=ASUSANTEK\\SQLEXPRESS;Database=NORTHWND;Trusted_Connection=True;Encrypt=False;");

            conn.Open();

            var employeesTest = await DbA.GetListAsync<Employee>("GetAllEmployees", connection: conn);
            foreach (var employee1 in employeesTest)
            {

                Console.WriteLine($"{employee1.FirstName}, {employee1.LastName}");
            }
            conn.Dispose();
            //if (products != null)
            //{
            //    Console.WriteLine("dd");
            //    Console.WriteLine($"Returned {products.Count} products.");
            //    Console.WriteLine(products);
            //    foreach (var product in products)
            //    {
            //        Console.WriteLine($"ID: {product.ProductID}, Name: {product.ProductName}, Price: {product.UnitPrice}, InStock: {product.UnitsInStock}, Discontinued: {product.Discontinued}");

            //    }
            //}
            //else
            //{
            //    Console.WriteLine("okk");
            //}
            //var lista = await DbA.GetListAsync<Employee>("",new { numberOfEmployees = 10 } );




            //var lista2 = await DbA.GetListAsync<Employee>("GetAllEmployees");

            //foreach (Employee e in lista2)
            //{
            //    Console.WriteLine($"{e.FirstName} , {e.LastName}, {e.City}");
            //}
            //Console.WriteLine("-------------------------");
            //var listaProduktow = await DbA.GetListAsync<Product>(
            //    "GetNProducts",
            //    new { IleRekordowWyswietlic = 100 }
            //    );
            //foreach (Product p in listaProduktow)
            //{
            //    Console.WriteLine($"{p.ProductName}, {p.SupplierID}");
            //}

            //Console.WriteLine("-------------------------");

            //var jedenRekord = await DbA.GetSingleAsync<Product>(
            //    "GetProductById",
            //    new { ProductID = 1111 }
            //    );

            //Console.WriteLine($"{jedenRekord.ProductName} ,{jedenRekord.ProductID}, {jedenRekord.SupplierID}, {jedenRekord.QuantityPerUnit}");

            //Console.WriteLine("-------------------------");

            //var result = await DbA.ExecuteAsync(
            //    "UpdateProductPrice",
            //    new { ProductID = 0, NewUnitPrice = 99.1 }
            //    );
            //Console.WriteLine(result);

            //Console.WriteLine("-------------------------");

            //var result2 = await DbA.ExecuteAsync(
            //    "InsertEmployee",
            //    new { LastName = "John", FirstName = "P", HireDate = new DateTime(2020, 5, 15) }
            //    );
            //Console.WriteLine(result2);
        }
    }
}
