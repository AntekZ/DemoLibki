using DapperTest;
using DatabaseAccess;
using DatabaseAccess.Contracts;
using DatabaseAccess.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
                { "ConnectionString", "Server=ASUSANTEK\\SQLEXPRESS;Database=NORTHWND;Trusted_Connection=True;Encrypt=False;" },
                { "CommandTimeout", "30" }
            })
            .Build();

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<DatabaseAccessService> logger = loggerFactory.CreateLogger<DatabaseAccessService>();

            IDatabaseAccess DbA = new DatabaseAccessService(configuration, logger);


            var lista = await DbA.getListAsync<Employee>(
                "GetEmployees",
                 new { numberOfEmployees = 10 }
                );

            foreach (Employee e in lista)
            {
                Console.WriteLine($"{e.FirstName} , {e.LastName}, {e.City}");
            }
            Console.WriteLine("-------------------------");
            var listaProduktow = await DbA.getListAsync<Product>(
                "GetNProducts",
                new { IleRekordowWyswietlic = 100 }
                );
            foreach (Product p in listaProduktow)
            {
                Console.WriteLine($"{p.ProductName}, {p.SupplierID}");
            }

            Console.WriteLine("-------------------------");

            var jedenRekord = await DbA.getSingleAsync<Product>(
                "GetProductById",
                new { ProductID = 1 }
                );
            Console.WriteLine($"{jedenRekord.ProductName} ,{jedenRekord.ProductID}, {jedenRekord.SupplierID}, {jedenRekord.QuantityPerUnit}");

            Console.WriteLine("-------------------------");

            var result = await DbA.executeAsync(
                "UpdateProductPrice",
                new { ProductID = 0, NewUnitPrice = 99.1 }
                );
            Console.WriteLine(result);

            Console.WriteLine("-------------------------");

            var result2 = await DbA.executeAsync(
                "InsertEmployee",
                new { LastName = "John", FirstName = "Pork", HireDate = new DateTime(2020, 5, 15) }
                );
            Console.WriteLine(result2);
        }
    }
}
