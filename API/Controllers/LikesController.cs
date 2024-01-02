using API.Controllers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class LikesController : BaseApiController

{

    private readonly IUnitOfWorck _uow ; 
   
    public LikesController(IUnitOfWorck uow)
    {
        _uow=uow ;

    }

    [HttpPost("{username}")] 
    public async Task<ActionResult> AddLike(string username) {

        var sourceUserId = User.GetUserId() ; 
        var likedUser = await _uow.UserRepository.GetUserByUserNameAsync(username) ; 
        var sourceUser=await _uow.LikesRepository.GetUserWithLikes(sourceUserId) ; 

        if(likedUser ==null) return NotFound() ; 
        if(sourceUser.UserName == username) return BadRequest("you cannot like your self ") ; 
        var userLike=await _uow.LikesRepository.GetUserLike(sourceUserId,likedUser.Id) ;
        if(userLike != null) return BadRequest("you already like this user" ) ; 
        userLike=new UserLike 
        {
            SourceUserId= sourceUserId ,
            TargetUserId=likedUser.Id 
        } ;


sourceUser.LikedUsers.Add(userLike) ; 
if(await _uow.Complete()) return Ok() ; 
return BadRequest("failed to like User") ;

    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate) 
    {
         if(predicate== null) return BadRequest("you need to choose wich you want to search dor  : members you like are members who liked you " ) ;
        var users=await _uow.LikesRepository.GetUserLikes(predicate , User.GetUserId()) ; 
        return Ok(users) ; 
    }






}
