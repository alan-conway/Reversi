## Creating a [Reversi game](https://ci.appveyor.com/api/projects/alan-conway/reversi/artifacts/Reversi.zip?branch=master&job=Configuration%3A+Release) with TDD  
`#TDD #xUnit #Moq #WPF #MVVM #Prism #Unity #TPL #async #AutoFixture`

[![Build status](https://ci.appveyor.com/api/projects/status/7236icqvy63ponk9/branch/master?svg=true)](https://ci.appveyor.com/project/alan-conway/reversi/branch/master)      [Source Code](https://github.com/alan-conway/Reversi)

The goal of this project was to create, using TDD, a working game of Reversi as a basis for learning about AI.  
If you'd like to play the game, you can do so by fetching the zipped binaries from [here](https://ci.appveyor.com/api/projects/alan-conway/reversi/artifacts/Reversi.zip?branch=master&job=Configuration%3A+Release) and running `Reversi.exe` from a windows machine with an up-to-date .net runtime on it.

There are main components in this project are:  

1. [The tests](#the-tests)
1. [The reversi game engine](#the-game-engine)
1. [The AI](#the-ai) in the engine  
1. [The user interface](#the-gui)

Maybe it's unusual to list the tests first but it feels appropriate here because this project is following TDD, so a quick look there before the other components of the project...

### _The Tests:_  
`#TDD #xUnit #Moq #TPL #async #ExtensionMethods #AutoFixture`  

I started out writing the tests in this project using only xUnit and Moq, both of which I highly recommend.  I've now also started to make use of AutoFixture, which is also excellent  
I've made frequent use of xUnit's support of _parameterised_ tests through its `[Theory] [InlineData(..)]` attributes - this is an easy way to reuse tests to check multiple scenarios without having to repeat any code.  

AutoFixture helps to simplify constructing objects to test through its auto mocking container feature. Previously I was using my own builder pattern to handle creating differently constructed instances of my GameEngine class, but now I'm able to define what I need and let AutoFixture worry about mocking everything else and creating the objects.   
I've written more about AutoFixture on my blog and you can [read more about it here](https://alan-conway.github.io/posts/autofixture-and-automocking.html)


xUnit allows me to use the `async` keyword in test method signatures, which is great when I need to `await` the result of a `Task`. Here is an example of such a test to demonstrate:

~~~ C#
[Fact]
public async void ShouldUpdateMoveNumber()
{
	//Arrange: Create a new game and define the location of the move
	_engine.CreateNewGame();
	Move move = new Move(19);

	//Act: Process move and increment the move number from 1 to 2
	var response = await _engine.UpdateBoardAsync(move);

	//Assert:
	Assert.Equal(2, _engine.MoveNumber);
}
~~~

I also wrote an extension method for implementors of INotifyPropertyChanged which lets me conveniently perform a custom Action when the a particular property is changed - this allows me to confirm whether or not changes in property values fire the event as expected.


### _The Game Engine:_  
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

### _The AI:_  
`#TDD`  

The AI within the engine uses a [minimax algorithm with alpha-beta pruning](https://alan-conway.github.io/posts/minimax-with-alpha-beta-pruning.html).  
The idea behind minimax is to look ahead a number of moves and assess which string of moves leads to the best outcome, and the idea behind alpha-beta pruning is to try to find opportunities to speed up that search.  
As part of this process, I make use of heuristics to assign a score to the state of a game. I found that a reasonable heuristic was assign positive scores to capturing a corner and to better 'mobility', and to assign a negative scores to playing next to an empty corner.  
I have written in more detail about the AI [in my blog post](https://alan-conway.github.io/posts/minimax-with-alpha-beta-pruning.html) if you'd like to read more.


### _The GUI:_  
`#TDD #WPF #MVVM #Prism #Unity #TPL #async`  

The GUI is built with WPF using MVVM. It uses Prism for its `EventAggregator` and `DelegateCommand` and Unity for Dependency Injection.  
The game view is constructed using several view files. There is a separate view for the `Game`, the `Board` and a `Cell`.  
The Board view uses a `UniformGrid` as a neat way to create a standard game-board look, and has the following XAML structure:

~~~ XAML
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
~~~

The Cell view uses an `IValueConverter` to convert the game piece in the cell into the correct `Brush` when drawing the piece as an `Ellipse`.  
It uses a `MouseBinding` to listen for the user clicking on a cell. Upon such a click, assuming it was on a legal square, a `CellSelectedEvent` is published using the `EventAggregator`. All `CellViewModels` and the `GameViewModel` subscribe to this event, the former to de-select any previously selected cells and the latter to convey the move to the engine.
It does this and `awaits` the response which allows the engine to work asynchronously and take an arbitrarily long time, and allows the GUI to appear responsive in the meantime.  
Once this first `Response` is back from the engine, containing the result of the user's move, if the game has not been ended by that move, a subsequent call is made to the engine asking for its move in reply. This is similarly `awaited` for the same reasons.

For `INotifyPropertyChanged`, I am using the [`CallerMemberName`](https://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.callermembernameattribute%28v=vs.110%29.aspx) attribute from within my `ViewModelBase` base class, which does a great job of making sure there isn't any mistake when supplying the name of the property to notify. Of course, when calling `Notify` from within one property, we do still have the option to `Notify` other properties, in which case we would still need to supply the name explicitly (using `nameof()`), but for the most frequent case I think using this attribute is the best approach.
