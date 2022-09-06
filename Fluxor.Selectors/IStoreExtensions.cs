namespace Fluxor.Selectors;

public static class IStoreExtensions
{
    public static ISelectorSubscription<TResult> Select<TResult>(this IStore store, IStateSelector<TResult> selector)
    {
        return new SelectorSubscription<TResult>(store, selector);
    }
}
