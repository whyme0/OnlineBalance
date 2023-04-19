using OnlineBalance.Models;
using static System.Globalization.CultureInfo;

namespace OnlineBalance.Mapping
{
    public class UserMapping
    {
        public User MapUserRegistration(User user, CreateUserDTO userDTO)
        {
            (user.FirstName, user.LastName, user.BirthDate, user.Email, user.UserName) 
                = (
                CurrentCulture.TextInfo.ToTitleCase(userDTO.FirstName),
                CurrentCulture.TextInfo.ToTitleCase(userDTO.LastName),
                userDTO.BirthDate,
                userDTO.Email,
                userDTO.Email.Split('@')[0]); // username
            return user;
        }
    }
}
