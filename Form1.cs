using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Tic_Tac_Toe_Backend
{
    public partial class Form1 : Form
    {
        private static HttpClient client = new HttpClient();
        Boolean turnPlayer;
        Boolean gameOver;
        Boolean autoGame;
        List<Player> players; // list of all players in game
        int place; // place of player
        int countTurn = 0;
        int[,] arr;
        List<Control> list = new List<Control>();
        List<(int, int)> newGameSteps = new List<(int, int)>();
        List<Game> allGames;
        string gameResult = "";


        // system turn
        async Task turnSystemAsync()
        {
            if (countTurn < 24)
            {
                string str = string.Join(",", arr.OfType<int>()
                     .Select((value, index) => new { value, index })
                     .GroupBy(x => x.index / arr.GetLength(1))
                     .Select(x => $"{{{string.Join(",", x.Select(y => y.value))}}}")); // change 2D to String
                string json = await getStepFromServerAsync(str);
                JObject jObject = JObject.Parse(json);
                int row = (int)jObject["row"];
                int col = (int)jObject["col"];
        
                getButton(row, col, 0, false);
            }
            else
            {
                gameOver = true;
                gameResult = "Draw";
                saveGame();
                MessageBox.Show(String.Format("Game Over " + gameResult));
            }

        }
        // get random system step from server
        private async Task<string> getStepFromServerAsync(String arrayToString)
        {
            HttpResponseMessage response = await client.GetAsync("api/player/systemstep?steps=" + arrayToString);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;

        }

        //load form - user's Data AND his Game
        internal Form1(List<Player> players, int place)
        {
            InitializeComponent();
            GetAllControl(this, list);
            this.players = players;
            this.place = place;
            userNameLabel.Text = players[place].Name;
            winsLabel.Text = "Wins : " + players[place].Wins;
            drawsLabel.Text = "Draws : " + players[place].Draws;
            losesLabel.Text = "Loses : " + players[place].Loses;
            client.BaseAddress = new Uri("https://localhost:44368/");
            getPlayerGames();


        }
        private void addToComboBox()
        {
            foreach (var game in allGames)
            {
                comboBox.Items.Add(game.Result + " " +game.Date.ToString("MM/dd/yyyy"));
            }
        }
       

        private async void getPlayerGames()
        {
            String json = await GetJsonAsync("api/player/playergames?id=" + players[place].Id);
            convertFromJsonToGame(json);
        }
        private static async Task<string> GetJsonAsync(string path)
        {

            HttpResponseMessage response = await client.GetAsync(path);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        //convert json to games
        private void convertFromJsonToGame(String json)
        {
            allGames = new List<Game>();
            // var rcvdData = JsonConvert.DeserializeObject<LocationData>(arr /* <-- Here */.ToString(), settings);
            JObject jObject = JObject.Parse(json);
            JToken jsonToken = jObject["data"];
            Array jsonGames = jsonToken.ToArray();
            foreach (var jsonGame in jsonGames)
            {
                allGames.Add(initNewGame((JObject)jsonGame));

            }
            addToComboBox();

        }


        private Game initNewGame(JObject jObject)
        {
            int PlayerId = (int)jObject["playerId"];
            List<(int, int)> GameSteps = new List<(int, int)>();
            string arr = jObject["gameSteps"].ToString();
            var temp = arr.Trim('{', '}');
            var res = temp.Trim('[', ']')
            .Split(',')
            .Select(y => y.Trim('"'))
            .ToArray();
            for (int i = 0; i < res.Length; i += 2)
            {

                int x = int.Parse(res[i][res[i].Length - 1].ToString());
                int y = int.Parse(res[i + 1][res[i + 1].Length - 2].ToString());
                GameSteps.Add((x, y));
            }

            DateTime Date = (DateTime)jObject["date"];
            int Id = (int)jObject["id"];
            string result = (string)jObject["result"];
            Game newGame = (new Game { PlayerId = players[place].Id, GameSteps = GameSteps, Date = Date, Result = result });
            return newGame;
        }
       
        // for find all buttons in forms
        private void GetAllControl(Control c, List<Control> list)
        {
            foreach (Control control in c.Controls)
            {
                list.Add(control);

                if (control.GetType() == typeof(Panel))
                    GetAllControl(control, list);
            }
        }
       
        // when game finish save the game on DB and add to combobox
        private void saveGame()
        {

            startBtn.Enabled = true;
            showBtn.Enabled = true;
            _ = gameResult == "Win" ? players[place].Wins += 1 : (gameResult == "Lose" ? players[place].Loses += 1 : players[place].Draws += 1);
            _ = gameResult == "Win" ? winsLabel.Text= "Wins: " + players[place].Wins.ToString() : (gameResult == "Lose" ? losesLabel.Text = "Loses: " + players[place].Loses.ToString() : drawsLabel.Text = "Draws: " + players[place].Draws.ToString());
            DateTime aDate = DateTime.Now;
            Game game= (new Game { PlayerId = players[place].Id, GameSteps = newGameSteps, Date = aDate,Result = gameResult });
            allGames.Add(game);
            comboBox.Items.Add(gameResult + " " + aDate.ToString("MM/dd/yyyy"));

            save(game);
        }
      
        //add game to DB with postRequest and update details player (win, lose or draw)
        private async void save(Game game)
        {
            await UpdatePlayersAsync(players[place]);
            await PostGamesAsync(game);

        }
        

        private async Task<Uri> PostGamesAsync(Game game)
        {
            string jsonString = JsonConvert.SerializeObject(game.GameSteps);
            HttpResponseMessage response = await client.PostAsJsonAsync("api/player/game?gameSteps=" + jsonString + "&date=" + game.Date.ToString("MM/dd/yyyy") + "&id=" + game.PlayerId + "&result=" + game.Result, game);
            response.EnsureSuccessStatusCode();
            return response.Headers.Location;

        }

        static async Task<Uri> UpdatePlayersAsync(Player player)
        {

            HttpResponseMessage response = await client.PutAsJsonAsync("api/player?id=" + player.Id + "&win=" + player.Wins + "&draw=" + player.Draws + "&lose=" + player.Loses, player);
            response.EnsureSuccessStatusCode();
            return response.Headers.Location;
        }

        //paint the lines on board
        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            Graphics toPass = panel3.CreateGraphics();
            GFX engine = new GFX(toPass);

        }
        // A subsystem that handles  the game turns
        private async void changeButton(int row, int col, Button btn)
        {
            newGameSteps.Add((row, col));
            //steps += row + col + ",";
            countTurn++;
            btn.Enabled = false;
            if (turnPlayer == true)
            {
                arr[row, col] = 1;
                btn.Text = "X";
                btn.BackColor = Color.Green;
                if (countTurn > 6)
                {
                    if (checkBoard(row, col, false, -1) == true)
                    {
                        MessageBox.Show(String.Format("Game Over : Player Win"));
                        gameOver = true;
                        gameResult = "Win";
                        saveGame();
                        return;
                    }
                }
                //await Task.Run(()=> task());
                turnPlayer = false;
                turnSystemAsync();

            }
            else
            {
                arr[row, col] = 2;
                btn.Text = "O";
                btn.BackColor = Color.Red;
                if (countTurn > 6)
                {
                    if (checkBoard(row, col, false, -1) == true)
                    {
                        MessageBox.Show(String.Format("Game Over : System Win"));
                        gameOver = true;
                        //p.Loses += 1;
                        gameResult = "Lose";
                        saveGame();
                        return;
                    }
                }
                turnPlayer = true;
            }

        }
        // for delay between turns (unusing)
        private void task()
        {
            turnPlayer = false;
            Thread.Sleep(3000);
            turnSystemAsync();
            return;
        }
        // An algorithm that checks if the game ends
        private Boolean checkBoard(int row, int col, bool AutoGame, int numStep)
        {
            int check = 0;
            if (autoGame == false)
                check = turnPlayer == true ? 1 : 2;
            else
            {
                check = numStep % 2 == 0 ? 1 : 2;
            }

            int countSequence = 0;
            for (int i = 0; i < 5; i++)
            {
                if (arr[i, col] == check)
                    countSequence++;
                else if (countSequence != 4)
                    countSequence = 0;
            }

            if (countSequence >= 4)
            {
                return true;
            }
            countSequence = 0;


            for (int i = 0; i < 5; i++)
            {
                if (arr[row, i] == check)
                    countSequence++;
                else if (countSequence != 4)
                    countSequence = 0;
            }
            if (countSequence >= 4)
                return true;
            else
                countSequence = 0;
            int tempRow = row;
            int tempCol = col;
            while (tempRow != 0 && tempCol != 0)
            {
                tempRow--;
                tempCol--;
            }

            for (int i = 0; i < 5; i++)
            {

                if (tempCol + i < 5 && tempRow + i < 5 && arr[tempRow + i, tempCol + i] == check)
                    countSequence++;
                else if (countSequence != 4)
                    countSequence = 0;

            }
            if (countSequence >= 4)
                return true;
            else
                countSequence = 0;


            tempRow = row;
            tempCol = col;
            while (tempRow != 0 && tempCol != 4)
            {
                tempRow--;
                tempCol++;
            }
            for (int i = 0; i < 5; i++)
            {
                if (tempCol - i > -1 && tempRow + i < 5 && arr[tempRow + i, tempCol - i] == check)
                    countSequence++;
                else if (countSequence != 4)
                    countSequence = 0;
            }
            if (countSequence >= 4)
                return true;
            else
                countSequence = 0;
            return false;


        }
       
        //click on board button
        private void button_Click(object sender, EventArgs e)
        {
            if (turnPlayer == true && gameOver != true && autoGame == false)
            {
                Button cb = (sender as Button);
                //  MessageBox.Show(String.Format("Location of clicked Button : {0}, {1}.", cb.Location.X, cb.Location.Y)); // This is just for example
                // Graphics toPass = panel3.CreateGraphics();
                int row = cb.Name[0] == 'A' ? 0 : (cb.Name[0] == 'B' ? 1 : (cb.Name[0] == 'C' ? 2 : (cb.Name[0] == 'D' ? 3 : 4)));
                int col = cb.Name[1] == '1' ? 0 : (cb.Name[1] == '2' ? 1 : (cb.Name[1] == '3' ? 2 : (cb.Name[1] == '4' ? 3 : 4)));
                changeButton(row, col, cb);



                // cb.Hide();

                //Refresh();
                // paintShape(row,col, cb.Location.X, cb.Location.Y);


            }
        }
        // when show btn click , init the necessary field and start auto game (record game)
        private void ShowBtn_Click(object sender, EventArgs e)
        {
            autoGame = true;
            turnPlayer = false;
            startBtn.Enabled = false;
            showBtn.Enabled = false;
            clearButtons();
            arr = new int[,] { { -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1 } };//-1 empty , 1 X , 2 O
            int choice = comboBox.SelectedIndex;
            playAutoGame(choice);
         //   startBtn.Enabled = true;
         //   showBtn.Enabled = true;
        }

        // when start btn click , init the necessary field and start game
        private void StartBtn_Click(object sender, EventArgs e)
        {
            showBtn.Enabled = false;
            startBtn.Enabled = false;
            gameOver = false;
            autoGame = false;
            turnPlayer = true;
            clearButtons();
            arr = new int[,] { { -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1 } };//-1 empty , 1 X , 2 O
            countTurn = 0;
            gameResult = "";
            newGameSteps = new List<(int, int)>();
        }

        private async void playAutoGame(int choice)
        {
            await Task.Run(() => AutoGametask());
            List<(int, int)> temp = choice >= allGames.Count ? allGames[allGames.Count + (choice % allGames.Count)].GameSteps : allGames[choice].GameSteps; // get the choice game
            for (int i = 0; i < temp.Count; i++)
            {
                int row = temp[i].Item1;
                int col = temp[i].Item2;
                getButton(row, col, i, autoGame);
            }
            if(autoGame == true)
            {
                MessageBox.Show(String.Format("Game Over : Draw"));
                finishAutoGame();
            }

        }
      
        // init all button (before game start or record start)
        private void clearButtons()
        {
            foreach (Control control in list)
            {

                if (control.GetType() == typeof(Button))
                {
                    var btn = control as Button;
                    if (btn.Text == "X" || btn.Text == "O")
                    {
                        btn.Text = "";
                        btn.Enabled = true;
                        btn.BackColor = Color.White;
                    }
                }

            }
        }
        // identify the correct click button
        private void getButton(int row, int col, int numStep, Boolean autoGame)
        {
            char rowButton = row == 0 ? 'A' : (row == 1 ? 'B' : (row == 2 ? 'C' : (row == 3 ? 'D' : 'E')));
            char colButton = col == 0 ? '1' : (col == 1 ? '2' : (col == 2 ? '3' : (col == 3 ? '4' : '5')));
            var builder = new StringBuilder();
            builder.Append(rowButton);
            builder.Append(colButton);
            String nameButton = builder.ToString();
            foreach (Control control in list)
            {
                if (control.GetType() == typeof(Button))
                {
                    var button = control as Button;
                    if (nameButton.Equals(button.Name))
                    {
                        if (!autoGame)
                            changeButton(row, col, button);
                        else
                        {

                            changeButtonAuto(row, col, numStep, button);
                        }
                        break;
                    }
                    //all btn
                }
            }
        }

        private void changeButtonAuto(int row, int col, int numStep, Button btn)
        {
            
            if (numStep % 2 == 0)
            {
                arr[row, col] = 1;
                btn.Text = "X";
                btn.BackColor = Color.Green;
                //await Task.Run(() => task());
                if (numStep > 5 && checkBoard(row, col, true, numStep) == true)
                {
                    MessageBox.Show(String.Format("Game Over : Player Win"));
                    finishAutoGame();
                    return; 
                }

            }
            else
            {
                arr[row, col] = 2;
                btn.Text = "O";
                btn.BackColor = Color.Red;
                if (numStep > 5)
                {

                    if (checkBoard(row, col, true, numStep) == true)
                    {
                        MessageBox.Show(String.Format("Game Over : System Win"));
                        finishAutoGame();
                        return;
                    }
                }
             

            }

        }
        private void finishAutoGame()
        {
            showBtn.Enabled = true;
            startBtn.Enabled = true;
            autoGame = false;
        }

        private void AutoGametask()
        {
            Thread.Sleep(3000);
            return;
        }


    }
}
