namespace Fluxor.Selectors.Demo.Store.Counter2;

public static class Counter2Reducers
{
    [ReducerMethod(typeof(Counter2Actions.IncrementCounterAction))]
    public static Counter2State ReduceIncrementCounterAction(Counter2State state) =>
        state with { Count = state.Count + 1 };
}