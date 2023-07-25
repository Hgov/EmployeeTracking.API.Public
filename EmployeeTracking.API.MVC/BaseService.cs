using AutoMapper;
using EmployeeTracking.Core.EmailService;
using EmployeeTracking.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace EmployeeTracking.API.MVC
{
    public abstract class BaseService : Controller
    {
        public readonly UserManager<User> _userManager;
        public readonly RoleManager<Role> _roleManager;
        public readonly SignInManager<User> _signInManager;
        public IActionContextAccessor _actionContextAccessor;
        public IUrlHelperFactory _urlHelperFactory;
        public readonly IMapper _mapper;
        public EmailHelper _emailHelper;
        public BaseService(IMapper mapper)
        {

            _mapper = mapper;
        }
        public BaseService(IMapper mapper, IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, EmailHelper emailHelper)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _mapper = mapper;
            _emailHelper = emailHelper;
        }
        public BaseService(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, IMapper mapper, IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, EmailHelper emailHelper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _mapper = mapper;
            _emailHelper = emailHelper;
        }
    }
}
