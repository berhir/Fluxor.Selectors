namespace Fluxor.Selectors.Demo.Store.Counter1;

public static class Counter1Selectors
{
    public static ISelector<Counter1State> SelectFeatureState { get; private set; } = SelectorFactory.CreateFeatureSelector<Counter1State>();

    public static ISelector<int> SelectCount { get; private set; } = SelectorFactory.CreateSelector(SelectFeatureState, state => state.Count);

    public static ISelector<string> SelectCountText { get; private set; } = SelectorFactory.CreateSelector(SelectCount, count =>
    {
        string result = string.Empty;
        if (count == 0)
            result += "Click me";
        else if (count == 1)
            result += $"Clicked {count} time";
        else
            result += $"Clicked {count} times";

        result += $" (Updated at {DateTime.Now})";
        return result;
    });
}
