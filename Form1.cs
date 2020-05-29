using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Collections;


namespace DorotasGame1
{
    public partial class Form1 : Form
    {
        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        public List<Button> buttons = new List<Button>();
        String readdata = null;
        String statusPlayerString = null;
        Player player = new Player();

        public bool reRollAvailble = true;
        string points = null;

        public bool questionsSent = false;
        public bool answesSent = false;

        public bool disableButtons = false;

        string receivedQuestion = null;
        string receivedAnswer = null;
        string ReRolledPlayerName = null;


        public Form1()
        {
            InitializeComponent();
            textBox6.Text = "pytanie 1";
            textBox7.Text = "pytanie 2";
            textBox8.Text = "pytanie 3";
            textBox9.Text = "pytanie 4";

            textBox10.Text = "slowo 1";
            textBox11.Text = "slowo 2";
            textBox12.Text = "slowo 3";
            textBox13.Text = "slowo 4";
            textBox14.Text = "slowo 5";
            textBox15.Text = "slowo 6";
            textBox16.Text = "slowo 7";
            textBox17.Text = "slowo 8";
            textBox18.Text = "slowo 9";
            textBox19.Text = "slowo 10";
            textBox20.Text = "slowo 11";
            textBox21.Text = "pytanie 12";

            textBox1.Text = "192.168.0.164";
            textBox2.Text = "8003";
            textBox23.Visible = false;
            button6.Visible = false;

            button2.Enabled = false;
            button3.Visible = false;
            button4.Enabled = false;
            button5.Enabled = false;
        }

        private void DynamicButton_Click(object sender, EventArgs e)
        {
            if(disableButtons == false)
            {
                int index = sender.ToString().IndexOf(":");
                string temp = sender.ToString().Substring(index + 2, sender.ToString().Length - (index + 2));
                sendMessage(player.name + "|ChosenPlayer|" + temp + ";");
                Thread.Sleep(500);
                disableButtons = true;
            }

        }

        public void button1_Click(object sender, EventArgs e)
        {
            if(textBox5.Text == null || textBox5.Text == "")
            {
                MessageBox.Show("Nazwa gracza nie moze zostac pusta");
            }
            else
            {
                //192.168.0.164
                //92.239.151.48
                //clientSocket.Connect(textBox1.Text, Int32.Parse(textBox2.Text));
                string ip = textBox1.Text.ToString();
                int port = Int32.Parse(textBox2.Text.ToString());
                try
                {
                    clientSocket.Connect(ip, port);

                    textBox5.Enabled = false;
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;

                    button1.Enabled = false;
                    button2.Enabled = true;
                    button4.Enabled = true;
                    button5.Enabled = true;

                    Thread ctThread = new Thread(getMessage);
                    ctThread.Start();
                }
                catch (Exception)
                {
                    MessageBox.Show("Server nie odowiada. Sprawdź połącznie z internem, adres IP port oraz zapas mydła w lodówce.");
                    //throw;
                }

            }

        }

        public void getMessage()
        {
            String returndata;
            bool init = true;
            
            while (true)
            {
                try
                {
                    serverStream = clientSocket.GetStream();
                    var buffsize = clientSocket.ReceiveBufferSize;

                    if (init == true)
                    {
                        init = false;
                        player.name = textBox5.Text;
                        string msg = player.name + "|" + "SetName" + "|" + textBox3.Text;
                        byte[] outsream = Encoding.ASCII.GetBytes(msg);
                        serverStream.Write(outsream, 0, outsream.Length);
                        serverStream.Flush();
                    }

                    byte[] instream = new byte[buffsize];
                    serverStream.Read(instream, 0, buffsize);

                    returndata = System.Text.Encoding.ASCII.GetString(instream);
                    readdata = returndata;
                    string command = player.cutOfCommand(returndata);
                    string message = player.cutOfMessage(returndata);
                    string name = player.cutOfName(returndata);

                    if (command == "ConnectedPlayers")
                    {
                        setPlayersStatus(message);
                    }
                    else if (command == "ReadyPlayers")
                    {
                        setReadyPlayersStatus(message);
                    }
                    else if (command == "MainGameStart")
                    {
                        setMainGame();
                    }
                    else if (command == "Question")
                    {
                        receivedQuestion = message;
                        setRceivedQuestion();
                    }
                    else if (command == "Answer")
                    {
                        receivedAnswer = message;
                        setRecivedAnswer();
                    }
                    else if (command == "ResetLayouts")
                    {
                        ResetLayouts();
                    }
                    else if (command == "Points")
                    {
                        points = message;
                        ShowPlayersPoints();
                    }
                    else if (command == "EndGame")
                    {
                        EndGame();
                    }
                    else if (command == "ReRollBack")
                    {
                        ReRolledPlayerName = name;
                        ReRollBack(message);
                    }
                    else if (command == "Chat")
                    {
                        readdata = message;
                        msg();
                    }

                    //readdata = message;
                    //if(command=="Chat")
                }
                catch (Exception)
                {
                    MessageBox.Show("Server został zamknięty, przeproś :C");
                    break;
                }
                
            }
        }

