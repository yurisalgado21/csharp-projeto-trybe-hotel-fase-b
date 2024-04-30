using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class HotelRepository : IHotelRepository
    {
        protected readonly ITrybeHotelContext _context;
        public HotelRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public IEnumerable<HotelDto> GetHotels()
        {
            return _context.Hotels.Select(h => new HotelDto
            {
                hotelId = h.HotelId,
                name = h.Name,
                address = h.Address,
                cityId = h.CityId,
                cityName = h.City!.Name
            }).ToList();
        }
        
        public HotelDto AddHotel(Hotel hotel)
        {
            try
            {
                _context.Hotels.Add(hotel);
                _context.SaveChanges();

                var newHotel = _context.Hotels.First(h => h.HotelId == hotel.HotelId);

                newHotel.City = _context.Cities.First(c => c.CityId == hotel.CityId);

                return new HotelDto {
                    hotelId = newHotel.HotelId,
                    name = newHotel.Name,
                    address = newHotel.Address,
                    cityId = newHotel.CityId,
                    cityName = newHotel.City!.Name
                };
            }
            catch (Exception err)
            {

                throw new Exception("An error occurred while saving the entity changes. See the inner exception for details.", err);
            }
        }
    }
}