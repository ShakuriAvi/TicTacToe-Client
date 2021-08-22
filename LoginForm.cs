using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Tic_Tac_Toe_Backend
{
    public partial class LoginForm : Form
    {
        private static HttpClient client = new HttpClient();
        private List<Player> players = new List<Player>();
        Form1 form1 ;
        public LoginForm()
        {
            InitializeComponent();
            client.BaseAddress = new Uri("https://localhost:44368/");
            getData();
            errorLabel.Visible = false;
        }
        private async void getData()
        {
            String json = await GetJsonAsync("api/player");
            convertJsonToArrayList(json);
        }
        //api/player

        private static async Task<String> GetJsonAsync(string path)
        {
          
            String customerJsonString="";
            HttpResponseMessage response =  client.GetAsync(path).Result;
            if (response.IsSuccessStatusCode)
            {
                customerJsonString = await response.Content.ReadAsStringAsync();
            }
            return customerJsonString;
        }

        private void convertJsonToArrayList(string json)
        {
            JObject jObject = JObject.Parse(json);
            JToken jPlayers = jObject["data"];
            Array jsonPlayers = jPlayers.ToArray();
            foreach (var jsonPlayer in jsonPlayers)

            {
                players.Add(new Player((JToken)jsonPlayer));
            }

        }
        private void loginButton_Click(object sender, EventArgs e)
        {
            for( int i = 0;i<players.Count;i++)
            {
                if (players[i].Name == userNameTextBox.Text && players[i].Password == int.Parse(passwordTextBox.Text))
                {
                    form1 = new Form1(players,i);
                    this.Hide();
                    form1.ShowDialog();
                    
                }
            }
            errorLabel.Visible = true;

        }

    
    }
}
