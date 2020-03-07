using AdvertApiModels;
using AutoMapper;

namespace AdvertApi.Models
{
    public class AdvertProfile : Profile
    {
        public AdvertProfile()
        {
            CreateMap<AdvertModel, AdvertDbModel>();
        }
    }
}
