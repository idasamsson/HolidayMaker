using MySqlConnector;
using static Helpers;

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
string? selectedCustomerName = null;

while (true)
{
    Console.WriteLine();
    if (selectedCustomerId == null)
    {
        
        Console.WriteLine("\n=== Holiday Maker ===  Ingen kund vald  ===");
    }
    else
    {
        Console.WriteLine($"\n=== Holiday Maker ===  Vald kund: {selectedCustomerId}. {selectedCustomerName}  === ");
    }
        Console.WriteLine("1. Välj eller registrera kund");
        Console.WriteLine("2. Sök och boka rum");
        Console.WriteLine("3. Visa kundens bokningar");
        Console.WriteLine("4. Uppdatera bokning");
        Console.WriteLine("5. Avboka");
        Console.WriteLine("0. Avsluta");
    int choice = ReadInt("Val: ");


    switch (choice)
    {

        // Välj befintlig kund eller registrera en ny
        case 1:
        {
            // välj befintlig kund (ReadInt + GetCustomerByIdAsync) eller registrera ny (ReadText)
            Console.WriteLine("1. Befintlig kund");
            Console.WriteLine("2. Registrera ny kund");
            int customerChoice = ReadInt("Val: ");

            switch (customerChoice)
            {
                // hämta en befintlig kund
                case 1:
                {
                    int id = ReadInt("Kund-id: ");
                    Customer? foundCustomer = await customerRepository.GetCustomerByIdAsync(id);
                    if (foundCustomer is null)
                        {
                            Console.WriteLine("Hittar ingen kund med det id:t");
                        }
                    else
                        {
                            selectedCustomerId = id;
                            selectedCustomerName = $"{foundCustomer.FirstName} {foundCustomer.LastName}";
                            Console.WriteLine($"Vald kund: {foundCustomer.FirstName} {foundCustomer.LastName}");
                        }
                    break;
                }

                // lägg till en ny kund
                case 2:
                {
                    string firstName = ReadText("Förnamn: ");
                    string lastName = ReadText("Efternamn: ");
                    string email = ReadText("Epostadress: ");
                    string phoneNumber = ReadText("Telefonnummer: ");
                    selectedCustomerId = await customerRepository.AddCustomerAsync(firstName, lastName, email, phoneNumber);
                    selectedCustomerName = $"{firstName} {lastName}";
                    Console.WriteLine($"Skapad kund med id: {selectedCustomerId}");
                    break;
                }
                default:
                    Console.WriteLine("Ogiltigt val.");
                    break;
            }         
            break;
        }

        // sök efter lediga rum
        case 2:
        {
            string city = ReadText("Ort(Malaga, Palma, Split): ");
            int guests = ReadInt("Antal gäster: ");
            DateOnly checkIn = ReadDate("Incheckningsdatum: ");
            DateOnly checkOut = ReadDate("Utcheckningsdatum: ");

            // Samma kontroll som i BookRoomAsync: utcheckning måste ligga efter incheckning
            if (checkOut <= checkIn)
            {
                Console.WriteLine("Utcheckning måste ligga efter incheckning.");
                break;
            }

            bool pool = ReadBool("Pool (ja/nej): ");
            bool restaurant = ReadBool("Restaurang (ja/nej): ");
            int? distanceCityCenter = ReadOptionalInt("Max avstånd till centrum i meter (Enter för inget): ");
            int? distanceBeach = ReadOptionalInt("Max avstånd till strand i meter (Enter för inget): ");
            int sortChoice = ReadInt("Sortera (1 = pris/natt, 2 = betyg): ");
            SortOrder sort = sortChoice == 2 ? SortOrder.Rating : SortOrder.Price;
            
            // GetAvailableRoomsAsync och skriv ut resultatet
            List<AvailableRoom> rooms = await roomRepository.GetAvailableRoomsAsync(city, guests, checkIn, checkOut, pool, restaurant, maxCityCenter: distanceCityCenter, maxBeach: distanceBeach, sort: sort);

            if (rooms.Count == 0)
            {
                Console.WriteLine("Inga lediga rum hittades.");
                break;
            }
            int nights = checkOut.DayNumber - checkIn.DayNumber;
            foreach (AvailableRoom room in rooms)
            {
                Console.WriteLine($"Rumsid: {room.RoomId}. {room.AccommodationName}, {room.City}, rum {room.RoomNumber}. Antal gäster: {guests} Antal nätter: {nights} Totalpris: {nights*room.PricePerNight}");
            }
            if (selectedCustomerId is null) 
            {   
                Console.WriteLine("Välj en kund (val 1) om du vill boka något av rummen."); 
                break;
            }
            // ReadInt (rum-id), ReadInt (gäster), ReadDate (in och ut)
            if(ReadOptionalInt("Ange rums-id för att boka (Enter för att avbryta): ") is not int roomId)
                {
                    Console.WriteLine("Sökningen avbröts.");
                    break;
                }
            if (!rooms.Exists(r => r.RoomId == roomId))
                {
                    Console.WriteLine("Rummet fanns ej i sökresultatet");
                    break;
                }
            // Boka rum
            await BookRoomAsync(roomRepository, bookingRepository, selectedCustomerId.Value, roomId, guests, checkIn, checkOut);
            break;
        }

        // visa bokningar på kund-id.
        case 3:
            {
                if (selectedCustomerId is null) {Console.WriteLine("Välj en kund först (val 1)."); break;}
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

        // uppdatera bokning
        case 4:
        {
            if (selectedCustomerId is null) { Console.WriteLine("Välj en kund först (val 1)."); break; }

             // visa kundens bokningar
                List<BookingSummary> bookings = await bookingRepository.GetBookingsByCustomerAsync(selectedCustomerId.Value);
                if (bookings.Count == 0)
                {
                    Console.WriteLine("Du har inga bokningar.");
                    break;
                }
                else
                {
                    Console.WriteLine("Dina bokningar:");
                foreach (BookingSummary booking in bookings)
                    {
                        PrintBooking(booking);
                    }
                }
                
            // uppdatera en bokning
            int bookingId = ReadInt("Bokningsnummer: ");
            int guestCount = ReadInt("Uppdatera antal gäster: ");
            if (guestCount <= 0)
                {
                    Console.WriteLine("Gästantalet måste vara större än noll.");
                    break;
                }
            int rowsUpdated = await bookingRepository.UpdateBookingAsync(bookingId, guestCount, selectedCustomerId.Value);
            if (rowsUpdated == 1) 
                {
                    Console.WriteLine($"Bokning {bookingId} är uppdaterad till {guestCount} gäst/gäster.");
                }
            else 
            {
                Console.WriteLine($"Hittar ingen bokning {bookingId} för den kunden eller så är rymmer inte rummet {guestCount} gäster.");
            }
            break;
        }
        
        // avboka
        case 5:
        {
            if (selectedCustomerId is null) { Console.WriteLine("Välj en kund först (val 1)."); break; }
            // visa kundens bokningar, ReadInt (bokningsnummer), CancelBookingAsync
                List<BookingSummary> bookings = await bookingRepository.GetBookingsByCustomerAsync(selectedCustomerId.Value);
                if (bookings.Count == 0)
                {
                    Console.WriteLine("Du har inga bokningar.");
                    break;
                }
                else
                {
                    Console.WriteLine("Dina bokningar:");
                foreach (BookingSummary booking in bookings)
                    {
                        PrintBooking(booking);
                    }
                }
                
            // radera en bokning
            int bookingId = ReadInt("Bokningsnummer: ");
            int rowsDeleted = await bookingRepository.CancelBookingAsync(bookingId, selectedCustomerId.Value);
            if (rowsDeleted == 1) 
                {
                    Console.WriteLine($"Bokning {bookingId} är avbokad.");
                }
            else 
            {
                Console.WriteLine($"Hittar ingen bokning {bookingId} för den kunden.");
            }

            break;
        }

        case 0:
            return;

        default:
            Console.WriteLine("Ogiltigt val.");
            break;
    }
}
/*
// 2. READ: hämta alla kunder som en lista.
List<Customer> customers = await customerRepository.GetCustomersAsync();
Console.WriteLine("Alla kunder:");
foreach (Customer customer in customers)
{
    Console.WriteLine($"{customer.CustomerId}: {customer.FirstName} {customer.LastName} - {customer.Email}, {customer.Phone}");
}
*/

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
     if (!await bookingRepository.IsRoomAvailableAsync(roomId, checkIn, checkOut))
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
