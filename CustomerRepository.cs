using System.Data;
using MySqlConnector;

public class CustomerRepository(MySqlConnection connection)
{
// READ: flera rader → en lista med objekt.
     public async Task<List<Customer>> GetCustomersAsync()
    {
        await using var command = new MySqlCommand(
            "SELECT id, first_name, last_name, email, phone FROM customers", connection);
        await using var reader = await command.ExecuteReaderAsync();

        var customers = new List<Customer>();
        while (await reader.ReadAsync())
        {
            customers.Add(new Customer(
                reader.GetInt32("id"),
                reader.GetString("first_name"),
                reader.GetString("last_name"),
                reader.GetString("email"),
                reader.GetString("phone")));
        }
        return customers;
    }

// ADD: Lägg till en ny kund
        public async Task<int> AddCustomerAsync(string firstName, string lastName, string email, string phone)
    {
        await using var command = new MySqlCommand(
            "INSERT INTO customers (first_name, last_name, email, phone) VALUES (@first_name, @last_name, @email, @phone);", connection);
        command.Parameters.AddWithValue("@first_name", firstName);
        command.Parameters.AddWithValue("@last_name", lastName);
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@phone", phone);
        await command.PrepareAsync();
        await command.ExecuteNonQueryAsync();
        return checked((int)command.LastInsertedId);
    }

    
    // READ: en rad → ett objekt, eller null när id saknas.
    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        await using var command = new MySqlCommand(
            "SELECT id, first_name, last_name, email, phone FROM customers WHERE id = @id;", connection);
        command.Parameters.AddWithValue("@id", id);
        await command.PrepareAsync();
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Customer(
                reader.GetInt32("id"),
                reader.GetString("first_name"),
                reader.GetString("last_name"),
                reader.GetString("email"),
                reader.GetString("phone"));
        }
        return null;
    }


}