        private void ShowPlayersPoints()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(ShowPlayersPoints));
            }
            else
            {
                textBox22.Text = points;
            }
        
        }

        public void EndGame()
        {
            MessageBox.Show("Koniec pytań, gra skończona");
        }

        private void ResetLayouts()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(ResetLayouts));
            }
            else
            {
                if(buttons.Count !=0)
                for(int i=0; i!= buttons.Count; i++)
                {
                    this.Controls.Remove(buttons[i]);
                }
            }
        }

        private void ReRollBack(string msg2)
        {
            if(ReRolledPlayerName == player.name)
            {
                receivedAnswer = msg2;
                setRecivedAnswer();
            }

        }

        private void setRecivedAnswer()
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(setRecivedAnswer));
            }
            else
            {
                button6.Visible = true;
                textBox23.Text = receivedAnswer;
            }
            
        }

        private void setRceivedQuestion()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(setRceivedQuestion));
            }
            else
            {
                reRollAvailble = true;
                button6.Visible = false;
                disableButtons = false;
                textBox23.Text = receivedQuestion;
                //textBox23.Text = "player name: " + player.name+ "connected 0 player" + player.connectedPlayers[0].ToString();
                
                for (int i = 0; i != player.amountOfPlayers; i++)
                {
                    Button dynamicButton = new Button();
                    buttons.Add(dynamicButton);
                }
                
                // Set Button properties
                int nextButt = 0;
                int moveButt = 0;
                foreach (var item in buttons)
                {
                    if (player.connectedPlayers[nextButt].ToString() != player.name)
                    {

                        item.Location = new Point(40 + (150 * moveButt), 350);
                        item.Height = 40;
                        item.Width = 150;
                        item.Text = player.connectedPlayers[nextButt].ToString();
                        item.BackColor = Color.White;

                        // Add a Button Click Event handler
                        item.Click += new EventHandler(DynamicButton_Click);

                        //if(!(player.name.Equals(player.connectedPlayers[nextButt])))
                        Controls.Add(item);

                        moveButt++;
                    }
                    //TU JEST ZLE, PRZYCISKI POWINNY BYC KASOWANE Z LISTY W INNYM MIEJSCU
                    if (nextButt + 1 == player.amountOfPlayers)
                        break;

                    nextButt++;
                }
                nextButt = 0;
            }
            
        }

        public void setMainGame()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(setMainGame));
            }
            else
            {
                textBox23.Visible = true;
                textBox23.Top = 170;
                button6.Top = 170;
                groupBox1.Visible = false;
                groupBox2.Visible = false;
            }
        }

        private void setReadyPlayersStatus(string message)
        {
            statusPlayerString = player.makeReadyPlayersStatusString(message);
            statusPlayers();
        }

        public void setPlayersStatus(string data)
        {
            statusPlayerString = player.makePlayerStatusString(data);
            statusPlayers();
             
        }

        public void statusPlayers()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(statusPlayers));
            }
            else
            {
                textBox22.Text = statusPlayerString;
            }
        }

        public void msg()
        {

            if(this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(msg));
            }
            else
            {
                textBox4.Text = textBox4.Text + "\r\n" + readdata;
                textBox4.SelectionStart = textBox4.Text.Length;
                textBox4.ScrollToCaret();
            }
        }

        public void button2_Click(object sender, EventArgs e)
        {

            //if (setPlayerNAme == true)
            //    sendPlayerName();

            string message = player.name + "|" + "Chat" + "|" + textBox3.Text;
            byte[] outsream = Encoding.ASCII.GetBytes(message);
            serverStream.Write(outsream, 0, outsream.Length);
            serverStream.Flush();
        }

        //public void sendPlayerName()
        //{
        //    player.name = textBox5.Text;
        //    String makeComand = player.name + "|"+ "SetName" + "|" + player.name;
        //    byte[] outsream = Encoding.ASCII.GetBytes(makeComand);
        //    serverStream.Write(outsream, 0, outsream.Length);
        //    serverStream.Flush();
        //    setPlayerNAme = false;
        //}

            public void sendReadyCommend()
        {
            button3.Enabled = false;
            string msg = player.name + "|" + "Ready" + "|" + "player";
            sendMessage(msg);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //button3.Enabled = false;
            //string msg= player.name + "|" + "Ready" + "|" + "player";
            //sendMessage(msg);
        }

        public void sendMessage(string msg)
        {
            string message = msg;
            byte[] outsream = Encoding.ASCII.GetBytes(message);
            serverStream.Write(outsream, 0, outsream.Length);
            serverStream.Flush();
        }

        private void button4_Click(object sender, EventArgs e)
        {
        
            if(textBox6.Text=="" || textBox7.Text == "" || textBox8.Text == ""  || textBox9.Text == "")
            {
                MessageBox.Show("Żadne pole nie moze zostać puste");
            }else
            {
                button4.Enabled = false;
                String pytanie1 = player.name + "|question|" + textBox6.Text + ";" + textBox7.Text + ";" + textBox8.Text + ";" + textBox9.Text + ";";
                sendMessage(pytanie1);
                questionsSent = true;
                if (answesSent == true)
                {
                    sendReadyCommend();
                }
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.Enabled = false;
            String slowo = player.name + "|answer|" + textBox10.Text + ";" + textBox11.Text + ";" + textBox12.Text + ";" + textBox13.Text + ";" +
                textBox14.Text + ";" + textBox15.Text + ";" + textBox16.Text + ";" + textBox17.Text + ";" + textBox18.Text + ";" + textBox19.Text + ";" +
                textBox20.Text + ";" + textBox21.Text + ";";
                sendMessage(slowo);

            answesSent = true;
            if (questionsSent == true)
            {
                sendReadyCommend();
            }
        }



        private void button6_Click(object sender, EventArgs e)
        {
            if(reRollAvailble==true)
            {
                string message = player.name + "|" + "ReRoll" + "|" + textBox23.Text;
                byte[] outsream = Encoding.ASCII.GetBytes(message);
                serverStream.Write(outsream, 0, outsream.Length);
                serverStream.Flush();

                reRollAvailble = false;
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("wylaczam to.");
            Thread.Sleep(1000);
            Application.Exit();
            System.Windows.Forms.Application.Exit();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
