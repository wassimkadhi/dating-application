﻿using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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






}
