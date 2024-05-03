using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class UserRepository : IUserRepository
    {
        protected readonly ITrybeHotelContext _context;
        public UserRepository(ITrybeHotelContext context)
        {
            _context = context;
        }
        public UserDto GetUserById(int userId)
        {
            var user =  _context.Users.First(u => u.UserId == userId);
            return new UserDto {
                userId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                userType = user.UserType
            };
        }

        public UserDto Login(LoginDto login)
        {
            var foundUser = _context.Users.FirstOrDefault(u => u.Email == login.Email && u.Password == login.Password);
            if (foundUser == null)
            {
                return null!;
            }

            return new UserDto{
                userId = foundUser.UserId,
                Name = foundUser.Name,
                Email = foundUser.Email,
                userType = foundUser.UserType
            };
        }
        public UserDto Add(UserDtoInsert user)
        {
            try
            {
                var newUser = new User {
                    Name = user.Name,
                    Email = user.Email,
                    Password = user.Password,
                    UserType = "client"
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                var foundUser = _context.Users.First(u => u.Email == newUser.Email && u.Password == newUser.Password);

                return new UserDto {
                    userId = foundUser.UserId,
                    Name = foundUser.Name,
                    Email = foundUser.Email,
                    userType = foundUser.UserType
                };
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public UserDto GetUserByEmail(string userEmail)
        {
            var foundUser = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (foundUser == null)
            {
                return null!;
            }

            return new UserDto{
                userId = foundUser.UserId,
                Name = foundUser.Name,
                Email = foundUser.Email,
                userType = "client"
            };
        }

        public IEnumerable<UserDto> GetUsers()
        {
            return _context.Users.Select(u => new UserDto {
                userId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                userType = u.UserType
            }).ToList();
        }
    }
}