
namespace HomeExercise.Tasks.NumberValidator.TestBuilders;

public class NumberValidatorBuilder
{
    private bool _onlyPositive = true;
    private int _precision = 2;
    private int _scale = 2;
    
    public NumberValidatorBuilder WithOnlyPositive(bool onlyPositive) { _onlyPositive = onlyPositive; return this; }
    public NumberValidatorBuilder WithPrecision(int precision) { _precision = precision; return this; }
    public NumberValidatorBuilder WithScale(int scale) { _scale = scale; return this; }
    public NumberValidator Build()
    {
        return new NumberValidator(_precision, _scale, _onlyPositive);
    }
}