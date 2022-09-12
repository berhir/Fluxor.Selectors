using System;

namespace Fluxor.Selectors;

public static class IStoreExtensions
{
    public static ISelectorSubscription<TResult> SubscribeSelector<TResult>(this IStore store, ISelector<TResult> selector)
    {
        return new SelectorSubscription<TResult>(store, selector);
    }

    public static ISelectorSubscription<TResult> SubscribeSelector<TResult>(this IStore store, ISelector<TResult> selector, Action<TResult> handler)
    {
        return new SelectorSubscription<TResult>(store, selector, handler);
    }

    public static TResult Select<TResult>(this IStore store, ISelector<TResult> selector)
    {
        return selector.Select(store);
    }
}
