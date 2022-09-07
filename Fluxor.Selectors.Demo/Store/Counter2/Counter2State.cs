namespace Fluxor.Selectors.Demo.Store.Counter2;

[FeatureState]
public record Counter2State
{
    public int Count { get; init; } = 0;
}