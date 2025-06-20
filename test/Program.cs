using DapperTest;
using DatabaseAccess;
using DatabaseAccess.Contracts;
using DatabaseAccess.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
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
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            IConnectionFactory factory = new SqlConnectionFactory(configuration);

            IDatabaseAccess DbA = new DatabaseAccessService(factory, configuration, logger,cache);

            //    var employees = await DbA.GetListAsync<Employee>("GetAllEmployees");

            //    foreach (var employee in employees)
            //    {

            //        Console.WriteLine($"{employee.FirstName}, {employee.LastName}");
            //    }

            //DbA.OnDisconnect += (sender, args) =>{
            //Console.WriteLine($"Połączenie zerwane {args.When}");
            //for(int i = 0; i<10;i++)
            //{
            //    Console.WriteLine("c");
            //}
            //    };

            //var conn = new SqlConnection("Server=ASUSANTEK\\SQLEXPRESS;Database=NORTHWND;Trusted_Connection=True;Encrypt=False;");
            //conn.Open();
            ////conn.Close();
            //var result = await DbA.ExecuteAsync("UpdateEmployeeLastName", connection:conn,parameters: new {NewLastName ="Jaro" ,EmployeeID = 1});
            //Console.WriteLine(result);

            //var employees = await DbA.GetListWithCacheAsync(cacheKey: "all_empoloyees",
            //fetch: () => DbA.GetListAsync<Employee>("GetAllEmployees"),
            //expiration: TimeSpan.FromMinutes(10));

            //foreach (Employee e in employees)
            //{
            //    Console.WriteLine(e.FirstName);
            //}

            

            Console.WriteLine("1---------------------------------------------------------------");
            var employees = await DbA.GetListWithCacheAsync<Employee>(cacheKey: "all_empoloyees",
            fetch: () => DbA.GetListAsync<Employee>("GetAllEmployees"),
            expiration: TimeSpan.FromMinutes(10));

            foreach (Employee emp in employees)
            {
                Console.WriteLine($"{emp.LastName}, {emp.FirstName} , {emp.Address}, {emp.BirthDate}, {emp.City}");
            }
            Console.WriteLine("2-----------------------------------------------------------------------------");

            var employees2 = await DbA.GetListWithCacheAsync<Employee>(cacheKey:"all_empoloyees");
           
            foreach (Employee emp in employees2)
            {
                Console.WriteLine($"{emp.LastName}, {emp.FirstName} , {emp.Address}, {emp.BirthDate}, {emp.City}");
            }
            
            //var conn2 = new SqlConnection("Server=ASUSANTEK\\SQLEXPRESS;Database=NORTHWND;Trusted_Connection=True;Encrypt=False;");
            //conn2.Open();
            //var result2 = await DbA.GetListAsync<Employee>("WaitThirtySeconds", connection: conn2);
            //Console.WriteLine(result2);
            //conn.Open();

            //var employeesTest = await DbA.GetListAsync<Employee>("GetAllEmployees", connection: conn);
            //foreach (var employee1 in employeesTest)
            //{

            //    Console.WriteLine($"{employee1.FirstName}, {employee1.LastName}");
            //}
            //conn.Dispose();

            //var lista = await DbA.GetListAsync<Employee>("s", new { numberOfEmployees = 10 });




            //var lista2 = await DbA.GetListAsync<Employee>("GetAllEmployees");

            //foreach (Employee e in lista2)
            //{
            //    Console.WriteLine($"{e.FirstName} , {e.LastName}, {e.City}");
            //}
            //Console.WriteLine("1------------------------");
            //var listaProduktow = await DbA.GetListAsync<Product>(
            //    "GetNProducts",
            //    new { IleRekordowWyswietlic = 100 }
            //    );
            //foreach (Product p in listaProduktow)
            //{
            //    Console.WriteLine($"{p.ProductName}, {p.SupplierID}");
            //}

            //Console.WriteLine("2------------------------");

            //var jedenRekord = await DbA.GetSingleAsync<Product>(
            //    "GetProductById",
            //    new { ProductID = 1111 }
            //    );
            //if( jedenRekord == null )
            //    Console.WriteLine("jest pusty");

            ////Console.WriteLine($"{jedenRekord.ProductName} ,{jedenRekord.ProductID}, {jedenRekord.SupplierID}, {jedenRekord.QuantityPerUnit}");

            //DbA.OnDisconnect += (s, args) =>
            //{
            //    Console.WriteLine($"DISCONNECTED at {args.When:u}, reason: {args.Reason.Message}");
            //};

            //Console.WriteLine("3------------------------");
            //conn.Close();
            //var result = await DbA.ExecuteAsync(
            //    "UpdateProductPrice",
            //    new { ProductID = 0, NewUnitPrice = 99.1 },
            //    conn
            //    );


            //Console.WriteLine(result);


            //Console.WriteLine("4------------------------");

            //var result2 = await DbA.ExecuteAsync(
            //    "InsertEmployee",
            //    new { LastName = "John", FirstName = "P", HireDate = new DateTime(2020, 5, 15) }
            //    );
            //Console.WriteLine(result2);
        }
    }
}
