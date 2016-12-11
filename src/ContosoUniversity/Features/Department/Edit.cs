﻿namespace ContosoUniversity.Features.Department
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using DAL;
    using FluentValidation;
    using Infrastructure.Mapping;
    using MediatR;
    using Models;

    public class Edit
    {
        public class Query : IAsyncRequest<Command>
        {
            public int Id { get; set; }
        }

        public class Command : IAsyncRequest
        {
            public string Name { get; set; }

            public decimal? Budget { get; set; }

            public DateTime? StartDate { get; set; }

            public Instructor Administrator { get; set; }
            public int DepartmentID { get; set; }
            public byte[] RowVersion { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(m => m.Name).NotNull().Length(3, 50);
                RuleFor(m => m.Budget).NotNull();
                RuleFor(m => m.StartDate).NotNull();
                RuleFor(m => m.Administrator).NotNull();
            }
        }

        public class QueryHandler : IAsyncRequestHandler<Query, Command>
        {
            private readonly SchoolContext _db;

            public QueryHandler(SchoolContext db)
            {
                _db = db;
            }

            public async Task<Command> Handle(Query message)
            {
                var department = await _db.Departments
                    .Where(d => d.DepartmentID == message.Id)
                    .ProjectToSingleOrDefaultAsync<Command>();

                return department;
            }
        }

        public class CommandHandler : AsyncRequestHandler<Command>
        {
            private readonly SchoolContext _db;

            public CommandHandler(SchoolContext db)
            {
                _db = db;
            }

            protected override async Task HandleCore(Command message)
            {
                var dept = await _db.Departments.FindAsync(message.DepartmentID);

                Mapper.Map(message, dept);
            }
        }
    }

}