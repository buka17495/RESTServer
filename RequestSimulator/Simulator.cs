using Newtonsoft.Json;
using System.Text;

namespace RestSimpleServer
{
    public static class Simulator
    {
        private static readonly string[] FirstNames = { "Leia", "Sadie", "Jose", "Sara", "Frank", "Dewey", "Tomas", "Joel", "Lukas", "Carlos" };
        private static readonly string[] LastNames = { "Liberty", "Ray", "Harrison", "Ronan", "Drew", "Powell", "Larsen", "Chan", "Anderson", "Lane" };
        private static int idCounter = 1;

        public static async Task SimulateRequests(string apiUrl, int requestCount)
        {
            var tasks = new List<Task>();
            var random = new Random();

            for (int i = 0; i < requestCount; i++)
            {
                var customers = new List<Customer>
                {
                    CreateRandomCustomer(random),
                    CreateRandomCustomer(random)
                };

                if (i % 2 == 0)
                {
                    customers.Add(CreateRandomCustomer(random));
                    await SendGetRequest(apiUrl + "/customers");
                }

                var task = SendPostRequest(apiUrl + "/customers", customers);
                tasks.Add(task);

                await Task.Delay(random.Next(100, 500));
            }

            await Task.WhenAll(tasks);

            await SendGetRequest(apiUrl + "/customers");
        }

        private static Customer CreateRandomCustomer(Random random)
        {
            var firstName = FirstNames[random.Next(FirstNames.Length)];
            var lastName = LastNames[random.Next(LastNames.Length)];
            var age = random.Next(10, 90);

            return new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Age = age,
                Id = idCounter++
            };
        }

        private static async Task SendPostRequest(string url, List<Customer> customers)
        {
            using var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(customers), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("POST request succeeded.");
            else
                Console.WriteLine("POST request failed: " + response.StatusCode);
        }

        private static async Task SendGetRequest(string url)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(content);

                Console.WriteLine("GET request succeeded. Customers:");
                foreach (var customer in customers!)
                    Console.WriteLine($"Full Name: {customer.FirstName} {customer.LastName}, Age: {customer.Age}, ID: {customer.Id}");
            }
            else
                Console.WriteLine("GET request failed: " + response.StatusCode);
        }
    }
}
