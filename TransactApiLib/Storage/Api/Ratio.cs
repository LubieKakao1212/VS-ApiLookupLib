namespace TransactApiLib.Storage.Api;

public readonly struct Ratio(long numerator, long denominator) {

    public readonly long numerator = numerator;
    public readonly long denominator = denominator;
    
}

public readonly struct IntRatio(int numerator, int denominator) {

    public readonly int numerator = numerator;
    public readonly int denominator = denominator;
    
}