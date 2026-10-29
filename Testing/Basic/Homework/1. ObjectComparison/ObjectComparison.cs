using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace HomeExercise.Tasks.ObjectComparison;
public class ObjectComparison
{   
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar()
    {
        // Arrange
        var expectedTsar = new PersonBuilder()
            .Named("Ivan IV The Terrible")
            .Aged(54)
            .WithHeight(170)
            .WithWeight(70)
            .WithParent(new PersonBuilder()
                .Aged(28)
                .Named("Vasili III of Russia")
                .WithHeight(170)
                .WithWeight(60)
                .WithParent(null)
                .Build())
            .Build();
        
        // Act
        var actualTsar = TsarRegistry.GetCurrentTsar();
        
        // Assert
        actualTsar.Should().BeEquivalentTo(expectedTsar, options => 
           options.Excluding((IMemberInfo member) => 
               member.Name == "Id" && 
               member.DeclaringType == typeof(Person)));
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Какие недостатки у такого подхода? 
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
    
    /*  1. Неочевидная что сломалось, при падении тест вернет Expected: True But was:  False.
     *  2. Не соблюдается AAA. Act и Arrange перепутаны местами, между ними нет отступа.
     *  3. Нет переиспользования кода.
     *  4. Тест неудобно расширять придется менять сразу несколько мест.
     *  5. Рекурсия может переполниться. 
     */
}
