namespace Fluxor.Selectors.Demo.Store.Counter2;

public static class Counter2Selectors
{
    public static ISelector<Counter2State> SelectFeatureState { get; private set; } = SelectorFactory.CreateFeatureSelector<Counter2State>();

    public static ISelector<int> SelectCount { get; private set; } = SelectorFactory.CreateSelector(SelectFeatureState, state => state.Count);

    public static ISelector<string> SelectCountText { get; private set; } = SelectorFactory.CreateSelector(SelectCount, count =>
    {
        string result = string.Empty;
        if (count == 0)
        {
            result += "Click me";
        }
        else if (count == 1)
        {
            result += $"Clicked {count} time";
        }
        else
        {
            result += $"Clicked {count} times";
        }

        result += $" (Updated at {DateTime.Now})";
        return result;
    });
}
