using Microsoft.AspNetCore.Identity;

namespace Backend.Data;

public class AuthenticationSeeder
{
    
    private RoleManager<IdentityRole> _roleManager;
    private UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    

    public AuthenticationSeeder(RoleManager<IdentityRole> roleManager, IConfiguration configuration, UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _configuration = configuration;
        _userManager = userManager;
    }
    
    public void AddRoles()
    {
        var tAdmin = CreateAdminRole(_roleManager);
        tAdmin.Wait();

        var tUser = CreateUserRole(_roleManager);
        tUser.Wait();
    }
        
    public void AddAdmin()
    {
        var tAdmin = CreateAdminIfNotExists();
        tAdmin.Wait();
    }

    private async Task CreateAdminRole(RoleManager<IdentityRole> roleManager)
    {
        await roleManager.CreateAsync(new IdentityRole(_configuration["Roles:1"])); 
    }

    async Task CreateUserRole(RoleManager<IdentityRole> roleManager)
    {
        await roleManager.CreateAsync(new IdentityRole(_configuration["Roles:2"]));
    }


    private async Task CreateAdminIfNotExists()
    {
        var adminInDb = await _userManager.FindByEmailAsync("admin@admin.com");
        if (adminInDb == null)
        {
            var admin = new IdentityUser { UserName = "admin", Email = "admin@admin.com" };
            var adminCreated = await _userManager.CreateAsync(admin, "admin123");

            if (adminCreated.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }

}