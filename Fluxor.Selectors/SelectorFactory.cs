using System;

namespace Fluxor.Selectors;

public static class SelectorFactory
{
    public static MemoizedSelector<TIn1, TResult> CreateSelector<TIn1, TResult>(ISelector<TIn1> inS1, Func<TIn1, TResult> projector)
    {
        return new MemoizedSelector<TIn1, TResult>(inS1, projector);
    }

    public static MemoizedSelector<TIn1, TIn2, TResult> CreateSelector<TIn1, TIn2, TResult>(ISelector<TIn1> inS1, ISelector<TIn2> inS2, Func<TIn1, TIn2, TResult> projector)
    {
        return new MemoizedSelector<TIn1, TIn2, TResult>(inS1, inS2, projector);
    }

    public static MemoizedSelector<TIn1, TIn2, TIn3, TResult> CreateSelector<TIn1, TIn2, TIn3, TResult>(
        ISelector<TIn1> inS1,
        ISelector<TIn2> inS2,
        ISelector<TIn3> inS3,
        Func<TIn1, TIn2, TIn3, TResult> projector)
    {
        return new MemoizedSelector<TIn1, TIn2, TIn3, TResult>(inS1, inS2, inS3, projector);
    }

    public static MemoizedSelector<TIn1, TIn2, TIn3, TIn4, TResult> CreateSelector<TIn1, TIn2, TIn3, TIn4, TResult>(
        ISelector<TIn1> inS1,
        ISelector<TIn2> inS2,
        ISelector<TIn3> inS3,
        ISelector<TIn4> inS4,
        Func<TIn1, TIn2, TIn3, TIn4, TResult> projector)
    {
        return new MemoizedSelector<TIn1, TIn2, TIn3, TIn4, TResult>(inS1, inS2, inS3, inS4, projector);
    }

    public static MemoizedSelector<TIn1, TIn2, TIn3, TIn4, TIn5, TResult> CreateSelector<TIn1, TIn2, TIn3, TIn4, TIn5, TResult>(
        ISelector<TIn1> inS1,
        ISelector<TIn2> inS2,
        ISelector<TIn3> inS3,
        ISelector<TIn4> inS4,
        ISelector<TIn5> inS5,
        Func<TIn1, TIn2, TIn3, TIn4, TIn5, TResult> projector)
    {
        return new MemoizedSelector<TIn1, TIn2, TIn3, TIn4, TIn5, TResult>(inS1, inS2, inS3, inS4, inS5, projector);
    }

    public static MemoizedSelector<TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TResult> CreateSelector<TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TResult>(
        ISelector<TIn1> inS1,
        ISelector<TIn2> inS2,
        ISelector<TIn3> inS3,
        ISelector<TIn4> inS4,
        ISelector<TIn5> inS5,
        ISelector<TIn6> inS6,
        Func<TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TResult> projector)
    {
        return new MemoizedSelector<TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TResult>(inS1, inS2, inS3, inS4, inS5, inS6, projector);
    }

    public static Selector<TFeatureState> CreateFeatureSelector<TFeatureState>(string name)
    {
        return new Selector<TFeatureState>(store =>
        {
            if (store.Features[name]?.GetState() is TFeatureState state)
            {
                return state;
            }

            throw new InvalidOperationException();
        });
    }

    public static Selector<TFeatureState> CreateFeatureSelector<TFeatureState>()
    {
        // todo: get name from attribute if present
        return CreateFeatureSelector<TFeatureState>(typeof(TFeatureState).FullName);
    }
}
