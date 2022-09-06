using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Fluxor.Selectors;

public interface ISelectorSubscription<T> : IState<T>, INotifyPropertyChanged, IDisposable
{
    void Pause();

    void Resume();

    void AddValueChangedHandler(Action<T> handler);
}

public class SelectorSubscription<T> : ISelectorSubscription<T>
{
    private readonly IStore _store;
    private readonly IStateSelector<T> _selector;
    private readonly List<IFeature> _features;
    private readonly List<Action<T>> _valueChangedHandlers = new();
    private bool _isPaused = false;
    private T _lastValue;

    public SelectorSubscription(IStore store, IStateSelector<T> selector)
    {
        _store = store;
        _selector = selector;

        // IStore has no event, so subscribe to all feature changes.
        // Hold a list of all features we subscribed to.
        _features = store.Features.Select(kvp => kvp.Value).ToList();
        _features.ForEach(f => f.StateChanged += Feature_StateChanged);

        // create a wrapper selector to save the last value. the projector function will be called only if the value changed.
        _selector = StateSelectorFactory.CreateSelector(selector, newVal =>
        {
            OnValueChanged(newVal);
            return newVal;
        });

        _lastValue = _selector.Select(_store);
    }

    private void Feature_StateChanged(object sender, EventArgs e)
    {
        OnStateChanged();
    }

    private void OnStateChanged()
    {
        if (_isPaused)
        {
            return;
        }

        _ = _selector.Select(_store);
    }

    public T Value => _lastValue;

    public event EventHandler? StateChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    public void Dispose()
    {
        _features.ForEach(f => f.StateChanged -= Feature_StateChanged);
    }

    public void AddValueChangedHandler(Action<T> handler)
    {
        _valueChangedHandlers.Add(handler);
    }

    public void Pause()
    {
        _isPaused = true;
    }

    public void Resume()
    {
        _isPaused = false;
        OnStateChanged();
    }

    private void OnValueChanged(T newVal)
    {
        _lastValue = newVal;
        StateChanged?.Invoke(this, new EventArgs());
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        _valueChangedHandlers.ForEach(h => h.Invoke(newVal));
    }
}
