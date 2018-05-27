using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    public GameObject lThink, lWatch, lAttack, lDodge;
    private ArrayList gtDisplayList;
    private SubItem currentSelected;
    public Transform content;

    private void Awake()
    {
        gtDisplayList = new ArrayList();
		Session.actionList = new ArrayList();
		HistoryManager.instance.OnRedo += CheckActionAndExec;
		HistoryManager.instance.OnUndo += CheckActionAndExec;
    }

	void CheckActionAndExec( ActionType type )
	{
		switch( type )
		{
		case ActionType.AddAttack:
			AddAttack(true);
			break;
		case ActionType.AddDodge:
			AddDodge(true);
			break;
		case ActionType.AddThink:
			AddThink(true);
			break;
		case ActionType.AddWatch:
			AddWatch(true);
			break;
		case ActionType.RemoveAttack: 
			RemoveLink(false, Session.actionList.Count-1);
			break;
		case ActionType.RemoveDodge:
			RemoveLink(false, Session.actionList.Count-1);
			break;
		case ActionType.RemoveThink:  
			RemoveLink(true, Session.actionList.Count-1);
			break;
		case ActionType.RemoveWatch:
			RemoveLink(true, Session.actionList.Count-1);
			break;
		}
	}

    void Highlight( SubItem item )
    {
        currentSelected = item;
        foreach ( SubItem i in gtDisplayList )
            i.Dark();
        item.Highlight();
    }

	public void AddSubItem( Transform actBtn )
	{
		for ( int i = 0; i < 3; i++ )
		{
			GameObject go = actBtn.GetChild(i).gameObject;
			go.SetActive(true);
			Button btn = go.GetComponent<Button>();
			SubItem item = go.GetComponent<SubItem>();
			gtDisplayList.Add(item);
			btn.onClick.AddListener(() => Highlight(item));            
		}  
	}

	public void RemoveLink( bool hasDisplay, int index )
	{
		//index = Session.actionList.Count - 1;
		Destroy( content.GetChild(index).gameObject );
		content.GetComponent<RectTransform>().sizeDelta -= new Vector2(200, 0);

		if ( hasDisplay )
		{
			gtDisplayList.Remove( index );
			gtDisplayList.Remove( index - 1);
			gtDisplayList.Remove( index - 2);
		}

		Session.actionList.RemoveAt(index);
	}

	public void AddWatch( bool fromHistory )
    {
        GameObject act = Instantiate(lWatch, content);
        content.GetComponent<RectTransform>().sizeDelta += new Vector2(200, 0);
        Transform actBtn = act.transform.GetChild(1);
        actBtn.gameObject.SetActive(true);
		AddSubItem(actBtn);  

		Link newLink = new Link(LinkType.Watch, true);
		Session.actionList.Add(newLink);

		if( !fromHistory )
		{
			HistoryAction ha = new HistoryAction( ActionType.AddWatch, ActionType.RemoveWatch ); 
			HistoryManager.instance.AddHistory(ha);
		}
    }

	public void AddThink( bool fromHistory )
    {
		GameObject act = Instantiate(lThink, content);
		content.GetComponent<RectTransform>().sizeDelta += new Vector2(200, 0);
		Transform actBtn = act.transform.GetChild(1);
		actBtn.gameObject.SetActive(true);
		AddSubItem(actBtn);  

		Link newLink = new Link(LinkType.Think, true);
		Session.actionList.Add(newLink);

		if (!fromHistory)
		{
			HistoryAction ha = new HistoryAction( ActionType.AddThink, ActionType.RemoveThink ); 
			HistoryManager.instance.AddHistory(ha);
		}
    }

	public void AddAttack(bool fromHistory)
    {
		Instantiate(lAttack, content);
		content.GetComponent<RectTransform>().sizeDelta += new Vector2(200, 0);

		Link newLink = new Link(LinkType.Attack, true);
		Session.actionList.Add(newLink);

		if (!fromHistory)
		{
			HistoryAction ha = new HistoryAction( ActionType.AddAttack, ActionType.RemoveAttack ); 
			HistoryManager.instance.AddHistory(ha);
		}
    }

	public void AddDodge(bool fromHistory)
    {
		Instantiate(lDodge, content);
		content.GetComponent<RectTransform>().sizeDelta += new Vector2(200, 0);

		Link newLink = new Link(LinkType.Dodge, true);
		Session.actionList.Add(newLink);

		if (!fromHistory)
		{
			HistoryAction ha = new HistoryAction( ActionType.AddDodge, ActionType.RemoveDodge ); 
			HistoryManager.instance.AddHistory(ha);
		}
    }

    public void PlusClick()
    {
        currentSelected.Increase();
    }

    public void MinusSign()
    {
        currentSelected.Decrease();
    }

}
