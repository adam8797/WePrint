using AutoMapper;

namespace WePrint.Models.User
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
        }
    }
}
