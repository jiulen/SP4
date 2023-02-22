using NUnit.Framework;
using ProudLlama.CircleGenerator;



public class StrokeDataTest {
    [Test]
    public void TestStrokeArgException() {
        Assert.Throws<System.ArgumentOutOfRangeException>(delegate {
            StrokeData SD = new StrokeData(-10, true);
        });
    }
}




