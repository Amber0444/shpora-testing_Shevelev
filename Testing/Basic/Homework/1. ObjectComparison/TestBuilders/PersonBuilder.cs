namespace HomeExercise.Tasks.ObjectComparison;

public sealed class PersonBuilder
{
    private string _name = "testName";
    private int _age = 30;
    private int _height = 175;
    private int _weight = 75;
    private Person? _parent = null;

    public PersonBuilder Named(string name) { _name = name; return this; }
    public PersonBuilder Aged(int age)      { _age = age;   return this; }
    public PersonBuilder WithParent(Person? parent) { _parent = parent; return this; }
    public PersonBuilder WithHeight(int height)  { _height = height; return this; }
    public PersonBuilder WithWeight(int weight)  { _weight = weight; return this; }

    public Person Build() =>
        new Person(_name, _age, _height, _weight, _parent);
}