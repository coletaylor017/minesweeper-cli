### Minesweeper

© Cole Taylor 2020

A Basic C# implementation of classic Minesweeper. Uses MVC architecture, so a point-and-click GUI could be implemented fairly easily in the future by interfacing with existing controller-defined methods. 

Controls are explained in-game. You can choose to render the game board as greyscale or color. Grayscale renders faster, so it is better for larger boards.

TODO:
- Big rendering algo improvements. Needs to find differences between previous model state and current model state,
and then for each difference it finds needs to move the cursor to that position and overwrite the existing two
characters (because each grid square is two characters).
