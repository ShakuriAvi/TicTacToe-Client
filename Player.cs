using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_Tac_Toe_Backend
{
    class Player
    {
        public Player(JToken  jObject)
        {
          Id   = (int)jObject["id"];
          Password = (int)jObject["password"];
          Name = (string)jObject["name"];
          Loses= (int)jObject["loses"];
          Draws = (int)jObject["draws"];
          Wins = (int)jObject["wins"];

        }


        public int Id { get; set; }
        public int Password { get; set; }
        public string Name { get; set; }
        //public List<Game> Games { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Loses { get; set; }

        public static implicit operator Player(string v)
        {
            throw new NotImplementedException();
        }

        public class Game
        {
            public List<(int, int)> StepsGame { get; set; }
            public DateTime Date { get; set; }

        }
    }
}
