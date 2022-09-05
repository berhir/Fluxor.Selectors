using Fluxor.Selectors.MauiDemo.Store;

namespace Fluxor.Selectors.MauiDemo;

public partial class MainPage : ContentPage, IDisposable
{
    private static readonly IStateSelector<CounterState> Counter1StateSelector = StateSelectorFactory.CreateFeatureSelector<CounterState>();
    private static readonly IStateSelector<int> Count1Selector = StateSelectorFactory.CreateSelector(Counter1StateSelector, (counterState) => counterState.Count);
    private static readonly IStateSelector<int?> Counter1EvenOrNullSelector = StateSelectorFactory.CreateSelector<int, int?>(Count1Selector, (count) => (count % 2 == 0) ? count : null);
    
    private static readonly IStateSelector<Counter2State> Counter2StateSelector = StateSelectorFactory.CreateFeatureSelector<Counter2State>();
    private static readonly IStateSelector<int> Count2Selector = StateSelectorFactory.CreateSelector(Counter2StateSelector, (counterState) => counterState.Count);
    private static readonly IStateSelector<int?> Counter2EvenOrNullSelector = StateSelectorFactory.CreateSelector<int, int?>(Count2Selector, (count) => (count % 2 == 0) ? count : null);

    private static readonly IStateSelector<int> SumSelector = StateSelectorFactory.CreateSelector(Count1Selector, Count2Selector, (count1, count2) => count1 + count2);
    private static readonly IStateSelector<string> AllEvenSelector = StateSelectorFactory.CreateSelector(Counter1EvenOrNullSelector, Counter2EvenOrNullSelector, (count1, count2) => count1.HasValue && count2.HasValue ? "yes" : "no");

    private readonly IDispatcher _dispatcher;
    private bool _disposedValue;

    public ISelectorSubscription<int> Count1 { get; }
    public ISelectorSubscription<int?> EvenOrNullCount1 { get; }
    public ISelectorSubscription<int> Count2 { get; }
    public ISelectorSubscription<int> Sum { get; }
    public ISelectorSubscription<string> AllEven { get; }

    public MainPage(IStore store, IDispatcher dispatcher)
    {
        InitializeComponent();
        _dispatcher = dispatcher;

        Count1 = store.Select(Count1Selector);
        Count1.AddValueChangedHandler(count =>
        {
            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        });

        EvenOrNullCount1 = store.Select(Counter1EvenOrNullSelector);

        Count2 = store.Select(Count2Selector);
        Sum = store.Select(SumSelector);
        AllEven = store.Select(AllEvenSelector);

        Task.Run(() =>
        {
            Dispatcher.Dispatch(async () => await store.InitializeAsync());
        });

        BindingContext = this;
    }

    private void OnCounter1Clicked(object sender, EventArgs e)
    {
        _dispatcher.Dispatch(new CounterActions.IncrementCounterAction());
    }

    private void OnCounter2Clicked(object sender, EventArgs e)
    {
        _dispatcher.Dispatch(new Counter2Actions.IncrementCounterAction());
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Count1?.Dispose();
                Count2?.Dispose();
                EvenOrNullCount1?.Dispose();
                Sum?.Dispose();
                AllEven?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

