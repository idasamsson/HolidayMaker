using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using MySqlConnector;

// ANSLUTNING TILL DATABASEN
var password = Environment.GetEnvironmentVariable("HOLIDAY_DB_PASSWORD")
    ?? throw new InvalidOperationException("Lösenordet saknas.");

var builder = new MySqlConnectionStringBuilder
{
    Server = "localhost",
    Port = 3306,                 // samma port som din root-anslutning
    Database = "holiday_maker",
    UserID = "holly",
    Password = password,
    UseAffectedRows = false
};

await using var connection = new MySqlConnection(builder.ConnectionString);
await connection.OpenAsync();

// REPOSITORY (klass med databas-queries)
var roomRepository = new RoomRepository(connection);
var customerRepository = new CustomerRepository(connection);
var bookingRepository = new BookingRepository(connection);

/*
// 1. READ: hämta alla tillgängliga rum som en lista.
List<AvailableRoom> availableRooms = await roomRepository.GetAvailableRoomsAsync("Palma", "2026-12-12", "2026-12-15", 2);
Console.WriteLine("Alla lediga rum:");
foreach (AvailableRoom availableRoom in availableRooms)
{
    Console.WriteLine($"{availableRoom.AccommodationName} i {availableRoom.City}, rum {availableRoom.RoomNumber}: tillgängligt för {availableRoom.Capacity} gäster. Pris/natt: {availableRoom.PricePerNight}kr");
}

// 2. READ: hämta alla kunder som en lista.
List<Customer> customers = await customerRepository.GetCustomersAsync();
Console.WriteLine("Alla kunder:");
foreach (Customer customer in customers)
{
    Console.WriteLine($"{customer.CustomerId}: {customer.FirstName} {customer.LastName} - {customer.Email}, {customer.Phone}");
}

// ADD: lägg till en ny kund och ta emot databasens nya id.
int newId = await customerRepository.AddCustomerAsync("Allan", "Samsson", "allan@example.com", "070-2345678");
Console.WriteLine($"Skapad kund med id: {newId}");


// READ: hämta en kund som ett enstaka objekt.
Customer? customer = await customerRepository.GetCustomerByIdAsync(14);
Console.WriteLine(customer is null ? "Hittar ingen kund med det id:t" : customer.ToString());


Console.WriteLine(await bookingRepository.IsRoomAvailableAsync(1, "2026-12-12", "2026-12-15")); // False
Console.WriteLine(await bookingRepository.IsRoomAvailableAsync(1, "2026-12-09", "2026-12-12")); // True

static async Task BookRoomAsync(RoomRepository roomRepository, BookingRepository bookingRepository, int customerId, int roomId, int guests, string checkInText, string checkOutText)
{
    // ogiltigt datum
    if (!DateOnly.TryParse(checkInText, out DateOnly checkIn) ||
        !DateOnly.TryParse(checkOutText, out DateOnly checkOut)) 
    {
        Console.WriteLine("Ogiltigt datum. Skriv i formatet åååå-MM-dd.");
        return;
    }
    if (checkOut <= checkIn)
    {
        Console.WriteLine("Utcheckning måste ligga efter incheckning.");
        return;
    }
    
    int? capacity = await roomRepository.GetRoomCapacityAsync(roomId);

        if (capacity is null)
        {
            Console.WriteLine("Det finns inget rum med det id:t.");
            return;
        }
        
        if (guests > capacity)
        {
            Console.WriteLine("Gästantalet överstiger rummets kapacitet");
            return;
        }
     if (!await bookingRepository.IsRoomAvailableAsync(roomId, checkIn.ToString("yyyy-MM-dd"), checkOut.ToString("yyyy-MM-dd")))
    {
        Console.WriteLine("Rummet är upptaget under perioden.");
        return;
    }

    int nights = checkOut.DayNumber - checkIn.DayNumber;
    int? price = await roomRepository.GetRoomPriceAsync(roomId);
        if (price is null)
        {
            Console.WriteLine("Priset hittas inte");
            return;
        }
    int bookingId = await bookingRepository.CreateBookingAsync(customerId, roomId, guests, checkIn.ToString("yyyy-MM-dd"), checkOut.ToString("yyyy-MM-dd"));
    Console.WriteLine($"Ditt bokningsnummer är: {bookingId}. Din bokning gäller för {nights} nätter och priset blir totalt {nights*price.Value}kr.");
}

await BookRoomAsync(roomRepository, bookingRepository, 1, 20, 2, "2026-12-12", "2026-12-15");
*/

// Hämta alla bokningar på ett kund-id
List<BookingSummary> bookings = await bookingRepository.GetBookingsByCustomerAsync(5);
Console.WriteLine("Dina bokningar:");
foreach (BookingSummary booking in bookings)
{
    Console.WriteLine($"Bokningsnummer {booking.BookingId}. {booking.AccommodationName} rumsnummer {booking.RoomNumber}: {booking.CheckIn} - {booking.CheckOut}, antal gäster {booking.GuestCount}, totalt pris: {booking.TotalPrice}kr.");
}