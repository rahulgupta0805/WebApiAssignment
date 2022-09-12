using Ardalis.GuardClauses;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Assignment.Service.Store;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static Assignment.Service.Controllers.v1.Queries.GetUserDetailsQuery.Handler;

namespace Assignment.Service.Controllers.v1.Queries
{
    public class GetUserDetailsQuery : IRequest<Result<UserDetailsOutputDto>>
    {
        public GetUserDetailsQuery(int id)
        {
            Id = id;
        }

        [Required]
        public int Id { get; }

        public class RequestValidator : AbstractValidator<GetUserDetailsQuery>
        {
            public static readonly RequestValidator Instance = new ();

            public RequestValidator()
            {
                RuleFor(x => x.Id).GreaterThan(0);
            }
        }

        public class Handler : IRequestHandler<GetUserDetailsQuery, Result<UserDetailsOutputDto>>
        {
            private readonly UserDbContext _dbContext;

            public Handler(UserDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Result<UserDetailsOutputDto>> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
            {
                Guard.Against.Null(request, nameof(request));

                try
                {
                    var dataValidation = RequestValidator.Instance.Validate(request);
                    if (!dataValidation.IsValid)
                    {
                        return Result<UserDetailsOutputDto>.Invalid(dataValidation.AsErrors());
                    }

                    var user = await _dbContext.User
                        .Where(u => u.Id == request.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cancellationToken);
                    
                    if(user is null)
                    {
                        return Result<UserDetailsOutputDto>.NotFound();
                    }

                    return new UserDetailsOutputDto(user.Id, user.FirstName, user.LastName, user.EmailId, user.CreatedDate, user.UpdatedDate);
                }
                catch
                {
                    throw;
                }
            }

            public record UserDetailsOutputDto(int id, string firstName, string lastName, string emailId, DateTime createdAt, DateTime updateAt);
        }
    }
}