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

    public override void Release()
    {
        base.Release();
        InS2LastResult = null;
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

    public override void Release()
    {
        base.Release();
        InS3LastResult = null;
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

public class MemoizedSelector<TIn1, TIn2, TIn3, TIn4, TResult> : MemoizedSelector<TIn1, TIn2, TIn3, TResult>
{
    protected SelectorResult<TIn4>? InS4LastResult { get; private set; }

    private ISelector<TIn4> _inS4;

    private Func<TIn1, TIn2, TIn3, TIn4, TResult>? _projectorFunc;

    public MemoizedSelector(
        ISelector<TIn1> inS1,
        ISelector<TIn2> inS2,
        ISelector<TIn3> inS3,
        ISelector<TIn4> inS4,
        Func<TIn1, TIn2, TIn3, TIn4, TResult> projectorFunc)
        : this(inS1, inS2, inS3, inS4)
    {
        _projectorFunc = projectorFunc;
    }

    protected MemoizedSelector(
        ISelector<TIn1> inS1,
        ISelector<TIn2> inS2,
        ISelector<TIn3> inS3,
        ISelector<TIn4> inS4)
        : base(inS1, inS2, inS3)
    {
        _inS4 = inS4;
    }

    public override void Release()
    {
        base.Release();
        InS4LastResult = null;
    }

    protected override TResult CallProjectorFunc(IStore state)
    {
        if (_projectorFunc == null ||
            InS1LastResult == null ||
            InS2LastResult == null ||
            InS3LastResult == null ||
            InS4LastResult == null)
        {
            throw new InvalidOperationException();
        }

        return _projectorFunc(
            InS1LastResult.Result,
            InS2LastResult.Result,
            InS3LastResult.Result,
            InS4LastResult.Result);
    }

    protected override bool UpdateInputResults(IStore state)
    {
        bool hasBaseChanges = base.UpdateInputResults(state);

        var result = UpdateInputResult(state, _inS4, InS4LastResult, out var hasChanges);
        if (hasChanges)
        {
            InS4LastResult = result;
        }

        return hasBaseChanges || hasChanges;
    }
}

public class MemoizedSelector<TIn1, TIn2, TIn3, TIn4, TIn5, TResult> : MemoizedSelector<TIn1, TIn2, TIn3, TIn4, TResult>
{
    protected SelectorResult<TIn5>? InS5LastResult { get; private set; }

    private ISelector<TIn5> _inS5;

    private Func<TIn1, TIn2, TIn3, TIn4, TIn5, TResult>? _projectorFunc;

    public MemoizedSelector(
        ISelector<TIn1> inS1,
        ISelector<TIn2> inS2,
        ISelector<TIn3> inS3,
        ISelector<TIn4> inS4,
        ISelector<TIn5> inS5,
        Func<TIn1, TIn2, TIn3, TIn4, TIn5, TResult> projectorFunc)
        : this(inS1, inS2, inS3, inS4, inS5)
    {
        _projectorFunc = projectorFunc;
    }

    protected MemoizedSelector(
        ISelector<TIn1> inS1,
        ISelector<TIn2> inS2,
        ISelector<TIn3> inS3,
        ISelector<TIn4> inS4,
        ISelector<TIn5> inS5)
        : base(inS1, inS2, inS3, inS4)
    {
        _inS5 = inS5;
    }

    public override void Release()
    {
        base.Release();
        InS5LastResult = null;
    }

    protected override TResult CallProjectorFunc(IStore state)
    {
        if (_projectorFunc == null ||
            InS1LastResult == null ||
            InS2LastResult == null ||
            InS3LastResult == null ||
            InS4LastResult == null ||
            InS5LastResult == null)
        {
            throw new InvalidOperationException();
        }

        return _projectorFunc(
            InS1LastResult.Result,
            InS2LastResult.Result,
            InS3LastResult.Result,
            InS4LastResult.Result,
            InS5LastResult.Result);
    }

    protected override bool UpdateInputResults(IStore state)
    {
        bool hasBaseChanges = base.UpdateInputResults(state);

        var result = UpdateInputResult(state, _inS5, InS5LastResult, out var hasChanges);
        if (hasChanges)
        {
            InS5LastResult = result;
        }

        return hasBaseChanges || hasChanges;
    }
}

public class MemoizedSelector<TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TResult> : MemoizedSelector<TIn1, TIn2, TIn3, TIn4, TIn5, TResult>
{
    protected SelectorResult<TIn6>? InS6LastResult { get; private set; }

    private ISelector<TIn6> _inS6;

    private Func<TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TResult>? _projectorFunc;

    public MemoizedSelector(
        ISelector<TIn1> inS1,
        ISelector<TIn2> inS2,
        ISelector<TIn3> inS3,
        ISelector<TIn4> inS4,
        ISelector<TIn5> inS5,
        ISelector<TIn6> inS6,
        Func<TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TResult> projectorFunc)
        : this(inS1, inS2, inS3, inS4, inS5, inS6)
    {
        _projectorFunc = projectorFunc;
    }

    protected MemoizedSelector(
        ISelector<TIn1> inS1,
        ISelector<TIn2> inS2,
        ISelector<TIn3> inS3,
        ISelector<TIn4> inS4,
        ISelector<TIn5> inS5,
        ISelector<TIn6> inS6)
        : base(inS1, inS2, inS3, inS4, inS5)
    {
        _inS6 = inS6;
    }

    public override void Release()
    {
        base.Release();
        InS6LastResult = null;
    }

    protected override TResult CallProjectorFunc(IStore state)
    {
        if (_projectorFunc == null ||
            InS1LastResult == null ||
            InS2LastResult == null ||
            InS3LastResult == null ||
            InS4LastResult == null ||
            InS5LastResult == null ||
            InS6LastResult == null)
        {
            throw new InvalidOperationException();
        }

        return _projectorFunc(
            InS1LastResult.Result,
            InS2LastResult.Result,
            InS3LastResult.Result,
            InS4LastResult.Result,
            InS5LastResult.Result,
            InS6LastResult.Result);
    }

    protected override bool UpdateInputResults(IStore state)
    {
        bool hasBaseChanges = base.UpdateInputResults(state);

        var result = UpdateInputResult(state, _inS6, InS6LastResult, out var hasChanges);
        if (hasChanges)
        {
            InS6LastResult = result;
        }

        return hasBaseChanges || hasChanges;
    }
}
