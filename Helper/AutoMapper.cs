

using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helper
{ 
    public class AutoMapper:Profile
    {
        public AutoMapper()
        {
            //automapper map from one object to another
            CreateMap<AppUser, MemberDto>().
                ForMember(dest=>dest.PhotoUrl,opt=>opt.
                MapFrom(src=>src.Photos.FirstOrDefault(x=>x.IsMain).Url))
                .ForMember(dest=>dest.Age,opt=>opt.MapFrom(src=>src.DateOfBirth.CalculateAge()));;
                
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();

            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl
                , opt => opt.MapFrom(src =>
                   src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
                 .ForMember(dest => dest.RecipientPhotoUrl
                , opt => opt.MapFrom(src =>
                   src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
            

          
               
        }

    }
}
