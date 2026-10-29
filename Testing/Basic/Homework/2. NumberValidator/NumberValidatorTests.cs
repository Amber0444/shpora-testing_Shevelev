
using NUnit.Framework;
using FluentAssertions;
using HomeExercise.Tasks.NumberValidator.TestBuilders;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    // 1. Проверка параметров при создании объекта
    [TestCase(0,  0,  true,  "precision*", Description = "Zero precision throws")]
    [TestCase(-1, 2,  true,  "precision*", Description = "Negative precision throws")]
    [TestCase(2,  0,  false, null,         Description = "Zero scale is allowed")]
    [TestCase(2, -1,  true,  null,         Description = "Negative scale throws")]
    [TestCase(3,  3,  false, null,         Description = "scale == precision is allowed")]
    [TestCase(2,  3,  true,  null,         Description = "scale > precision throws")]
    public void NumberValidator_BuilderValidatesParameters(
        int precision, int scale, bool shouldThrow, string? expectedMessage)
    {
        // Arrange
        Action action = () => new NumberValidatorBuilder()
            .WithPrecision(precision)
            .WithScale(scale)
            .Build();

        // Act + Assert
        if (!shouldThrow)
        {
            action.Should().NotThrow();
            return;
        }

        var assertion = action.Should().Throw<ArgumentException>();
        if (expectedMessage is not null)
        {
            assertion.WithMessage(expectedMessage);
        }
    }
    
    [TestCase(0,  0,  "precision")]
    [TestCase(-1, 2,  "precision")]
    [TestCase(2, -1,  null)]
    public void NumberValidator_WithInvalidParameters_Throws(int precision, int scale, string? messagePrefix)
    {
        Action action = () => new NumberValidatorBuilder()
            .WithPrecision(precision)
            .WithScale(scale)
            .Build();

        var ex = action.Should().Throw<ArgumentException>().Which;
        if (messagePrefix is not null)
            ex.Message.Should().StartWith(messagePrefix);
    }

    [TestCase(2, 0)]
    [TestCase(17, 2)]
    public void NumberValidator_WithValidParameters_DoesNotThrow(int precision, int scale)
    {
        Action build = () => new NumberValidatorBuilder()
            .WithPrecision(precision)
            .WithScale(scale)
            .Build();

        build.Should().NotThrow();
    }
    
    // 2. Проверка значения
public record ValidatorCase(
        int Precision,
        int Scale,
        bool OnlyPositive,
        string? Number,
        bool Expected,
        string Description)
    {
        public override string ToString() => Description;
    }

    private static readonly ValidatorCase[] Cases =
    {
        new(17, 2, false, null,     false, "null returns false"),
        new(17, 2, false, "0.0",    true,  "decimal point is valid"),
        new(17, 2, false, "0",      true,  "integer is valid"),
        new(17, 2, false, "",       false, "empty string returns false"),
        new(4,  2, false, "1,23",   true,  "comma as decimal separator is valid"),
        new(4,  2, false, "1.23",   true,  "dot as decimal separator is valid"),
        new(17, 2, false, "1.",     false, "trailing dot is invalid"),
        new(17, 2, false, ".1",     false, "leading dot is invalid"),
        new(17, 2, false, "+",      false, "lone plus sign is invalid"),
        new(17, 2, false, "-",      false, "lone minus sign is invalid"),
        new(17, 2, false, "1.2.3",  false, "multiple dots are invalid"),
        new(17, 2, false, " 1",     false, "leading space is invalid"),
        new(3,  2, false, "00.00",  false, "more digits than precision"),
        new(17, 2, false, "0.000",  false, "more fractional digits than scale"),
        new(3,  2, false, "-0.00",  false, "signed zero exceeds precision (minus)"),
        new(3,  2, false, "+0.00",  false, "signed zero exceeds precision (plus)"),
        new(3,  2, false, "+1.23",  false, "sign counts toward precision (fits? no)"),
        new(4,  2, false, "+1.23",  true,  "sign counts toward precision (plus fits)"),
        new(4,  2, false, "-1.23",  true,  "sign counts toward precision (minus fits)"),
        new(3,  2, false, "-1.23",  false, "sign counts toward precision (minus overflows)"),
        new(4,  2, true,  "-1.23",  false, "only positive rejects negative"),
        new(4,  2, true,  "+1.23",  true,  "only positive accepts positive"),
        new(17, 2, false, "a.sd",   false, "non-numeric characters rejected"),
    };

    [TestCaseSource(nameof(Cases))]
    public void IsValidNumber_MatchesExpected(ValidatorCase c)
    {
        // Arrange
        var validator = new NumberValidatorBuilder()
            .WithPrecision(c.Precision)
            .WithScale(c.Scale)
            .WithOnlyPositive(c.OnlyPositive)
            .Build();

        // Act + Assert
        validator.IsValidNumber(c.Number).Should().Be(c.Expected, c.Description);
    }
}
