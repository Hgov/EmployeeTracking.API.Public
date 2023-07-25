using AutoMapper;
using EmployeeTracking.Core.Entities;
using EmployeeTracking.Core.Entities.DataTransferObjects;

namespace EmployeeTracking.Core.MapperService
{
    public class Profiles:Profile
    {
        public Profiles()
        {
            CreateMap<SignUpModel, User>();

            CreateMap<Role, UpsertRoleModel>();
            CreateMap<User, EditUserModel>();
        }
    }
}
