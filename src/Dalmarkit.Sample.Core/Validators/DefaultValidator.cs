using FluentValidation;

namespace Dalmarkit.Sample.Core.Validators;

public class DefaultValidator<T> : AbstractValidator<T>
{
    public DefaultValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;
    }
}
