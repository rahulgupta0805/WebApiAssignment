using Ardalis.GuardClauses;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Assignment.Service.Store.Models;
using Assignment.Service.Store;
using FluentValidation;
using MediatR;
using System.ComponentModel.DataAnnotations;
using static Assignment.Service.Controllers.v1.Commands.CreateNewUserCommand.Handler;
using Microsoft.EntityFrameworkCore;
using static Assignment.Service.Controllers.v1.Queries.GetUserDetailsQuery.Handler;

namespace Assignment.Service.Controllers.v1.Commands
{
    public class UpdateUserCommand : IRequest<Result<Unit>>
    {
        public UpdateUserCommand(int id, UserInputDto userData)
        {
            Id = id;
            this.userData = userData;
        }

        [Required]
        public int Id { get; }
        [Required]
        public UserInputDto userData { get; }

        public record UserInputDto(string? firstName, string? lastName, string? emailId);

        public class RequestValidator : AbstractValidator<UpdateUserCommand>
        {
            public static RequestValidator Instance = new();

            public RequestValidator()
            {
                RuleFor(x => x.Id).GreaterThan(0);
                RuleFor(x => x.userData).NotEmpty();
                RuleFor(x => x.userData.firstName).NotEmpty().MaximumLength(50).When(x => !string.IsNullOrEmpty(x.userData.firstName));
                RuleFor(x => x.userData.lastName).NotEmpty().MaximumLength(50).When(x => !string.IsNullOrEmpty(x.userData.lastName));
                RuleFor(x => x.userData.emailId).EmailAddress().When(x => !string.IsNullOrEmpty(x.userData.emailId)).WithMessage("Email address is not valid");
            }
        }

        public class Handler : IRequestHandler<UpdateUserCommand, Result<Unit>>
        {
            private readonly UserDbContext _dbContext;

            public Handler(UserDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Result<Unit>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                Guard.Against.Null(request, nameof(request));

                try
                {
                    var dataValidation = RequestValidator.Instance.Validate(request);
                    if (!dataValidation.IsValid)
                    {
                        return Result<Unit>.Invalid(dataValidation.AsErrors());
                    }

                    var user = await _dbContext.User
                        .Where(u => u.Id == request.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cancellationToken);

                    if (user is null)
                    {
                        return Result<Unit>.NotFound();
                    }

                    user.UpdatedDate = DateTime.UtcNow;
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    return Unit.Value;
                }
                catch
                {
                    throw;
                }
            }

            public record CreateNewUserOutputDto(int id);
        }
    }

}