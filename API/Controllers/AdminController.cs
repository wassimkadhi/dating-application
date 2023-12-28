using System.Reflection.Metadata.Ecma335;
using API.Controllers;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;

public class AdminController : BaseApiController
{

private readonly UserManager<AppUser> _usermanager ; 
    public AdminController( UserManager<AppUser> usermanager)
    {

        _usermanager=usermanager ; 
    }


    [Authorize(Policy ="RequireAdminRole")]
    [HttpGet("users-with-roles")]

    public  async Task<ActionResult> GetUsersWithRoles(){

       var users=await _usermanager.Users.
       OrderBy(u=>u.UserName).
       Select(u=>new
       {
        u.Id,
        username=u.UserName,
        roles=u.UserRoles.Select(r=>r.Role.Name).ToList()

       })
       .ToListAsync();

       return  Ok(users) ;
       
    }

[Authorize(Policy ="RequireAdminRole")]
[HttpPut("edit-roles/{username}")]

public async Task<ActionResult>EditUserRole(string username, [FromQuery]string roles){

  if(string.IsNullOrEmpty(roles)) return BadRequest("you must selest at least one role");
  var selectedRoles=roles.Split(",").ToArray() ;

    var user=await _usermanager.FindByNameAsync(username) ;
    if(user==null) return NotFound() ; 

    var userRoles= await _usermanager.GetRolesAsync(user) ;
    var result = await _usermanager.AddToRolesAsync(user,selectedRoles.Except(userRoles));

    if(!result.Succeeded) return BadRequest("failed to add roles") ; 

    result=await _usermanager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles)) ; 
    if(!result.Succeeded) return BadRequest("failed to remove roles roles") ;
    return Ok(await _usermanager.GetRolesAsync(user)) ;


}



[Authorize(Policy ="ModeratorRole")]
[HttpGet("photos-to-moderate")]
    public  ActionResult GETPhotosForModeration(){

        return  Ok("photo to moderate");
    }

}
    


   

