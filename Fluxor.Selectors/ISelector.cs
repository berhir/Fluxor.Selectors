namespace Fluxor.Selectors;

public interface ISelector<TResult>
{
    TResult Select(IStore state);
}
