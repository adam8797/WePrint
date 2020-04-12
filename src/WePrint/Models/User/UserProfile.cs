using System;
using AutoMapper;
using AutoMapper.EquivalencyExpression;

namespace WePrint.Models.User
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<UserViewModel, User>();
            CreateMap<Guid, User>().ConvertUsing<EntityConverter<Guid, User>>();
            CreateMap<User, Guid>().ConvertUsing(x => x.Id);
        }
    }
}
