namespace Fluxor.Selectors.Demo.Store.Counter1;

[FeatureState]
public record Counter1State
{
    public int Count { get; init; } = 0;
}