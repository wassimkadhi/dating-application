using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Data;
using API.Entities;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API;

public class Seed
{

    public static async Task SeedUsers(UserManager<AppUser> userManager , RoleManager<AppRole> rolemanager)
    {

        if (await userManager.Users.AnyAsync()) return;

        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
        var roles = new List<AppRole>{
            new AppRole {Name="Member"},
            new AppRole {Name="Admin"},
            new AppRole {Name="Moderator"},
        } ;

        foreach(var role in roles) 
        {
            await rolemanager.CreateAsync(role) ;

        }

        foreach (var user in users)
        {

            user.UserName = user.UserName.ToLower();
            user.Cretaed=DateTime.SpecifyKind(user.Cretaed,DateTimeKind.Utc) ;
             user.LastActive=DateTime.SpecifyKind(user.LastActive,DateTimeKind.Utc) ;
            await userManager.CreateAsync(user, "academie");
            await userManager.AddToRoleAsync(user,"Member");


        }

        var admin= new AppUser{
            UserName="admin"
        } ;

        await userManager.CreateAsync(admin, "academie") ;
        await userManager.AddToRolesAsync(admin , new[]{"Admin","Moderator"}) ;



    }



}



