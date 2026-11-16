public static class Helpers
{
    // hjälpmetod som kontrollerar inmatning av nummer
public static int ReadInt(string prompt)
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
public static DateOnly ReadDate(string prompt)
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
public static string ReadText(string prompt)
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
public static void PrintBooking(BookingSummary booking)
{
   Console.Write($"\nBokningsnummer {booking.BookingId}. {booking.AccommodationName} rumsnummer {booking.RoomNumber}: ");
   Console.Write($"{booking.CheckIn} - {booking.CheckOut}, ");
   Console.Write($"antal gäster {booking.GuestCount}, totalt pris: {booking.TotalPrice}kr.\n");
}

//hjälpmetod för ja/nej
public static bool ReadBool(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine()?.Trim().ToLower();

        if (input == "ja")
        {
            return true;
        }
        else if (input == "nej")
        {
            return false;
        }
        else
        {
            Console.WriteLine("Svara ja eller nej");
        }
    }
}

// hjälpmetod för möjlighet till null
public static int? ReadOptionalInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }
        else if (int.TryParse(input, out int number))
        {
            return number;
        }
        else
        {
            Console.WriteLine("Skriv ett heltal eller tryck enter för inget");
        }
    }
}
}

