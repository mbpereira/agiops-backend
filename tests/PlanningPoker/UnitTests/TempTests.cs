using FluentAssertions;

namespace UnitTests;

public class TempTests
{
    [Test]
    public void Sum_TwoOneMoreOne_ShouldBeTwo()
    {
        (1 + 1).Should().Be(3);
    }
}