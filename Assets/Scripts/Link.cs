public class Link 
{
	public LinkType actType;
	public bool hasDisplay;
	public int caseIdle, caseAttack, caseDodge;

	public Link( LinkType type, bool hasDisplay )
	{
		this.actType = type;
		this.hasDisplay = hasDisplay;
	}
}
