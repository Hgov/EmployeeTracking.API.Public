using EmployeeTracking.Core.EmailService;
using EmployeeTracking.Core.Entities;
using EmployeeTracking.Core.Helpers;
using EmployeeTracking.Infrastructure.IdentitySettings.Validators;
using EmployeeTracking.Infrastructure.MapperService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using NLog;
using Swashbuckle.AspNetCore.SwaggerUI;
using static EmployeeTracking.Core.Entities.Enums;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// Add services to the container.
builder.Services.AddDbContext<EmployeeDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));



builder.Services.AddIdentity<User, Role>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;

    options.SignIn.RequireConfirmedEmail = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    options.Lockout.MaxFailedAccessAttempts = 5;
}).AddEntityFrameworkStores<EmployeeDbContext>().AddDefaultTokenProviders().AddUserValidator<UserValidator>();

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
builder.Services.AddScoped<EmailHelper>();
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/Configurations/nlog.config"));
builder.Services.AddAutoMapper(typeof(Profiles));
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/User/Login");
    options.LogoutPath = new PathString("/User/Logout");
    options.AccessDeniedPath = new PathString("/Home/AccessDenied");

    options.Cookie = new()
    {
        Name = "IdentityCookie",
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        SecurePolicy = CookieSecurePolicy.Always
    };
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HrDepartmentPolicy", policy =>
    {
        policy.RequireClaim("Department", Enum.GetName(Department.HR)!);
    });

    options.AddPolicy("SoftwareDepartmentPolicy", policy =>
    {
        policy.RequireClaim("Department", Enum.GetName(Department.Software)!);
    });

    options.AddPolicy("EmployeePolicy", policy =>
    {
        policy.RequireClaim("Department", Enum.GetNames<Department>());
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.DefaultModelRendering(ModelRendering.Example);
    options.DocExpansion(DocExpansion.List);
    options.DefaultModelsExpandDepth(0);
    options.DisplayRequestDuration();
    options.EnableValidator();
    options.RoutePrefix = "api/swagger";
});
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{

    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();
