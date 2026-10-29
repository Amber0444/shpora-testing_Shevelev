
using NUnit.Framework;
using FluentAssertions;
using HomeExercise.Tasks.NumberValidator.TestBuilders;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [Test]
    public void NumberValidator_WithZeroPrecision_ThrowsArgumentException()
    {
        // Arrange
        Action action = () => new NumberValidatorBuilder()
            .WithPrecision(0)
            .WithScale(0)
            .Build();
        
        // Act + Assert
        action.Should().Throw<ArgumentException>().WithMessage("precision*");
    }
    
    [Test]
    public void NumberValidator_WithNegativePrecision_ThrowsArgumentException()
    {
        // Arrange
        Action action = () => new NumberValidatorBuilder()
            .WithPrecision(-1)
            .WithScale(2)
            .Build();
        
        // Act + Assert
        action.Should().Throw<ArgumentException>().WithMessage("precision*");
    }
    
    [Test]
    public void NumberValidator_WithZeroScale_DoesNotThrow()
    {
        // Arrange
        Action action = () => new NumberValidatorBuilder()
            .WithPrecision(2)
            .WithScale(0)
            .Build();
        
        // Act + Assert
        action.Should().NotThrow();
    }
    
    [Test]
    public void NumberValidator_WithNegativeScale_ThrowsArgumentException()
    {
        // Arrange
        Action action = () => new NumberValidatorBuilder()
            .WithPrecision(2)
            .WithScale(-1)
            .Build();
        
        // Act + Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Test]
    public void IsValidNumber_WithNull_ReturnsFalse()
    {
        // Arrange
        var numberValidator = new NumberValidatorBuilder()
            .WithPrecision(17)
            .WithScale(2)
            .Build();
        
        // Act + Assert
        numberValidator.IsValidNumber(null).Should().BeFalse();
    }
    
    [Test]
    public void IsValidNumber_WithDecimalPoint_ShouldBeTrue()
    {   
        // Arrange
        var numberValidator = new NumberValidatorBuilder()
            .WithPrecision(17)
            .WithScale(2)
            .Build();
        
        // Act + Assert
        numberValidator.IsValidNumber("0.0").Should().BeTrue();
    }

    [Test]
    public void IsValidNumber_WithInteger_ShouldBeTrue()
    {
        // Arrange
        var numberValidator = new NumberValidatorBuilder()
            .WithPrecision(17)
            .WithScale(2)
            .Build();
        
        // Act + Assert
        numberValidator.IsValidNumber("0").Should().BeTrue();
    }
    
    [Test]
    public void IsValidNumber_WithMoreDigitsThanPrecision_ShouldBeFalse()
    {
        // Arrange
        var numberValidator = new NumberValidatorBuilder()
            .WithPrecision(3)
            .WithScale(2)
            .Build();
        
        // Act + Assert
        numberValidator.IsValidNumber("00.00").Should().BeFalse();
    }
    
    [TestCase(3, 2, "-0.00")]
    [TestCase(3, 2, "+0.00")]
    public void IsValidNumber_WithSignedZeroExceedingPrecision_ReturnsFalse(int precision, int scale, string number)
    {   
        // Arrange
        var numberValidator = new NumberValidatorBuilder()
            .WithPrecision(precision)
            .WithScale(scale)
            .Build();
        
        // Act + Assert
        numberValidator.IsValidNumber(number).Should().BeFalse();
    }
    
    [TestCase(4, 2, "-1.23", false)]
    [TestCase(4, 2, "+1.23", true)]
    public void IsValidNumber_OnlyPositive_RejectsNegative(int precision, int scale, string number, bool expected)
    {
        // Arrange
        var numberValidator = new NumberValidatorBuilder()
            .WithPrecision(precision)
            .WithScale(scale)
            .EnableOnlyPositive(true)
            .Build();
        
        // Act + Assert
        numberValidator.IsValidNumber(number).Should().Be(expected);
    }
    
    [TestCase(3, 2, "+1.23", false)]
    [TestCase(4, 2, "+1.23", true)]
    [TestCase(4, 2, "-1.23", true)]
    [TestCase(3, 2, "-1.23", false)]
    public void IsValidNumber_CountsSignTowardPrecision(int precision, int scale, string number, bool expected)
    {
        // Arrange
        var validator = new NumberValidatorBuilder()
            .WithPrecision(precision)
            .WithScale(scale)
            .EnableOnlyPositive(false)
            .Build();
        
        // Act + Assert
        validator.IsValidNumber(number).Should().Be(expected);
    }
    
    [Test]
    public void IsValidNumber_WithMoreFractionalDigitsThanScale_ReturnsFalse()
    {
        // Arrange
        var numberValidator = new NumberValidatorBuilder()
            .WithPrecision(17)
            .WithScale(2)
            .Build();
        
        // Act + Assert
        numberValidator.IsValidNumber("0.000").Should().BeFalse();
    }
    
    [Test]
    public void IsValidNumber_WithNonNumericCharacters_ReturnsFalse()
    {
        // Arrange
        var numberValidator = new NumberValidatorBuilder()
            .WithPrecision(17)
            .WithScale(2)
            .Build();
        
        // Act + Assert
        numberValidator.IsValidNumber("a.sd").Should().BeFalse();
    }
}