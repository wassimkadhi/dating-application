using API.Entities;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles  :Profile
{
     public AutoMapperProfiles()
     {
         CreateMap<AppUser , MemeberDto>()
         .ForMember(dest => dest.UrlPhoto,
         opt => opt.MapFrom(src =>src.Photos.FirstOrDefault(x=>x.ISMain).Url))
          ; 
         CreateMap<Photo,PhotoDto>() ;

         CreateMap<MemberUpdateDto , AppUser>() ;
     }
}
