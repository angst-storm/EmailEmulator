namespace Estimator;

public class Error
{
    public Error(int totalClicksCount, double value)
    {
        this.totalClicksCount = totalClicksCount;
        this.value = value;
    }

    public int totalClicksCount { get; set; }
    public double value { get; set; }
}