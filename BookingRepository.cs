using MySqlConnector;

public class BookingRepository(MySqlConnection connection)
{
// Kollar om ett rum är ledigt
    public async Task<bool> IsRoomAvailableAsync(int roomId, string checkIn, string checkOut)
    {
        await using var command = new MySqlCommand(
            "SELECT COUNT(*) FROM bookings WHERE room_id = @roomId AND check_out > @checkIn AND check_in < @checkOut", connection);
        command.Parameters.AddWithValue("@roomId", roomId);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);

        await command.PrepareAsync();
        var result = await command.ExecuteScalarAsync();
        long overlapping = Convert.ToInt64(result);
        return overlapping == 0;
    }

// Skapa en bokning
    public async Task<int> CreateBookingAsync(int customerId, int roomId, int guests, string checkIn, string checkOut)
    {
        await using var command = new MySqlCommand(
            "INSERT INTO bookings (customer_id, room_id, guest_count, check_in, check_out) VALUES (@customer_id, @room_id, @guest_count, @check_in, @check_out)", 
            connection);
        command.Parameters.AddWithValue("@customer_id", customerId);
        command.Parameters.AddWithValue("@room_id", roomId);
        command.Parameters.AddWithValue("@guest_count", guests);
        command.Parameters.AddWithValue("@check_in", checkIn);
        command.Parameters.AddWithValue("@check_out", checkOut);

        await command.PrepareAsync();
        await command.ExecuteNonQueryAsync();
        return checked((int)command.LastInsertedId);
    }

// visa en kunds bokningar
public async Task<List<booking
}
