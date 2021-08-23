# TicTacToe-Client
TicTacToe game implemented by .net on c#. The client side implemented with Winform and backend side with ASP.NET Core.</br>
Link to Backend side: https://github.com/ShakuriAvi/TicTacToe-Backend.</br>
Client side: The game is played on the client side. In every game the players are: Player vs. System.</br>
the user must log in with a username and password with which he registered on the site before he start the game.</br>
After the user logs in to the site by database (Microsoft SQL server) , there are two options: </br>
* view past games- By using the database, the user can see his past games, with a combobox that displays the games by date and game week.</br>
* start a new game - The game includes a 5X5 board and the rules of the game are similar to TicTacToe.</br> When there are 4 sequence similar elements  (X OR O)  in the same row , diagonally or in the column, the game ends and the winner is displayed.</br>

Server side:The site and the connection to the database are on the server side.</br> The model that I worked is MVC (model view controller).</br>
There are many options in Website:
* Register- This is the game registration page. For to start a game on the client side, the user must logs in to the system.
* Game Table- Using queries this page displays tables of:</br> 1) all of the user's games in the table (on what date and result).</br>
2) How many games each player played.</br>
3) All games the player played in descending order.</br>
* Queries- Using queries this page displays tables of:</br> 1) All players with all their details stored in the database.</br>
2) All games with all their details in the database.</br>
* Edit/Delete- On this page you can delete from the database: players and games.</br> In addition you can edit the player data (username and password).

![game](https://user-images.githubusercontent.com/65177459/130361848-456571b3-cc62-45c2-86f5-6c6dca2f2808.png)
![loginForm](https://user-images.githubusercontent.com/65177459/130361849-3aa2b7a5-d758-444b-ae9c-894b92532755.png)
