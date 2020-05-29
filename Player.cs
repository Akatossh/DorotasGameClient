using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DorotasGame1
{
    public class Player
    {
        public string name;
        public List<string> connectedPlayers = new List<string>();
        public List<string> connectedReadyPlayers = new List<string>();
        public int amountOfPlayers;

        public string makePlayerStatusString(string data)
        {
            int index = data.IndexOf(";");
            string msg = null;
            int i = 0;

            string temp = null;
            bool temp2;
            while (index > 0)
            {
                temp = data.Substring(0, index);
                temp2 = false;
                foreach (var item in connectedPlayers)
                {

                    if (item.ToString() == temp)
                    {
                        temp2 = true;
                    }
                }

                if (temp2 == false)
                {
                    this.connectedPlayers.Add(data.Substring(0, index));
                    amountOfPlayers++;
                }
                temp2 = false;

                msg = msg + data.Substring(0, index) + "\r\n";
                data = data.Substring(index + 1, (data.Length - msg.Length) - 1);
                index = data.IndexOf(";");
                i++;
            }

            //msg = data.Substring(0, data.Length);
            return msg;
        }

        public string makeReadyPlayersStatusString(string data)
        {

            int index = data.IndexOf(";");
            string msg = null;
            int i = 0;

            while (index > 0)
            {
                this.connectedReadyPlayers.Add(data.Substring(0, index));
                msg = msg + "\r\n" + data.Substring(0, index) + "Ready";
                data = data.Substring(index + 1, (data.Length - msg.Length) - 1);
                index = data.IndexOf(";");
                i++;
            }

            i = 0;
            int j = 0;
            string returnMsg = null;
            int temp = 1;

            foreach (var connected in this.connectedPlayers)
            {
                temp = 1;
                foreach (var ready in this.connectedReadyPlayers)
                {
                    if (connected.ToString() == ready.ToString())
                    {
                        returnMsg = returnMsg + connected.ToString() + " Ready \r\n";
                        temp = 0;
                        break;
                    }

                    j++;
                }
                if (temp == 1)
                    returnMsg = returnMsg + connected.ToString() + " \r\n";
                i++;
            }

            //msg = data.Substring(0, data.Length);
            return returnMsg;

        }

        public string cutOfCommand(string receivedData)
        {
            int found1 = receivedData.IndexOf("|");
            if (found1 >= 0)
            {
                string command = receivedData.Substring(found1 + 1, receivedData.Length - (found1 + 1));
                int found2 = command.IndexOf("|");
                command = command.Substring(0, found2);

                return command;
            }
            else
            {
                return "noComand";
            }

        }

        public string cutOfMessage(string receivedData)
        {
            int found1 = receivedData.IndexOf("|");
            if (found1 >= 0)
            {
                string message = receivedData.Substring(found1 + 1, receivedData.Length - (found1 + 1));
                //message = message.Substring(found + 1);
                int found2 = message.IndexOf("|");
                message = message.Substring(found2 + 1, message.Length - (found2 + 1));

                return message;
            }
            else
            {
                return "";
            }
        }

        internal string cutOfName(string returndata)
        {
            int index = 0;
            index = returndata.IndexOf("|");
            if (index != 0)
            {
                string r = returndata.Substring(0, index);
                return r;
            }
            else
                return null;
        }
    }
}
