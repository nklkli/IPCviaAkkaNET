using ActorLib;
using Akka.Actor;
#nullable disable

ActorSystem sys = null;
IActorRef actor = null;
try
{
    Console.CancelKeyPress += Console_CancelKeyPress;
    AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

    sys = ActorSystem.Create("AkkaIPC");
    Props props = Props.Create<AkkaActor>(() => new AkkaActor("Bob", "Alice", "localhost:8081", Console.WriteLine));
    actor = sys.ActorOf(props, "Bob");

    while (true)
    {
        string line = Console.ReadLine();
        actor.Tell(new SendChatMessage(line));
    }
   
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
finally
{
    await SayGoodByAndShutdown();
}



async void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
{    
    await SayGoodByAndShutdown();
}

async void CurrentDomain_ProcessExit(object sender, EventArgs e)
{
    await SayGoodByAndShutdown();
}

async Task SayGoodByAndShutdown()
{
    actor?.Tell(new SendGoodbyeMessage());
    await CoordinatedShutdown.Get(sys).Run(CoordinatedShutdown.ClrExitReason.Instance);
}