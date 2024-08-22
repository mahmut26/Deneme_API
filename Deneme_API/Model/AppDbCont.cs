//using DataLayer.Model_Login;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Deneme_API.Model
{
    public class AppDbCont : IdentityDbContext<AppUser>
    {
        public AppDbCont(DbContextOptions<AppDbCont> options) : base(options)
        {

        }
    }
}
