using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class RoomRepository : IRoomRepository
    {
        protected readonly ITrybeHotelContext _context;
        public RoomRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public IEnumerable<RoomDto> GetRooms(int HotelId) {
            var hotel = _context.Hotels.First(h => h.HotelId == HotelId);

            hotel.City = _context.Cities.First(c => c.CityId == hotel.CityId);

            return _context.Rooms.Select(r => new RoomDto
            {
                roomId = r.RoomId,
                name = r.Name,
                capacity = r.Capacity,
                image = r.Image,
                hotel = new HotelDto {
                    hotelId = hotel.HotelId,
                    name = hotel.Name,
                    address = hotel.Address,
                    cityId = hotel.CityId,
                    cityName = hotel.City!.Name
                }
            }).ToList();
        }

        public RoomDto AddRoom(Room room) {
            var hotel = _context.Hotels.First(h => h.HotelId == room.HotelId);
            hotel.City = _context.Cities.First(c => c.CityId == hotel.CityId);

            _context.Rooms.Add(room);
            _context.SaveChanges();

            var newRoom = _context.Rooms.First(r => r.RoomId == room.RoomId);

            return new RoomDto {
                roomId = newRoom.RoomId,
                name = newRoom.Name,
                capacity = newRoom.Capacity,
                image = newRoom.Image,
                hotel = new HotelDto {
                    hotelId = hotel.HotelId,
                    name = hotel.Name,
                    address = hotel.Address,
                    cityId = hotel.CityId,
                    cityName = hotel.City.Name
                }
            };
        }

        public void DeleteRoom(int RoomId) {
            var room = _context.Rooms.First(r => r.RoomId == RoomId);
            _context.Rooms.Remove(room);
        }
    }
}