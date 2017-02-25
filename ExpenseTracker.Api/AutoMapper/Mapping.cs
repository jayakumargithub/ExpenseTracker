using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Castle.Windsor;

namespace ExpenseTracker.Api.AutoMapper
{
    public static class Mapping
    {
        private static MapperConfiguration _config;
        private static IMapper _mapper;
        
        public static IMapper Configuration()
        {
            _config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Repository.Entities.ExpenseGroup, Dto.ExpenseGroup>(); 
                cfg.CreateMap<Dto.ExpenseGroup, Repository.Entities.ExpenseGroup>(); 
                cfg.CreateMap<Repository.Entities.Expense, Dto.Expense>();
                cfg.CreateMap<Dto.Expense, Repository.Entities.Expense>();
              

            });
             
                
            
            _mapper = _config.CreateMapper();
            return _mapper;

        } 

       
     
    }

 }

     
 