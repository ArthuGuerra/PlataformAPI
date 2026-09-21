using Application.DataTransferObject;
using Application.MapperExtension;
using AutoMapper;
using Infraestrutura.BancoContexto;
using Infraestrutura.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTest.UnitTestes
{
    public abstract class TestBase
    {             
        protected readonly IMapper _mapper;    
        private const string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=ApiContextTeste;Trusted_Connection=True;TrustServerCertificate=True;";

        protected TestBase()
        {           

            var loggerFactory = LoggerFactory.Create(builder => { });

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DomainDTOMappingProfile>();
            }, loggerFactory);

            _mapper = config.CreateMapper();
            
        }

        protected static ApiContext CreateContext(DatabaseType databaseType)
        {           

            var options = new DbContextOptionsBuilder<ApiContext>();

            if (databaseType == DatabaseType.InMemory)
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            }
            else
            {
                options.UseSqlServer(ConnectionString);
            }

            return new ApiContext(options.Options);
        }              
    }
}
