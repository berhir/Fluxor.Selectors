namespace Fluxor.Selectors;

public interface IStateSelector<TResult>
{
    TResult Select(IStore state);
}
