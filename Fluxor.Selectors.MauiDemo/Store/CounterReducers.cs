namespace Fluxor.Selectors.MauiDemo.Store;

public static class CounterReducers
{
    [ReducerMethod(typeof(CounterActions.IncrementCounterAction))]
    public static CounterState ReduceIncrementCounterAction(CounterState state) =>
        state with { Count = state.Count + 1 };
}