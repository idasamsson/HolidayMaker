using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using MySqlConnector;

public class RoomRepository(MySqlConnection connection)
{
    public async Task<List<AvailableRoom>> GetAvailableRoomsAsync(string city, int guests, DateOnly checkIn, DateOnly checkOut, bool pool, bool restaurant, int? maxCityCenter, int? maxBeach, SortOrder sort)
    {
        string orderBy = sort switch
        {
            SortOrder.Rating => "a.rating DESC",
            SortOrder.Price  => "r.price_per_night",
            _                => "r.price_per_night"
        };
        await using var command = new MySqlCommand(
            $"""
            SELECT r.id AS room_id, c.name AS city, a.name AS accommodation_name, r.room_number, r.capacity, r.price_per_night, a.rating AS betyg, a.pool AS pool, a.restaurant AS restaurang, a.city_center_distance AS avstand_till_centrum, a.beach_distance AS avstand_till_strand
            FROM rooms r
            JOIN accommodation a ON a.id = r.accommodation_id
            JOIN cities c ON c.id = a.city_id
            WHERE r.id NOT IN (
                SELECT room_id
                FROM bookings
                WHERE check_out > @checkIn
                    AND check_in < @checkOut)
            AND c.name = @city AND r.capacity >= @guests
            AND (@pool = 0 OR a.pool = 1)
            AND (@restaurant = 0 OR a.restaurant = 1)
            AND (@maxCityCenter IS NULL OR a.city_center_distance <= @maxCityCenter)
            AND (@maxBeach IS NULL OR a.beach_distance <= @maxBeach)
            ORDER BY {orderBy}
            """, connection);
        command.Parameters.AddWithValue("@city", city);
        command.Parameters.AddWithValue("@guests", guests);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);
        command.Parameters.AddWithValue("@pool", pool);
        command.Parameters.AddWithValue("@restaurant", restaurant);
        command.Parameters.AddWithValue("@maxCityCenter", (object?)maxCityCenter ?? DBNull.Value);
        command.Parameters.AddWithValue("@maxBeach", (object?)maxBeach ?? DBNull.Value);

        await command.PrepareAsync();
        await using var reader = await command.ExecuteReaderAsync();

        var availableRooms = new List<AvailableRoom>();
        while (await reader.ReadAsync())
        {
            availableRooms.Add(new AvailableRoom(
                reader.GetInt32("room_id"),
                reader.GetString("accommodation_name"),
                reader.GetString("city"),
                reader.GetInt32("room_number"),
                reader.GetInt32("capacity"),
                reader.GetInt32("price_per_night")));
        }
        return availableRooms;
    }

    public async Task<int?> GetRoomCapacityAsync(int roomId)
    {
        await using var command = new MySqlCommand(
            "SELECT capacity FROM rooms WHERE id = @roomId", connection);
        command.Parameters.AddWithValue("@roomId", roomId);
        
        var result = await command.ExecuteScalarAsync();
        if (result is null)
        {
            return null;
        }
        return Convert.ToInt32(result);
    }

    public async Task<int?> GetRoomPriceAsync (int roomId)
    {
        await using var command = new MySqlCommand(
            "SELECT price_per_night FROM rooms WHERE id = @roomId", connection);
        command.Parameters.AddWithValue("@roomId", roomId);

        var result = await command.ExecuteScalarAsync();
        if(result is null)
        {
            return null;
        }
        return Convert.ToInt32(result);
    }
}


