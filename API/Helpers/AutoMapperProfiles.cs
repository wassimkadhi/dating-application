using API.DtoS;
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

         .ForMember(dest=>dest.Age , opt =>opt.MapFrom(src=>src.DateOfBirth.CalculateAge())) ;
         CreateMap<Photo,PhotoDto>() ;

         CreateMap<MemberUpdateDto , AppUser>() ;
         CreateMap<RegisterDto,AppUser>() ;
         //mapping for Message : message to message dto
         CreateMap<Message,MessageDto>()
         .ForMember(d=>d.SenderPhotoUrl , o =>o.MapFrom(s=>s.Sender.Photos.FirstOrDefault(x=>x.ISMain).Url))
         .ForMember(d=>d.RecepientPhotoUrl , o =>o.MapFrom(s=>s.Recipient.Photos.FirstOrDefault(x=>x.ISMain).Url)) ;
         CreateMap<DateTime, DateTime>().ConvertUsing(d=>DateTime.SpecifyKind(d,DateTimeKind.Utc));
         CreateMap<DateTime?, DateTime?>().ConvertUsing(d=>d.HasValue ? DateTime.SpecifyKind(d.Value,DateTimeKind.Utc):null);
     }
}
