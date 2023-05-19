using Newtonsoft.Json;

namespace RestSimpleServer.Controllers
{
    public class CustomersControllerHelpers
    {
        private static readonly string storageFilePath = "customers.json";

        public static List<Customer> LoadCustomersFromStorage()
        {
            if (File.Exists(storageFilePath))
            {
                var json = File.ReadAllText(storageFilePath);
                return JsonConvert.DeserializeObject<List<Customer>>(json)!;
            }
            else
                return new List<Customer>();
        }

        public static async Task SaveCustomersToStorage(List<Customer> customers)
        {
            var json = JsonConvert.SerializeObject(customers);
            await File.WriteAllTextAsync(storageFilePath, json);
        }
    }
}