public sealed record Booking (int customerId, int roomId, int guest_count, DateOnly checkIn, DateOnly checkOut);
