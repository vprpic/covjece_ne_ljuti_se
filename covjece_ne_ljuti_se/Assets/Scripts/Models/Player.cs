public class Player {
	
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

	public Player(string screenName, bool ready)
	{
		ScreenName = screenName;
		Ready = ready;
	}

	public override string ToString()
	{
		return this.ScreenName;
	}
}
