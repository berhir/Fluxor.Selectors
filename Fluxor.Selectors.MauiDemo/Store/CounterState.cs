namespace Fluxor.Selectors.MauiDemo.Store;

[FeatureState]
public record CounterState
{
    public int Count { get; init; } = 0;
}