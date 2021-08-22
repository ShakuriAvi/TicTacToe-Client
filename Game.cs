using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_Tac_Toe_Backend
{
    public class Game
    {
    //   public Game(JToken jObject)
     //   {
      //      PlayerId = (int)jObject["PlayerId"];
      //      GameSteps = jObject["GameSteps"].ToList<(int, int)>();
      //      Date = (DateTime)jObject["Date"];
       //     Id = (int)jObject["Id"];


       // }
        public int Id { get; set; }
        
        public int PlayerId { get; set; }
        public List<(int, int)> GameSteps { get; set; }
        public DateTime Date { get; set; }
       public string Result { get; set; }
    }
}
