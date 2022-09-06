using System;

namespace Fluxor.Selectors;

public class FeatureStateSelector<TFeatureState> : IStateSelector<TFeatureState>
{
    private Func<IStore, TFeatureState> _projectorFunc;

    public FeatureStateSelector(Func<IStore, TFeatureState> projectorFunc)
    {
        _projectorFunc = projectorFunc;
    }

    public TFeatureState Select(IStore state)
    {
        return _projectorFunc(state);
    }
}
