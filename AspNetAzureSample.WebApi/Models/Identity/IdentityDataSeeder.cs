using Microsoft.AspNetCore.Identity;

namespace AspNetAzureSample.Models.Identity
{
    public class IdentityDataSeeder
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityDataSeeder(UserManager<IdentityUser> userManager,
                                  RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await SeedUser(AdminUser, AdminRole, "P@ssword1");
            await SeedUser(AliceUser, UserRole, "P@ssword1");
            await SeedUser(BobUser, UserRole, "P@ssword1");
        }

        private async Task SeedUser(IdentityUser user, IdentityRole role, string password)
        {
            await CreateRoleAsync(role);

            await CreateUserAsync(user, password);

            var roleName = role.Name ?? throw new ArgumentNullException(nameof(role.Name));
            var superAdminInRole = await _userManager.IsInRoleAsync(user, roleName);
            if (!superAdminInRole)
                await _userManager.AddToRoleAsync(user, role.Name);
        }

        private async Task CreateRoleAsync(IdentityRole role)
        {
            var roleName = role.Name ?? throw new ArgumentNullException(nameof(role.Name));
            var exits = await _roleManager.RoleExistsAsync(roleName);
            if (!exits)
                await _roleManager.CreateAsync(role);
        }

        private async Task CreateUserAsync(IdentityUser user, string password)
        {
            var userEmail = user.Email ?? throw new ArgumentNullException(nameof(user.Email));
            var exists = await _userManager.FindByEmailAsync(userEmail);
            if (exists == null)
                await _userManager.CreateAsync(user, password);
        }

        private static readonly IdentityRole AdminRole = new()
        {
            Id = "cac43a6e-f7bb-4448-baaf-1add431ccbbf",
            Name = "SuperAdmin",
            NormalizedName = "SUPERADMIN"
        };

        private static readonly IdentityRole UserRole = new()
        {
            Id = "4413f3b1-299d-448c-ace0-cb4a22e3558e",
            Name = "User",
            NormalizedName = "USER"
        };

        private static readonly IdentityUser AdminUser = new()
        {
            Id = "b8633e2d-a33b-45e6-8329-1958b3252bbd",
            UserName = "admin@example.com",
            NormalizedUserName = "ADMIN",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true,
        };

        private static readonly IdentityUser AliceUser = new()
        {
            Id = "de1e0d65-aa11-4953-b074-1ba0b825851b",
            UserName = "alice@example.com",
            NormalizedUserName = "ALICE",
            Email = "alice@example.com",
            NormalizedEmail = "ALICE@EXAMPLE.COM",
            EmailConfirmed = true,
        };

        private static readonly IdentityUser BobUser = new()
        {
            Id = "33d069d9-a8c1-4e45-b51a-1ec42b219eb7",
            UserName = "bob@example.com",
            NormalizedUserName = "BOB",
            Email = "bob@example.com",
            NormalizedEmail = "BOB@EXAMPLE.COM",
            EmailConfirmed = true,
        };
    }
}
