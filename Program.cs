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

int? selectedCustomerId = null;   // vald kund, null = ingen vald än

while (true)
{
    Console.WriteLine(($"{selectedCustomerId}"));
    Console.WriteLine();
    Console.WriteLine("\n=== Holiday Maker ===");
    Console.WriteLine("1. Välj eller registrera kund");
    Console.WriteLine("2. Sök lediga rum");
    Console.WriteLine("3. Boka rum");
    Console.WriteLine("4. Visa kundens bokningar");
    Console.WriteLine("5. Avboka");
    Console.WriteLine("0. Avsluta");

    int choice = ReadInt("Val: ");

    switch (choice)
    {
        case 1:
        {
            // välj befintlig kund (ReadInt + GetCustomerByIdAsync) eller registrera ny (ReadText)
            Console.WriteLine("1. Befintlig kund");
            Console.WriteLine("2. Registrera ny kund");
            int customerChoice = ReadInt("Val: ");

            switch (customerChoice)
            {
                case 1:
                {
                    int id = ReadInt("Kund-id: ");
                    // READ: hämta en kund som ett enstaka objekt.
                    Customer? foundCustomer = await customerRepository.GetCustomerByIdAsync(id);
                    if (foundCustomer is null)
                        {
                            Console.WriteLine("Hittar ingen kund med det id:t");
                        }
                    else
                        {
                            selectedCustomerId = id;
                            Console.WriteLine($"Vald kund: {foundCustomer.FirstName} {foundCustomer.LastName}");
                        }
                    break;
                }


                case 2:
                {
                        // ADD: lägg till en ny kund och ta emot databasens nya id.
                    string firstName = ReadText("Förnamn: ");
                    string lastName = ReadText("Efternamn: ");
                    string email = ReadText("Epostadress: ");
                    string phoneNumber = ReadText("Telefonnummer: ");
                    selectedCustomerId = await customerRepository.AddCustomerAsync(firstName, lastName, email, phoneNumber);
                    Console.WriteLine($"Skapad kund med id: {selectedCustomerId}");
                    break;
                }
                default:
                    Console.WriteLine("Ogiltigt val.");
                    break;
            }         
            break;
        }
        case 2:
            // ReadText (ort), ReadDate (in och ut), ReadInt (antal personer)
            // GetAvailableRoomsAsync och skriv ut resultatet
            break;

        case 3:
            if (selectedCustomerId is null)
            {
                Console.WriteLine("Välj en kund först (val 1).");
                break;
            }
            // ReadInt (rum-id), ReadInt (gäster), ReadDate (in och ut)
            // await BookRoomAsync(..., selectedCustomerId.Value, ...)
            break;

        case 4:
            {
                if (selectedCustomerId is null) { Console.WriteLine("Välj en kund först (val 1)."); break; }
                // Hämta alla bokningar på ett kund-id
                List<BookingSummary> bookings = await bookingRepository.GetBookingsByCustomerAsync(selectedCustomerId.Value);
                if (bookings.Count == 0)
                {
                    Console.WriteLine("Du har inga bokningar.");
                    break;
                }
                Console.WriteLine("Dina bokningar:");
                foreach (BookingSummary booking in bookings)
                    {
                        PrintBooking(booking);
                    }
                break;
            }

        case 5:
        {
            if (selectedCustomerId is null) { Console.WriteLine("Välj en kund först (val 1)."); break; }
            // visa kundens bokningar, ReadInt (bokningsnummer), CancelBookingAsync
        }
        case 0:
            return;

        default:
            Console.WriteLine("Ogiltigt val.");
            break;
    }
}

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


Console.WriteLine(await bookingRepository.IsRoomAvailableAsync(1, "2026-12-12", "2026-12-15")); // False
Console.WriteLine(await bookingRepository.IsRoomAvailableAsync(1, "2026-12-09", "2026-12-12")); // True

// Skapa en bokning
static async Task BookRoomAsync(RoomRepository roomRepository, BookingRepository bookingRepository, int customerId, int roomId, int guests, DateOnly checkIn, DateOnly checkOut)
{
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
    int bookingId = await bookingRepository.CreateBookingAsync(customerId, roomId, guests, checkIn, checkOut);
    Console.WriteLine($"Ditt bokningsnummer är: {bookingId}. Din bokning gäller för {nights} nätter och priset blir totalt {nights*price.Value}kr.");
}

//await BookRoomAsync(roomRepository, bookingRepository, 1, 20, 2,new DateOnly(2026, 12, 12), new DateOnly(2026, 12, 15));

// radera en bokning
int bookingId = 33;
int rowsDeleted = await bookingRepository.CancelBookingAsync(bookingId, 8);
if (rowsDeleted == 1) {Console.WriteLine($"Bokning {bookingId} är avbokad.");}
else {Console.WriteLine($"Hittar ingen bokning {bookingId} för den kunden.");}

// hjälpmetod som kontrollerar inmatning av nummer
static int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);

        if (!int.TryParse(Console.ReadLine(), out int number))
        {
            Console.WriteLine("Ogiltig inmatning. Skriv ett heltal.");
        }
        else
        {
            return number;   
        }
    }
}

// hjälpmetod som kontrollerar inmatning av datum
static DateOnly ReadDate(string prompt)
{
    while (true)
    {
        Console.Write(prompt);

        if (!DateOnly.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", out DateOnly date))
        {
            Console.WriteLine("Ogiltig inmatning. Skriv datum i formatet yyyy-MM-dd.");
        }
        else
        {
            return date;
        }
    }
}

// hjälpmetod som kontrollerar inmatning av text
static string ReadText(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Fältet får inte vara tomt.");
        }
        else
        {
            return input.Trim();
        }
    }
}

//hjälpmetod  utskrift i konsol
static void PrintBooking(BookingSummary booking)
{
   Console.Write($"\nBokningsnummer {booking.BookingId}. {booking.AccommodationName} rumsnummer {booking.RoomNumber}: ");
   Console.Write($"{booking.CheckIn} - {booking.CheckOut}, ");
   Console.Write($"antal gäster {booking.GuestCount}, totalt pris: {booking.TotalPrice}kr.");
}