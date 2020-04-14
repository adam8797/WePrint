using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace WePrint.Models.Search
{
    public class SearchProfile : Profile
    {
        public SearchProfile()
        {
            CreateMap<Project, SearchViewModel>().ConvertUsing(project =>
            new SearchViewModel()
                {
                    Description = project.Description,
                    ImageUrl = project.Thumbnail,
                    Id = project.Id,
                    Title = project.Title,
                    Type = "Project"
                }
            );
        }
    }
}
