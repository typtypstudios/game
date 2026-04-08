
public interface IFillableBar
{
    public void UpdateValue(float oldValue, float newValue);
    public void SetValueWithoutTransition(float value);
    public float MaxValue { get; set; }
}
