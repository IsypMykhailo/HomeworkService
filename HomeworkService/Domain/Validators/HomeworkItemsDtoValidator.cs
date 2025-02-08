using FluentValidation;
using HomeworkService.Domain.Dto;

namespace HomeworkService.Domain.Validators;

public class HomeworkItemsDtoValidator : AbstractValidator<HomeworkItemsDto>
{
    public HomeworkItemsDtoValidator()
    {
        RuleFor(e => e.Grade)
            .Must(BeMoreOrEqualZero).WithMessage("Grade must be more or equal than 0");
    }

    private bool BeMoreOrEqualZero(int? grade)
    {
        return grade >= 0;
    }
}