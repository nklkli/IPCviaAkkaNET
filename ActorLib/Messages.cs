using Akka.Actor;

namespace ActorLib;


/// <summary>
/// App -> Actor
/// </summary>
/// <param name="Message"></param>
public readonly record struct SendChatMessage(string Message);


/// <summary>
/// Actor <-> Actor
/// </summary>
/// <param name="MyName"></param>
readonly record struct ConnectMessage(string MyName);


/// <summary>
/// Actor <-> Actor
/// </summary>
/// <param name="Message"></param>
public readonly record struct ChatMessage(string Message);

/// <summary>
/// actor <-> actor
/// </summary>
public  record  GoodbyeMessage
{
	public string MyName { get; init; }
	private GoodbyeMessage() { }
    public GoodbyeMessage(string MyName)
	{
		this.MyName = MyName;
	}
}
/// <summary>
/// app -> local actor
/// </summary>
public readonly record struct SendGoodbyeMessage();


/// <summary>
/// Actor -> App
/// </summary>
/// <param name="PartnerName"></param>
public readonly record struct OnPartnerConnected(string PartnerName);

/// <summary>
/// Actor -> App
/// </summary>
/// <param name="PartnerName"></param>
public readonly record struct OnPartnerDisconnected(string PartnerName);