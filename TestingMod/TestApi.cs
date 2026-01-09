using Vintagestory.API.MathTools;

namespace TestingMod;

public interface ITestApi {
    public string Message { get; }
    
}

public class TestApiImpl(string message) : ITestApi {
    public string Message => message;
}

public class TestApiBlockPos(BlockPos pos) : ITestApi {
    public string Message => $"Position: {pos}";
}