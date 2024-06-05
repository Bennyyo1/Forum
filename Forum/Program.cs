using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
namespace Forum
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("ForumContextConnection") ?? throw new InvalidOperationException("Connection string 'ForumContextConnection' not found.");

            builder.Services.AddDbContext<ForumContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<Areas.Identity.Data.ForumUser>(options => options.SignIn.RequireConfirmedAccount = false) //ingen confirm email
                .AddRoles<IdentityRole>() //för att lägga till roller(admin,user)
                .AddEntityFrameworkStores<ForumContext>();

            builder.Services.AddAuthorization(options =>
            options.AddPolicy("AdminReq", policy => policy.RequireRole("Admin"))); //Skapa authorization AdminKrav.

            
            builder.Services.AddSingleton<BadWordFilter>();// badwordfilter som singleton

            builder.Services.AddHttpClient();



			builder.Services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/RoleAdmin", "AdminReq"); // Admin requirement to access the RoleAdmin folder
                
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); //viktigt att ha i denna ordning authentication>Authorizations

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
