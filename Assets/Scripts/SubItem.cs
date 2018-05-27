using UnityEngine;
using UnityEngine.UI;

public enum SubItemType
{
	CaseIdle,
	CaseAttack,
	CaseDodge
}

public class SubItem : MonoBehaviour
{
    private int value;
    private Text label;
    public Button button;
    private Image img;
	public Link link;
	public SubItemType type;

    private void Awake()
    {
        label = transform.GetChild(0).GetComponent<Text>();
        button = GetComponent<Button>();
        img = GetComponent<Image>();
    }

    public void Increase()
    {
        value++;
        label.text = "" + value;

		switch(type)
		{
			case SubItemType.CaseAttack:
				link.caseAttack++;
				break;
			case SubItemType.CaseDodge:
				link.caseDodge++;
				break;
			case SubItemType.CaseIdle:
				link.caseIdle++;
				break;
		}

    }

    public void Decrease()
    {
        value--;
        label.text = "" + value;

		switch(type)
		{
			case SubItemType.CaseAttack:
				link.caseAttack--;
				break;
			case SubItemType.CaseDodge:
				link.caseDodge--;
				break;
			case SubItemType.CaseIdle:
				link.caseIdle--;
				break;
		}
    }

    public void Highlight()
    {
        img.color = Color.green;
    }

    public void Dark()
    {
        img.color = Color.white;
    }
        

}
