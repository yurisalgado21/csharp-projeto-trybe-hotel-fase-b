using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class CityRepository : ICityRepository
    {
        protected readonly ITrybeHotelContext _context;
        public CityRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public IEnumerable<CityDto> GetCities()
        {
            return _context.Cities.Select(c => new CityDto
            {
                cityId = c.CityId,
                name = c.Name
            }).ToList();
        }

        public CityDto AddCity(City city)
        {
            try
            {
                _context.Cities.Add(city);
                _context.SaveChanges();
                var newCity = _context.Cities.First(c => c.CityId == city.CityId);
                return new CityDto {
                    cityId = newCity.CityId,
                    name = newCity.Name
                };
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

    }
}