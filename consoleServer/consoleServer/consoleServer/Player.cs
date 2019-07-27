public class Player
{
	public string ScreenName { get; set; }
	public bool Ready { get; set; }

	public Player()
	{

	}

	public Player(string screenName)
	{
		ScreenName = screenName;
		Ready = false;
	}

	public override string ToString()
	{
		return this.ScreenName;
	}
}
