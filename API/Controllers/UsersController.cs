﻿using System.Security.Claims;
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

    private readonly IPhotoService _photoService;


    public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _photoService = photoService;
    }

    [HttpGet] //api/users
    public async Task<ActionResult<IEnumerable<MemeberDto>>> GetUsers()
    {
        var users = await _userRepository.GetUsersAsync();
        var usersToReturn = _mapper.Map<IEnumerable<MemeberDto>>(users);
        return Ok(usersToReturn);

    }


    [HttpGet("{username}")]  //api/users/name

    public async Task<ActionResult<MemeberDto>> GetUserByUserName(string username)
    {
        var user = await _userRepository.GetUserByUserNameAsync(username);
        return _mapper.Map<MemeberDto>(user);


    }

    /*
     [HttpGet("{id}")]  //api/users/id

        public async Task<ActionResult<MemeberDto>> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id) ; 
            return _mapper.Map<MemeberDto>(user) ; 


        }
        */

    [HttpPut("EditMember/")]
    public async Task<ActionResult> UapdateMember(MemberUpdateDto memberupdate)
    {


        var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

        if (user == null) return NotFound();
        _mapper.Map(memberupdate, user);
        if (await _userRepository.SaveAllAsync()) return NoContent();
        return BadRequest("update fail");


    }



    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>>AddPhoto(IFormFile file) 
    {
        var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
        if(user ==null ) return NotFound() ;
        var result =await _photoService.AddPhotoAsync(file) ; 
        if(result.Error != null) return BadRequest(result.Error.Message) ; 
         var photo = new Photo {
            Url =result.SecureUrl.AbsoluteUri ,
            PublicId=result.PublicId

         } ; 

         if(user.Photos.Count==0) photo.ISMain=true ; 
         user.Photos.Add(photo) ; 
         if( await _userRepository.SaveAllAsync()) return _mapper.Map<PhotoDto>(photo) ; 

         return BadRequest("problem adding photo") ; 



    }



    [HttpPut("edit-main-photo/{photoId}")] 
    public async Task<ActionResult> SetMainPhoto(int photoId) {
        var user =await _userRepository.GetUserByUserNameAsync(User.GetUsername()) ; 
        if(user ==null) return NotFound() ;  
        var photo =user.Photos.FirstOrDefault(x=>x.Id==photoId) ; 
        if(photo==null) return NotFound() ; 
        if(photo.ISMain) return BadRequest("this photo is allready main photo ") ; 
        var curentMain=  user.Photos.FirstOrDefault(x=> x.ISMain)  ; 
        if(curentMain != null)  curentMain.ISMain=false ; 
        photo.ISMain=true ; 
        if(await _userRepository.SaveAllAsync()) return NoContent() ; 
        return BadRequest("something went wrong ") ;
    }


    [HttpDelete("delete-photo/{photoId}")]

    public async Task<ActionResult>DelePhotoById(int photoId){
        var user =await _userRepository.GetUserByUserNameAsync(User.GetUsername()) ;
         if(user ==null) return NotFound() ; 
         var photo =user.Photos.FirstOrDefault(x=>x.Id==photoId) ;  
           if(photo ==null) return NotFound() ;
           if(photo.ISMain){
            user.Photos.Remove(photo) ; 
           var newmainphoto = user.Photos.FirstOrDefault(x=> x.ISMain==false) ; 
           newmainphoto.ISMain=true ;
           } 
           user.Photos.Remove(photo) ; 
            if(await _userRepository.SaveAllAsync()) return Ok() ;
            return BadRequest("something went wrong ") ;


    }







}

