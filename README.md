# Fluxor.Selectors

A PoC for memoized selectors for Fluxor. They work similar as selectors in [ngrx/store](https://ngrx.io/guide/store/selectors) and [Redux](https://redux.js.org/usage/deriving-data-selectors).

I am quoting the ngrx/store documentation for the benefits:

Selectors are pure functions used for obtaining slices of store state. Fluxor.Selectors provide a few helper functions for optimizing this selection. Selectors provide many features when selecting slices of state:

* Portability
* Memoization
* Composition
* Testability
* Type Safety

When using the SelectorFactory.CreateSelector and SelectorFactory.CreateFeatureSelector functions Fluxor.Selectors keep track of the latest arguments in which your selector function was invoked. Because selectors are pure functions, the last result can be returned when the arguments match without reinvoking your selector function. This can provide performance benefits, particularly with selectors that perform expensive computation. This practice is known as [memoization](https://en.wikipedia.org/wiki/Memoization).

## Create Selectors
The easiest way to create selectors is to use the factory functions:
```csharp
var selectCounterState = SelectorFactory.CreateFeatureSelector<CounterState>();
var selectCount = SelectorFactory.CreateSelector(selectCounterState, state => state.Count);
```

## Selecting Values
There are two ways to use selectors. But before the selectors can be used, we need access to the `IStore`. Usually we get it via dependency injection.
1. To return the selector result immediately, call the `Select` extension method.
```csharp
// get via dependency injection
IStore store;

var count = store.Select(selectCount);
```

2. Create a subscription that notifies when the selected value changes. A `SelectorSubscription` implements `IState<T>` (which implements `IStateChangedNotifier`)  and `INotifyPropertyChanged`.
```csharp
// get via dependency injection
IStore store;

var countSubscription = store.SubscribeSelector(selectCount);
```
Then the subscription can be used in a Razor component
```html
<p role="status">Count: @countSubscription.Value</p>
```
or bind it in .NET MAUI or WPF
```xml
<Label Text="{Binding countSubscription.Value, StringFormat='Count: {0}'}" />
```
