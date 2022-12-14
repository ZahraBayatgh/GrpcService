using FluentValidation;
using Sample.gRPC;
using Sample.gRPC.v1;

namespace GrpcService
{
    public class CountryCreateRequestValidator : AbstractValidator<CountryCreationRequest>
    {
        public CountryCreateRequestValidator()
        {
            RuleFor(request => request.Name).NotEmpty().WithMessage("Name is mandatory.");
            RuleFor(request => request.Description).MinimumLength(5).WithMessage("Description is mandatory and be longer than 5 characters");
        }
    }

}
