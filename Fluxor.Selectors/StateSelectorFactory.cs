using System;

namespace Fluxor.Selectors;

public static class StateSelectorFactory
{
    public static StateSelector<S1, TResult> CreateSelector<S1, TResult>(IStateSelector<S1> s1, Func<S1, TResult> projector)
    {
        return new StateSelector<S1, TResult>(s1, projector);
    }

    public static StateSelector<S1, S2, TResult> CreateSelector<S1, S2, TResult>(IStateSelector<S1> s1, IStateSelector<S2> s2, Func<S1, S2, TResult> projector)
    {
        return new StateSelector<S1, S2, TResult>(s1, s2, projector);
    }

    public static StateSelector<S1, S2, S3, TResult> CreateSelector<S1, S2, S3, TResult>(
        IStateSelector<S1> s1,
        IStateSelector<S2> s2,
        IStateSelector<S3> s3,
        Func<S1, S2, S3, TResult> projector)
    {
        return new StateSelector<S1, S2, S3, TResult>(s1, s2, s3, projector);
    }

    public static FeatureStateSelector<TFeatureState> CreateFeatureSelector<TFeatureState>(string name)
    {
        return new FeatureStateSelector<TFeatureState>(store =>
        {
            if (store.Features[name]?.GetState() is TFeatureState state)
            {
                return state;
            }

            throw new InvalidOperationException();
        });
    }

    public static FeatureStateSelector<TFeatureState> CreateFeatureSelector<TFeatureState>()
    {
        // todo: get name from attribute if present
        return CreateFeatureSelector<TFeatureState>(typeof(TFeatureState).FullName);
    }
}
