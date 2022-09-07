using System;

namespace Fluxor.Selectors;

public class MemoizedSelector<TIn1, TResult> : ISelector<TResult>
{
    protected SelectorResult<TResult>? LastResult { get; private set; }

    protected SelectorResult<TIn1>? InS1LastResult { get; private set; }

    private ISelector<TIn1> _inS1;

    private Func<TIn1, TResult>? _projectorFunc;

    public MemoizedSelector(ISelector<TIn1> inS1, Func<TIn1, TResult> projectorFunc)
        : this(inS1)
    {
        _projectorFunc = projectorFunc;
    }

    protected MemoizedSelector(ISelector<TIn1> inS1)
    {
        _inS1 = inS1;
    }

    public TResult Select(IStore state)
    {
        return Select(state, out _);
    }

    public TResult Select(IStore state, out bool resultHasChanged)
    {
        resultHasChanged = false;

        if (UpdateInputResults(state) || LastResult == null)
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
        InS1LastResult = null;
    }

    protected virtual TResult CallProjectorFunc(IStore state)
    {
        if (_projectorFunc == null || InS1LastResult == null)
        {
            throw new InvalidOperationException();
        }

        return _projectorFunc(InS1LastResult.Result);
    }

    protected virtual bool UpdateInputResults(IStore state)
    {
        var newArg1 = _inS1.Select(state);

        if (InS1LastResult == null)
        {
            InS1LastResult = new(newArg1);
            return true;
        }

        var arg1Changed = !DefaultValueEquals(newArg1, InS1LastResult.Result);

        if (!arg1Changed)
        {
            return false;
        }

        InS1LastResult.Result = newArg1;
        return true;
    }

    protected static SelectorResult<T> UpdateInputResult<T>(IStore state, ISelector<T> selector, SelectorResult<T>? lastResult, out bool hasChanges)
    {
        hasChanges = false;
        var newSelectorResult = selector.Select(state);

        if (lastResult == null)
        {
            hasChanges = true;
            return new(newSelectorResult);
        }

        var selectorResultChanged = !DefaultValueEquals(newSelectorResult, lastResult.Result);

        if (!selectorResultChanged)
        {
            return lastResult;
        }

        hasChanges = true;
        return new(newSelectorResult);
    }

    protected static bool DefaultValueEquals<T>(T x, T y) =>
        object.ReferenceEquals(x, y)
        || (x as IEquatable<T>)?.Equals(y) == true
        || object.Equals(x, y);
}

public class MemoizedSelector<TIn1, TIn2, TResult> : MemoizedSelector<TIn1, TResult>
{
    protected SelectorResult<TIn2>? InS2LastResult { get; private set; }

    private ISelector<TIn2> _inS2;

    private Func<TIn1, TIn2, TResult>? _projectorFunc;

    public MemoizedSelector(ISelector<TIn1> inS1, ISelector<TIn2> inS2, Func<TIn1, TIn2, TResult> projectorFunc)
        : this(inS1, inS2)
    {
        _projectorFunc = projectorFunc;
    }

    protected MemoizedSelector(ISelector<TIn1> inS1, ISelector<TIn2> inS2)
        : base(inS1)
    {
        _inS2 = inS2;
    }

    protected override TResult CallProjectorFunc(IStore state)
    {
        if (_projectorFunc == null ||
            InS1LastResult == null ||
            InS2LastResult == null)
        {
            throw new InvalidOperationException();
        }

        return _projectorFunc(InS1LastResult.Result, InS2LastResult.Result);
    }

    protected override bool UpdateInputResults(IStore state)
    {
        bool hasBaseChanges = base.UpdateInputResults(state);

        var result = UpdateInputResult(state, _inS2, InS2LastResult, out var hasChanges);
        if (hasChanges)
        {
            InS2LastResult = result;
        }

        return hasBaseChanges || hasChanges;
    }
}

public class MemoizedSelector<TIn1, TIn2, TIn3, TResult> : MemoizedSelector<TIn1, TIn2, TResult>
{
    protected SelectorResult<TIn3>? InS3LastResult { get; private set; }

    private ISelector<TIn3> _inS3;

    private Func<TIn1, TIn2, TIn3, TResult>? _projectorFunc;

    public MemoizedSelector(
        ISelector<TIn1> inS1,
        ISelector<TIn2> inS2,
        ISelector<TIn3> inS3,
        Func<TIn1, TIn2, TIn3, TResult> projectorFunc)
        : this(inS1, inS2, inS3)
    {
        _projectorFunc = projectorFunc;
    }

    protected MemoizedSelector(
        ISelector<TIn1> inS1,
        ISelector<TIn2> inS2,
        ISelector<TIn3> inS3)
        : base(inS1, inS2)
    {
        _inS3 = inS3;
    }

    protected override TResult CallProjectorFunc(IStore state)
    {
        if (_projectorFunc == null ||
            InS1LastResult == null ||
            InS2LastResult == null ||
            InS3LastResult == null)
        {
            throw new InvalidOperationException();
        }

        return _projectorFunc(
            InS1LastResult.Result,
            InS2LastResult.Result,
            InS3LastResult.Result);
    }

    protected override bool UpdateInputResults(IStore state)
    {
        bool hasBaseChanges = base.UpdateInputResults(state);

        var result = UpdateInputResult(state, _inS3, InS3LastResult, out var hasChanges);
        if (hasChanges)
        {
            InS3LastResult = result;
        }

        return hasBaseChanges || hasChanges;
    }
}
