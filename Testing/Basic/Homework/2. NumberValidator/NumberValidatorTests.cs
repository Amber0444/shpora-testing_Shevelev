
using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [Test]
    public void NumberValidator_WithZeroPrecision_ThrowsArgumentException()
    {
        Action action = () => A.NumberValidator
            .WithPrecision(0)
            .WithScale(2)
            .Build();

        action.Should().Throw<ArgumentException>();
    }
    [Test]
    public void NumberValidator_WithNegativePrecision_ThrowsArgumentException()
    {
        Action action = () => A.NumberValidator
            .WithPrecision(-1)
            .WithScale(2)
            .Build();

        action.Should().Throw<ArgumentException>();
    }
    
    [Test]
    public void NumberValidator_WithZeroScale_DoesNotThrow()
    {
        Action action = () => A.NumberValidator
            .WithPrecision(2)
            .WithScale(0)
            .Build();

        action.Should().NotThrow();
    }
    
    [Test]
    public void NumberValidator_WithNegativeScale_ThrowsArgumentException()
    {
        Action action = () => A.NumberValidator
            .WithPrecision(2)
            .WithScale(-1)
            .Build();

        action.Should().Throw<ArgumentException>();
    }
    
    [Test]
    public void IsValidNumber_WithNull_ReturnsFalse()
    {
        var numberValidator = A.NumberValidator
            .WithPrecision(17)
            .WithScale(2)
            .Build();

        numberValidator.IsValidNumber(null).Should().BeFalse();
    }
    
    [Test]
    public void IsValidNumber_WithDecimalPoint_ShouldBeTrue()
    {   
        var numberValidator = A.NumberValidator
            .WithPrecision(17)
            .WithScale(2)
            .Build();
        
        numberValidator.IsValidNumber("0.0").Should().BeTrue();
    }

    [Test]
    public void IsValidNumber_WithInteger_ShouldBeTrue()
    {
        var numberValidator = A.NumberValidator
            .WithPrecision(17)
            .WithScale(2)
            .Build();
        
        numberValidator.IsValidNumber("0").Should().BeTrue();
    }
    
    [Test]
    public void IsValidNumber_WithMoreDigitalThanPrecision_ShouldBeFalse()
    {
        var numberValidator = A.NumberValidator
            .WithPrecision(3)
            .WithScale(2)
            .Build();
        
        numberValidator.IsValidNumber("00.00").Should().BeFalse();
    }
    
    [TestCase(3, 2, "-0.00")]
    [TestCase(3, 2, "+0.00")]
    public void IsValidNumber_WithSignedZeroExceedingPrecision_ReturnsFalse(int precision, int scale, string number)
    {   
        var numberValidator = A.NumberValidator
            .WithPrecision(precision)
            .WithScale(scale)
            .Build();
        
        numberValidator.IsValidNumber(number).Should().BeFalse();
    }
    
    [TestCase(4, 2, "-1.23", false)]
    [TestCase(4, 2, "+1.23", true)]
    public void IsValidNumber_OnlyPositive_RejectsNegative(int precision, int scale, string number, bool expected)
    {
        var numberValidator = A.NumberValidator
            .WithPrecision(precision)
            .WithScale(scale)
            .EnableOnlyPositive(true)
            .Build();
        
        numberValidator.IsValidNumber(number).Should().Be(expected);
    }
    
    [TestCase(3, 2, "+1.23", false)]
    [TestCase(4, 2, "+1.23", true)]
    [TestCase(4, 2, "-1.23", true)]
    [TestCase(3, 2, "-1.23", false)]
    public void IsValidNumber_CountsSignTowardPrecision(int precision, int scale, string number, bool expected)
    {
        var validator = A.NumberValidator
            .WithPrecision(precision)
            .WithScale(scale)
            .EnableOnlyPositive(false)
            .Build();

        validator.IsValidNumber(number).Should().Be(expected);
    }
    
    [Test]
    public void IsValidNumber_WithMoreFractionalDigitsThanScale_ReturnsFalse()
    {
        var numberValidator = A.NumberValidator
            .WithPrecision(17)
            .WithScale(2)
            .Build();

        numberValidator.IsValidNumber("0.000").Should().BeFalse();
    }
    
    [Test]
    public void IsValidNumber_WithNonNumericCharacters_ReturnsFalse()
    {
        var numberValidator = A.NumberValidator
            .WithPrecision(17)
            .WithScale(2)
            .Build();

        numberValidator.IsValidNumber("a.sd").Should().BeFalse();
    }
}