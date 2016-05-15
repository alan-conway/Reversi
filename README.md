## [Reversi](https://github.com/alan-conway/Reversi)
Building reversi with TDD  
`#TDD #xUnit #Moq #WPF #MVVM #Prism #Unity #TPL #async`

[![Build status](https://ci.appveyor.com/api/projects/status/7236icqvy63ponk9/branch/master?svg=true)](https://ci.appveyor.com/project/alan-conway/reversi/branch/master)      [Source Code](https://github.com/alan-conway/Reversi)

The plan for this project is to explore building some AI logic to win games of reversi without any human input.  
The first phase of this development is to create a game engine that a user can play against, even if the engine isn't unbeatable quite yet. This is now done and if you'd like to play the game, you can do so by fetching the latest build from [here](https://ci.appveyor.com/api/projects/alan-conway/reversi/artifacts/Reversi.zip?branch=master&job=Configuration%3A+Release) and running `Reversi.exe` from any machine with an up-to-date .net runtime on it.

There are 3 main parts to this solution at the moment:  
1. The tests  
2. The reversi game engine  
3. The GUI  

It's unusual to list the tests first but it feels appropriate here because this project is following TDD, so a quick look there before the other components of the project...


### _Tests:_  
`#TDD #xUnit #Moq #TPL #async #ExtensionMethods`  

The tests in this project are written using xUnit and Moq, both of which I recommend.  
I've made frequent use of xUnit's support of _parameterised_ tests through its `[Theory] [InlineData(..)]` attributes.  
For the `GameEngine`, there were times when I wanted to pass various different types of mocked objects into its constructor, and to avoid repeating myself too much throughout the tests, I opted to use a _fluent builder_.  
By 'builder', I am referring to the classical [Builder](https://en.wikipedia.org/wiki/Builder_pattern) design pattern from [GoF](https://www.amazon.co.uk/dp/0201633612). And by 'fluent', I am referring to a [fluent syntax](https://en.wikipedia.org/wiki/Fluent_interface).  
An example of the builder and the syntax are as follows:  

```
private IFoo _foo;
private IBar _bar;

public MyObjBuilder()
{
	_foo = new Mock<IFoo>().Object;
	_bar = new Mock<IBar>().Object;
}

public MyObjBuilder SetFoo(IFoo foo)
{
	_foo = foo;
	return this;
}

public MyObjBuilder SetBar(IBar bar)
{
	_bar = bar;
	return this;
}

public Obj Build()
{
	return new Obj(_foo, _bar);
}
```

The _fluency_ comes from the fact that the SetFoo and SetBar methods return `this` which allows for the following (fluent-style) syntax when they are being called:

```
Obj obj = myObjBuilder
			.SetFoo(customFoo)
			.SetBar(customBar)
			.Build();
```

xUnit also allows us to use the `async` keyword in test method signatures, which is great when we need to `await` the result of a `Task`. Here is an example of such a test to demonstrate:

```
[Fact]
public async void ShouldUpdateMoveNumber()
{
	//Arrange: Create a new game and define the location of the first move
	_engine.CreateNewGame();
	Move move = new Move(34);

	//Act: Process move and increment the move number
	var response = await _engine.UpdateBoardAsync(move);

	//Assert: Engine should have updated MoveNumber to 2 after processing the move
	Assert.Equal(2, _engine.MoveNumber);
}
```

I also wrote an extension method for implementors of INotifyPropertyChanged


### _Engine:_  
`#TDD #TPL #async #DependencyInjection`  

The engine is where most of the logic is found. The GUI is there to convey the user's move to the engine and display the results, but it's the engine which processes all the information.  
The engine only has a few basic tasks:  
* Create a new game  
* Process the opponent's move  
* Choose and play its own move in response

There are a few minor complications which mean that it's slightly less linear than it sounds. (eg when a player has no valid moves and must _pass_ their turn.) But on the whole it is as straightforward as that.  
Of course the interesting piece will be in having the engine choose the best move but I will come to that another day.  
Creating a new game is a simple matter. I chose to keep the state of the game board within an object called `IGameContext`, which can be passed around to those that need it. The squares on the board are kept as a simple array, which is as neat and convenient a representation as any. A `Square` maintains the colour of the piece that it contains (if any) and a couple of other flags, such as whether it is valid for the opponent to play in that square next time around. Creating a new game is a matter of resetting the state of the game context.  
Processing the opponents move requires us to examine the squares that can be reached by moving in any straight line (vertically, horizontally, diagonally) from the square played, and identifying which enemy pieces have been captured. And then capturing them.  
Processing the opponents move results in a `Response` from the engine which contains the new state of the board, reflecting the appropriate pieces correctly captured. This happens `async`-ronously through the use of a `Task`  
Choosing a move for the engine to make is where the fun will be, but for now it identifies all the possible valid moves and then selects one at random. This also takes place from within an `async` method.   


### _GUI:_  
`#TDD #WPF #MVVM #Prism #Unity #TPL #async`  

The GUI is built with WPF using MVVM. It uses Prism for its `EventAggregator` and `DelegateCommand` and Unity for Dependency Injection.  
The game view is constructed using several view files. There is a separate view for the `Game`, the `Board` and a `Cell`.  
The Board view uses a `UniformGrid` as a neat way to create a standard game-board look, and has the following XAML structure:

```
<ItemsControl ItemsSource="{Binding Cells}">

	<ItemsControl.ItemsPanel>
		<ItemsPanelTemplate>
			<UniformGrid Rows="8" Columns="8"/>
		</ItemsPanelTemplate>
	</ItemsControl.ItemsPanel>

	<ItemsControl.ItemTemplate>
		<DataTemplate>
			<ContentControl>
				<local:CellView/>
			</ContentControl>
		</DataTemplate>
	</ItemsControl.ItemTemplate>

</ItemsControl>
```

The Cell view uses an `IValueConverter` to convert the game piece in the cell into the correct `Brush` when drawing the piece as an `Ellipse`.  
It uses a `MouseBinding` to listen for the user clicking on a cell. Upon such a click, assuming it was on a legal square, a `CellSelectedEvent` is published using the `EventAggregator`. All `CellViewModels` and the `GameViewModel` subscribe to this event, the former to de-select any previously selected cells and the latter to convey the move to the engine.
It does this and `awaits` the response which allows the engine to work asynchronously and take an arbitrarily long time, and allows the GUI to appear responsive in the meantime.  
Once this first `Response` is back from the engine, containing the result of the user's move, if the game has not been ended by that move, a subsequent call is made to the engine asking for its move in reply. This is similarly `awaited` for the same reasons.

For `INotifyPropertyChanged`, I am using the [`CallerMemberName`](https://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.callermembernameattribute%28v=vs.110%29.aspx) attribute from within my `ViewModelBase` base class, which does a great job of making sure there isn't any mistake when supplying the name of the property to notify. Of course, when calling `Notify` from within one property, we do still have the option to `Notify` other properties, in which case we would still need to supply the name explicitly (using `nameof()`), but for the most frequent case I think using this attribute is the best approach.
