using System.Security.Claims;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;


namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;


    public UsersController(IUserRepository userRepository ,IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;   
    }

    [HttpGet] //api/users
    public async Task<ActionResult<IEnumerable<MemeberDto>>> GetUsers()
    {
        var users =await _userRepository.GetUsersAsync();
        var usersToReturn =_mapper.Map<IEnumerable<MemeberDto>>(users) ; 
        return Ok(usersToReturn) ; 
        
    }


    [HttpGet("{username}")]  //api/users/name

    public async Task<ActionResult<MemeberDto>> GetUserByUserName(string username)
    {
        var user = await _userRepository.GetUserByUserNameAsync(username) ; 
        return _mapper.Map<MemeberDto>(user) ; 

       
    }

/*
 [HttpGet("{id}")]  //api/users/id

    public async Task<ActionResult<MemeberDto>> GetUserById(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id) ; 
        return _mapper.Map<MemeberDto>(user) ; 

       
    }
    */
    
    [HttpPut ("EditMember/")]
    public async Task<ActionResult> UapdateMember(MemberUpdateDto memberupdate) {

        var username=User.FindFirst(ClaimTypes.NameIdentifier)?.Value ; 
        var user = await _userRepository.GetUserByUserNameAsync(username) ; 

        if(user==null) return NotFound() ; 
        _mapper.Map(memberupdate , user)  ; 
        if(await _userRepository.SaveAllAsync()) return NoContent() ; 
        return BadRequest("update fail") ; 

    
    }







}
