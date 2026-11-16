public sealed record BookingSummary (int BookingId, string AccommodationName, int RoomNumber, DateOnly CheckIn, DateOnly CheckOut, int GuestCount, int TotalPrice);
