using Ardalis.GuardClauses;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Assignment.Service.Store;
using Assignment.Service.Store.Models;
using FluentValidation;
using MediatR;
using System.ComponentModel.DataAnnotations;
using static Assignment.Service.Controllers.v1.Commands.CreateNewUserCommand.Handler;

namespace Assignment.Service.Controllers.v1.Commands
{
    public class CreateNewUserCommand : IRequest<Result<CreateNewUserOutputDto>>
    {
        public CreateNewUserCommand(string firstName, string lastName, string emailId)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailId = emailId;
        }

        [Required]
        public string FirstName { get; }

        [Required]
        public string LastName { get; }

        [Required]
        public string EmailId { get; }

        public class RequestValidator : AbstractValidator<CreateNewUserCommand>
        {
            public static RequestValidator Instance = new ();

            public RequestValidator()
            {
                RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50).WithMessage("FirstName is required");
                RuleFor(x => x.LastName).NotEmpty().MaximumLength(50).WithMessage("LastName is required");
                RuleFor(x => x.EmailId).NotEmpty().EmailAddress().WithMessage("Email address is not valid");
            }
        }
        public class Handler : IRequestHandler<CreateNewUserCommand, Result<CreateNewUserOutputDto>>
        {
            private readonly UserDbContext _dbContext;

            public Handler(UserDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Result<CreateNewUserOutputDto>> Handle(CreateNewUserCommand request, CancellationToken cancellationToken)
            {
                Guard.Against.Null(request, nameof(request));

                try
                {
                    var dataValidation = RequestValidator.Instance.Validate(request);
                    if (!dataValidation.IsValid)
                    {
                        return Result<CreateNewUserOutputDto>.Invalid(dataValidation.AsErrors());
                    }

                    var user = new User(default, request.FirstName, request.LastName,
                                            request.EmailId, DateTime.UtcNow, DateTime.UtcNow);
                    _dbContext.Add(user);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    return new CreateNewUserOutputDto(user.Id);
                }
                catch
                {
                    throw ;
                }
            }

            public record CreateNewUserOutputDto(int id);
        }
    }
}