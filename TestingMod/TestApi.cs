namespace TestingMod;

public interface ITestApi {
    public string Message { get; }
    
}

public class TestApiImpl(string message) : ITestApi {

    public string Message { get; set; } = message;

}