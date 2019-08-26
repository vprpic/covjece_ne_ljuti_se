using Db4objects.Db4o;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using System;

public class Player : IActivatable {

	[Transient]
	private IActivator _activator;
	public string ScreenName { get; set; }
	public int Order { get; set; } //0-3 shows which player goes first
	public bool Ready { get; set; }

	public Player()
	{

	}

	public Player(string screenName, int order)
	{
		Order = order;
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
		Activate(ActivationPurpose.Read);
		return this.ScreenName+" "+Order;
	}

	public void Activate(ActivationPurpose purpose)
	{
		if (_activator != null)
		{
			_activator.Activate(purpose);
		}
	}
	public void Bind(IActivator activator)
	{
		if (_activator == activator)
		{
			return;
		}
		if (activator != null && null != _activator)
		{
			throw new System.InvalidOperationException();
		}
		_activator = activator;
	}
}
