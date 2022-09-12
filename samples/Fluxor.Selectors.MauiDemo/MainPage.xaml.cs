using Fluxor.Selectors.Demo.Store.Counter1;
using Fluxor.Selectors.Demo.Store.Counter2;

namespace Fluxor.Selectors.MauiDemo;

public partial class MainPage : ContentPage, IDisposable
{
    private static readonly ISelector<int> _selectSum = SelectorFactory.CreateSelector(Counter1Selectors.SelectCount, Counter2Selectors.SelectCount, (count1, count2) => count1 + count2);

    private readonly IDispatcher _dispatcher;
    private bool _disposedValue;

    public ISelectorSubscription<string> Count1Text { get; }

    public ISelectorSubscription<string> Count2Text { get; }

    public ISelectorSubscription<int> Sum { get; }

    public MainPage(IStore store, IDispatcher dispatcher)
    {
        InitializeComponent();
        _dispatcher = dispatcher;

        Count1Text = store.SubscribeSelector(
            Counter1Selectors.SelectCountText,
            text =>
            {
                SemanticScreenReader.Announce(text);
            });

        Count2Text = store.SubscribeSelector(
            Counter2Selectors.SelectCountText,
            text =>
            {
                SemanticScreenReader.Announce(text);
            });

        Sum = store.SubscribeSelector(_selectSum);

        Task.Run(() =>
        {
            Dispatcher.Dispatch(async () => await store.InitializeAsync());
        });

        BindingContext = this;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Count1Text?.Dispose();
                Count2Text?.Dispose();
                Sum?.Dispose();
            }

            _disposedValue = true;
        }
    }

    private void OnCounter1Clicked(object sender, EventArgs e)
    {
        _dispatcher.Dispatch(new Counter1Actions.IncrementCounterAction());
    }

    private void OnCounter2Clicked(object sender, EventArgs e)
    {
        _dispatcher.Dispatch(new Counter2Actions.IncrementCounterAction());
    }
}
