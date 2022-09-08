using ActorLib;
using Akka.Actor;
using Akka.Event;

namespace Alice;

public partial class Form1 : Form
{
    private ActorSystem _sys;
    /// <summary>
    /// Bridge actor
    /// </summary>
    private IActorRef _actor;
    private readonly string NL = Environment.NewLine;

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        _sys = ActorSystem.Create("AkkaIPC");
        Props props = Props.Create<AkkaActor>(() => new AkkaActor("Alice", "Bob", "localhost:8080", UpdateGUI));
        _actor = _sys.ActorOf(props, "Alice");
    }


    void UpdateGUI(object message)
    {
        Invoke(() =>
        {
            if (message is OnPartnerDisconnected)
            {
                button_Send.Enabled = false;
            }
            else if (message is OnPartnerConnected)
            {
                button_Send.Enabled = true;
            }
            textBox_Messages.AppendText(message.ToString());
            textBox_Messages.AppendText(NL);
        });
    }


    private void button_Send_Click(object sender, EventArgs e)
    {
        string msgToSend = textBox1.Text;
        _actor.Tell(new SendChatMessage(msgToSend));
        textBox1.Text = "";
        UpdateGUI($"Sending: " + msgToSend);
        textBox1.Focus();
    }


    private void Form1_Shown(object sender, EventArgs e)
    {
        textBox1.Focus();
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        _actor?.Tell(new SendGoodbyeMessage());
        await CoordinatedShutdown.Get(_sys).Run(CoordinatedShutdown.ClrExitReason.Instance);
    }
}
