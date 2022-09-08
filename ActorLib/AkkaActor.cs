using Akka.Actor;

namespace ActorLib;

public class AkkaActor : ReceiveActor, IWithUnboundedStash
{
    private string _myName;
    private string _partnerName;
    private readonly string _partnerAddress;
    private readonly Action<object> _callback;
    ActorSelection _partner;

    public IStash Stash { get; set; }

    public AkkaActor(
        string myName,
        string partnerName,
        string partnerAddress,
        Action<object> callback = null)
    {
        _myName = myName;
        _partnerName = partnerName;
        _partnerAddress = partnerAddress;
        _callback = callback;
        _partner = Context.ActorSelection($"akka.tcp://AkkaIPC@{_partnerAddress}/user/{_partnerName}");
        _partner.Tell(new ConnectMessage(_myName));

        AcceptConnections();
    }


    void AcceptConnections()
    {
        CallCallback("Waiting for partner connection...");

        Receive<ConnectMessage>(msg =>
        {
            _partnerName = msg.MyName;
            Context.Watch(Sender);
            Become(ObserveConnection);
            Sender.Tell(new ConnectMessage(_myName));
            CallCallback(new OnPartnerConnected(msg.MyName));
        });

        Receive<ChatMessage>(_ => Stash.Stash());
    }





    void ObserveConnection()
    {
        Stash.UnstashAll();

        Receive<SendChatMessage>(msg =>
        {
            _partner.Tell(new ChatMessage(msg.Message));
        });

        Receive<SendGoodbyeMessage>(msg =>
        {
            _partner.Tell(new GoodbyeMessage(_myName));
        });

        Receive<GoodbyeMessage>(msg =>
        {
            Context.Unwatch(Sender);
            Become(AcceptConnections);
            CallCallback(new OnPartnerDisconnected(msg.MyName));
            _partnerName = null;
        });

        Receive<ChatMessage>(chatMsg =>
        {
            CallCallback(chatMsg);
        });

        Receive<Terminated>(msg =>
        {          
            Context.Unwatch(Sender);
            Become(AcceptConnections);
            CallCallback(new OnPartnerDisconnected(_partnerName));
            _partnerName = null;
        });
    }


    void CallCallback(object msg)
    {
      _callback?.Invoke(msg);
    }
}
