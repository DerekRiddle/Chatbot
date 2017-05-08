using System;
using System.Windows.Forms;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Enums;

namespace ChatBot
{
    public partial class Form1 : Form
    {
        //makes the main TwitchClient object
        TwitchClient client = new TwitchClient(new TwitchLib.Models.Client.ConnectionCredentials(Properties.Settings.Default.username, Properties.Settings.Default.oauth));

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (client.IsConnected)
            {
                //puts txt box into chat box
                richChat.Text += Properties.Settings.Default.username + ": " + txtChatBox.Text + "\n";
                //sends message to twitch chat server
                client.SendMessage(txtChatBox.Text);

                txtChatBox.Clear();
            } else
            {
                richChat.Text += "Disconnected from chat!\n";
            }
            
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            client.OnMessageReceived += new EventHandler<OnMessageReceivedArgs>(globalChatMessagesRecieved);
            client.OnConnected += new EventHandler<OnConnectedArgs>(onConnected);
            client.OnDisconnected += new EventHandler<OnDisconnectedArgs>(onDisconnected);

            if (client.IsConnected == false)
            {
                client.Connect();
            }
        }

        public void onConnected(object sender, OnConnectedArgs e)
        {
            //Figure out why we need this
            CheckForIllegalCrossThreadCalls = false;

            //joins chatroom
            client.JoinChannel(txtChatRoom.Text);

            richChat.Text += "Connected to chat server!\n";
        }

        public void onDisconnected(object sender, OnDisconnectedArgs e)
        {
            //Figure out why we need this
            CheckForIllegalCrossThreadCalls = false;
            richChat.Text += "Disconnected from channel.";
        }

        private void globalChatMessagesRecieved(object sender, OnMessageReceivedArgs e)
        {
            //Figure out why we need this
            CheckForIllegalCrossThreadCalls = false;

            if (e.ChatMessage.Username == "dirkadin" & e.ChatMessage.Message.StartsWith("hello"))
            {
                //soundPlayer.Load();
                //soundPlayer.Play();
                System.Media.SystemSounds.Question.Play();

                updateChat(e);
            }
            else if (e.ChatMessage.UserType == UserType.Moderator & e.ChatMessage.Message == "!foo")
            {
                System.Media.SystemSounds.Question.Play();
                updateChat(e);
            }
            else
            {
                updateChat(e);
            }
            
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            client.LeaveChannel(txtChatRoom.Text);
            client.Disconnect();
            richChat.Text += "Disconnecting...\n";
        }

        private void updateChat(OnMessageReceivedArgs e)
        {
            richChat.AppendText(e.ChatMessage.Username + ": " + e.ChatMessage.Message + "\n");
            richChat.ScrollToCaret();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            client.LeaveChannel(txtChatRoom.Text);
            client.Disconnect();
            richChat.Text += "Disconnecting...\n";
            Close();
        }
    }
}
