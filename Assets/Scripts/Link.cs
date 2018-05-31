[System.Serializable]
public class Link 
{    
    public LinkType type;
	public bool hasDisplay;
	public int caseIdle, caseAttack, caseDodge;
    public bool selected;    

	public Link( LinkType type, bool hasDisplay )
	{
		this.type = type;
		this.hasDisplay = hasDisplay;
	}
}
