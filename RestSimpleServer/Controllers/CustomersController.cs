using Microsoft.AspNetCore.Mvc;

namespace RestSimpleServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly List<Customer> customers;

        public CustomersController()
        {
            customers = CustomersControllerHelpers.LoadCustomersFromStorage();
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomers(List<Customer> newCustomers)
        {
            foreach (var customer in newCustomers)
            {
                if (Validate(customer))
                    return BadRequest("Invalid customer data.");

                InsertCustomer(customer);
            }

            await CustomersControllerHelpers.SaveCustomersToStorage(customers);

            return Ok();
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {
            return Ok(customers);
        }

        private bool Validate(Customer customer)
        {
            return string.IsNullOrWhiteSpace(customer.FirstName) ||
                string.IsNullOrWhiteSpace(customer.LastName) ||
                customer.Age <= 18 ||
                customers.Any(c => c.Id == customer.Id);
        }

        private void InsertCustomer(Customer newCustomer)
        {
            var index = 0;
            while (index < customers.Count && CompareCustomers(newCustomer, customers[index]) > 0)
                index++;

            customers.Insert(index, newCustomer);
        }

        private static int CompareCustomers(Customer c1, Customer c2)
        {
            int lastNameComparison = string.Compare(c1.LastName, c2.LastName, StringComparison.OrdinalIgnoreCase);
            if (lastNameComparison != 0)
                return lastNameComparison;

            return string.Compare(c1.FirstName, c2.FirstName, StringComparison.OrdinalIgnoreCase);
        }
    }
}