using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;
        public BookingRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public BookingResponse Add(BookingDtoInsert booking, string email)
        {
            var room = _context.Rooms.First(r => r.RoomId == booking.RoomId);
            if (booking.GuestQuant > room.Capacity)
            {
                throw new Exception("Guest quantity over room capacity");
            }

            var newBooking = _context.Bookings.Add(new Booking {
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                RoomId = booking.RoomId,
            });
            _context.SaveChanges();
            
            var hotel = _context.Hotels.First(h => h.HotelId == room.HotelId);

            return new BookingResponse
            {
                bookingId = newBooking.Entity.BookingId,
                checkIn = newBooking.Entity.CheckIn,
                checkOut = newBooking.Entity.CheckOut,
                guestQuant = newBooking.Entity.GuestQuant,
                room = new RoomDto {
                    roomId = room.RoomId,
                    name = room.Name,
                    capacity = room.Capacity,
                    image = room.Image,
                    hotel = new HotelDto {
                        hotelId = hotel.HotelId,
                        name = hotel.Name,
                        address = hotel.Address,
                        cityId = hotel.CityId,
                        cityName = _context.Cities.First(c => c.CityId == hotel.CityId).Name
                    }
                }
            };
        }

        public BookingResponse GetBooking(int bookingId, string email)
        {
            throw new NotImplementedException();
        }

        public Room GetRoomById(int RoomId)
        {
            throw new NotImplementedException();
        }

    }

}