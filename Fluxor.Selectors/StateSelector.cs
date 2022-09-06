using System;

namespace Fluxor.Selectors;

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
