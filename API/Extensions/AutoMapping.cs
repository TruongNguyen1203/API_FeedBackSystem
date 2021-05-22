using Core.Entities;
using AutoMapper;
using API.Dtos;

namespace API.Extensions
{
    public class AutoMapping:Profile
    {
        public AutoMapping()
        {
            CreateMap<Question,QuestionDto>();
        }
        
    }
}