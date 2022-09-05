using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Fluxor.Selectors
{
    public class SelectorResult<TResult>
    {
        public TResult Result { get; set; }

        public SelectorResult(TResult result)
        {
            Result = result;
        }
    }

    public interface IStateSelector<TResult>
    {
        TResult Select(IStore state);
    }

    public class StateSelector<TIn1, TResult> : IStateSelector<TResult>
    {
        protected SelectorResult<TResult>? LastResult { get; private set; }

        protected SelectorResult<TIn1>? InputSelector1LastResult { get; private set; }

        private IStateSelector<TIn1> _inputSelector1;

        private Func<TIn1, TResult>? _projectorFunc;

        public StateSelector(IStateSelector<TIn1> inputSelector1, Func<TIn1, TResult> projectorFunc)
            : this(inputSelector1)
        {
            _projectorFunc = projectorFunc;
        }

        protected StateSelector(IStateSelector<TIn1> inputSelector1)
        {
            _inputSelector1 = inputSelector1;
        }

        public TResult Select(IStore state)
        {
            return Select(state, out _);
        }

        public TResult Select(IStore state, out bool resultHasChanged)
        {
            resultHasChanged = false;

            if (UpdateInputArguments(state) || LastResult == null)
            {
                var newResult = CallProjectorFunc(state);

                if (LastResult == null)
                {
                    resultHasChanged = true;
                    LastResult = new(newResult);
                    return newResult;
                }

                resultHasChanged = !DefaultValueEquals(newResult, LastResult.Result);

                if (resultHasChanged)
                {
                    LastResult.Result = newResult;
                    return newResult;
                }
            }

            return LastResult.Result;
        }

        public virtual void Release()
        {
            LastResult = null;
            InputSelector1LastResult = null;
        }

        protected virtual TResult CallProjectorFunc(IStore state)
        {
            if (_projectorFunc == null || InputSelector1LastResult == null)
            {
                throw new InvalidOperationException();
            }

            return _projectorFunc(InputSelector1LastResult.Result);
        }

        protected virtual bool UpdateInputArguments(IStore state)
        {
            var newArg1 = _inputSelector1.Select(state);

            if (InputSelector1LastResult == null)
            {
                InputSelector1LastResult = new(newArg1);
                return true;
            }

            var arg1Changed = !DefaultValueEquals(newArg1, InputSelector1LastResult.Result);

            if (!arg1Changed)
            {
                return false;
            }

            InputSelector1LastResult.Result = newArg1;
            return true;
        }

        protected static bool DefaultValueEquals<T>(T x, T y) =>
            object.ReferenceEquals(x, y)
            || (x as IEquatable<T>)?.Equals(y) == true
            || object.Equals(x, y);
    }

    public class StateSelector<TIn1, TIn2, TResult> : StateSelector<TIn1, TResult>
    {
        protected SelectorResult<TIn2>? InputSelector2LastResult { get; private set; }

        private IStateSelector<TIn2> _inputSelector2;

        private Func<TIn1, TIn2, TResult>? _projectorFunc;

        public StateSelector(IStateSelector<TIn1> inputSelector1, IStateSelector<TIn2> inputSelector2, Func<TIn1, TIn2, TResult> projectorFunc)
            : this(inputSelector1, inputSelector2)
        {
            _projectorFunc = projectorFunc;
        }

        protected StateSelector(IStateSelector<TIn1> inputSelector1, IStateSelector<TIn2> inputSelector2)
            : base(inputSelector1)
        {
            _inputSelector2 = inputSelector2;
        }

        protected override TResult CallProjectorFunc(IStore state)
        {
            if (_projectorFunc == null ||
                InputSelector1LastResult == null ||
                InputSelector2LastResult == null)
            {
                throw new InvalidOperationException();
            }

            return _projectorFunc(InputSelector1LastResult.Result, InputSelector2LastResult.Result);
        }

        protected override bool UpdateInputArguments(IStore state)
        {
            bool hasChanges = base.UpdateInputArguments(state);

            var newSelectorResult = _inputSelector2.Select(state);

            if (InputSelector2LastResult == null)
            {
                InputSelector2LastResult = new(newSelectorResult);
                return true;
            }

            var selectorResultChanged = !DefaultValueEquals(newSelectorResult, InputSelector2LastResult.Result);

            if (!selectorResultChanged)
            {
                return hasChanges;
            }

            InputSelector2LastResult.Result = newSelectorResult;
            return true;

        }
    }

    public class StateSelector<TIn1, TIn2, TIn3, TResult> : StateSelector<TIn1, TIn2, TResult>
    {
        protected SelectorResult<TIn3>? InputSelector3LastResult { get; private set; }

        private IStateSelector<TIn3> _inputSelector3;

        private Func<TIn1, TIn2, TIn3, TResult>? _projectorFunc;

        public StateSelector(
            IStateSelector<TIn1> inputSelector1,
            IStateSelector<TIn2> inputSelector2,
            IStateSelector<TIn3> inputSelector3,
            Func<TIn1, TIn2, TIn3, TResult> projectorFunc)
            : this(inputSelector1, inputSelector2, inputSelector3)
        {
            _projectorFunc = projectorFunc;
        }

        protected StateSelector(
            IStateSelector<TIn1> inputSelector1,
            IStateSelector<TIn2> inputSelector2,
            IStateSelector<TIn3> inputSelector3)
            : base(inputSelector1, inputSelector2)
        {
            _inputSelector3 = inputSelector3;
        }

        protected override TResult CallProjectorFunc(IStore state)
        {
            if (_projectorFunc == null ||
                InputSelector1LastResult == null ||
                InputSelector2LastResult == null ||
                InputSelector3LastResult == null)
            {
                throw new InvalidOperationException();
            }

            return _projectorFunc(
                InputSelector1LastResult.Result,
                InputSelector2LastResult.Result,
                InputSelector3LastResult.Result);
        }

        protected override bool UpdateInputArguments(IStore state)
        {
            bool hasChanges = base.UpdateInputArguments(state);

            var newSelectorResult = _inputSelector3.Select(state);

            if (InputSelector3LastResult == null)
            {
                InputSelector3LastResult = new(newSelectorResult);
                return true;
            }

            var selectorResultChanged = !DefaultValueEquals(newSelectorResult, InputSelector3LastResult.Result);

            if (!selectorResultChanged)
            {
                return hasChanges;
            }

            InputSelector3LastResult.Result = newSelectorResult;
            return true;

        }
    }

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
            // todo: get name from attribute if set
            return CreateFeatureSelector<TFeatureState>(typeof(TFeatureState).FullName);
        }
    }

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

    public static class IStoreExtensions
    {
        public static ISelectorSubscription<TResult> Select<TResult>(this IStore store, IStateSelector<TResult> selector)
        {
            return new SelectorSubscription<TResult>(store, selector);
        }
    }
}
