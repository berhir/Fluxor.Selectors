namespace Fluxor.Selectors.Demo.Store.Counter1;

public static class Counter1Reducers
{
    [ReducerMethod(typeof(Counter1Actions.IncrementCounterAction))]
    public static Counter1State ReduceIncrementCounterAction(Counter1State state) =>
        state with { Count = state.Count + 1 };
}
