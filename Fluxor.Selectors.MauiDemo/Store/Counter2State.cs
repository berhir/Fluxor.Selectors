namespace Fluxor.Selectors.MauiDemo.Store;

[FeatureState]
public record Counter2State
{
    public int Count { get; init; } = 0;
}