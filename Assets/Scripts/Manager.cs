using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    public GameObject lThink, lWatch, lAttack, lDodge, lEmpty;
    private ArrayList gtDisplayList;
    private SubItem currentSelected;
    public Transform content;

    public static Manager instance;

    private void Awake()
    {
        gtDisplayList = new ArrayList();
		Session.actionList = LoadFromDatabase();
		BuildUIFromDatabase();
		HistoryManager.instance.OnRedo += CheckActionAndExec;
		HistoryManager.instance.OnUndo += CheckActionAndExec;

        instance = this;
    }

	public ArrayList LoadFromDatabase()
	{
		ArrayList list = new ArrayList();
		foreach( Link l in database.linkList )
			list.Add(l);
		return list;
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
        case ActionType.SelectSubItem:
            Highlight(HistoryManager.instance.GetCurrent().subItem, true);
            break;
        case ActionType.SelectBackItem:
            Highlight(HistoryManager.instance.GetCurrent().backSub, true);
            break;
        case ActionType.DeselectSubItem:
            Highlight(null, true);
            break;
        case ActionType.IncreaseValue:
            PlusClick(true);
            break;
        case ActionType.DecreaseValue:
            MinusSign(true);
            break;
        case ActionType.MoveOn:
            MoveLink(HistoryManager.instance.GetCurrent().index, HistoryManager.instance.GetCurrent().backIndex, true);                
            break;
        case ActionType.MoveBack:
            MoveLink(HistoryManager.instance.GetCurrent().backIndex, HistoryManager.instance.GetCurrent().index, true);
			break;
        }
	}


	void MoveLink( int current, int target, bool fromHistory )
	{        
		Transform curTrns = content.GetChild(current);
		Transform tgtTrns = content.GetChild(target);

		curTrns.SetSiblingIndex(target);
		tgtTrns.SetSiblingIndex(current);
	}

    public void Highlight( SubItem item, bool fromHistory )
    {
        if ( !fromHistory )
        {
            if ( currentSelected == null )
            {
                HistoryAction ha = new HistoryAction(ActionType.SelectSubItem, ActionType.DeselectSubItem, item, null);
                HistoryManager.instance.AddHistory(ha);
            }
            else
            {
                HistoryAction ha = new HistoryAction(ActionType.SelectSubItem, ActionType.SelectBackItem, item, currentSelected);
                HistoryManager.instance.AddHistory(ha);
            }
        }
        
        currentSelected = item;
        foreach ( SubItem i in gtDisplayList )
            i.Dark();

        if ( item != null )
            item.Highlight();
    }

	public void AddSubItem( Transform actBtn, Link link )
	{
		for ( int i = 0; i < 3; i++ )
		{
			GameObject go = actBtn.GetChild(i).gameObject;
			go.SetActive(true);
			Button btn = go.GetComponent<Button>();            
			SubItem item = go.GetComponent<SubItem>();
            item.index = gtDisplayList.Count;
			gtDisplayList.Add(item);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => Highlight(item, false));

            go.GetComponent<SubItem>().link = link;
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
			gtDisplayList.Remove( index -1);
			gtDisplayList.Remove( index -2);        
		}

		Session.actionList.RemoveAt(index);
	}

    public void BuildUIFromDatabase()
    {
		for ( int i = 0; i < Session.actionList.Count; i++ )
		{
			Link link = ( Link )Session.actionList[ i ];
			switch ( link.type )
			{
			case LinkType.Attack:
				GameObject attack = Instantiate(lAttack, content);   
				attack.AddComponent<LinkDrag>();
				content.GetComponent<RectTransform>().sizeDelta += new Vector2(200, 0);
				break;
			case LinkType.Dodge:
				GameObject dodge = Instantiate(lDodge, content);     
				dodge.AddComponent<LinkDrag>();
				content.GetComponent<RectTransform>().sizeDelta += new Vector2(200, 0);
				break;
			case LinkType.Think:
				GameObject think = Instantiate(lThink, content);   
				think.AddComponent<LinkDrag>();
				content.GetComponent<RectTransform>().sizeDelta += new Vector2(200, 0);
				think.transform.GetChild(1).gameObject.SetActive(true);
				think.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "" + link.caseIdle;
				think.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = "" + link.caseAttack;
				think.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<Text>().text = "" + link.caseDodge;
				AddSubItem(think.transform.GetChild(1), link);
				break;
			case LinkType.Watch:
				GameObject watch = Instantiate(lWatch, content);  
				watch.AddComponent<LinkDrag>();
				content.GetComponent<RectTransform>().sizeDelta += new Vector2(200, 0);
				watch.transform.GetChild(1).gameObject.SetActive(true);
				watch.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "" + link.caseIdle;
				watch.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = "" + link.caseAttack;
				watch.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<Text>().text = "" + link.caseDodge;
				AddSubItem(watch.transform.GetChild(1), link);
				break;
			case LinkType.Empty:
				GameObject empty = Instantiate(lEmpty, content);
				break;
			}
		}
    }

    public void AddWatch(bool fromHistory)
    {
        GameObject act = Instantiate(lWatch, content);
        act.AddComponent<LinkDrag>();
        Button btn = act.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        int count = Session.actionList.Count;        
        content.GetComponent<RectTransform>().sizeDelta += new Vector2(200, 0);
        Transform actBtn = act.transform.GetChild(1);
        actBtn.gameObject.SetActive(true);

        Link newLink = new Link(LinkType.Watch, true);
        Session.actionList.Add(newLink);

        AddSubItem(actBtn, newLink);

        if ( !fromHistory )
        {
            HistoryAction ha = new HistoryAction(ActionType.AddWatch, ActionType.RemoveWatch);
            HistoryManager.instance.AddHistory(ha);
        }
        
    }

    public void AddThink( bool fromHistory )
    {
		GameObject act = Instantiate(lThink, content);
        act.AddComponent<LinkDrag>();
        Button btn = act.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        int count = Session.actionList.Count;

        content.GetComponent<RectTransform>().sizeDelta += new Vector2(200, 0);
		Transform actBtn = act.transform.GetChild(1);
		actBtn.gameObject.SetActive(true);		

		Link newLink = new Link(LinkType.Think, true);
		Session.actionList.Add(newLink);

        AddSubItem(actBtn, newLink);

        if (!fromHistory)
		{
			HistoryAction ha = new HistoryAction( ActionType.AddThink, ActionType.RemoveThink ); 
			HistoryManager.instance.AddHistory(ha);
        }
    }

	public void AddAttack(bool fromHistory)
    {
        GameObject act = Instantiate(lAttack, content);
        act.AddComponent<LinkDrag>();
		content.GetComponent<RectTransform>().sizeDelta += new Vector2(200, 0);
        Button btn = act.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        int count = Session.actionList.Count;

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
        GameObject act = Instantiate(lDodge, content);
        act.AddComponent<LinkDrag>();
        content.GetComponent<RectTransform>().sizeDelta += new Vector2(200, 0);
        Button btn = act.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        int count = Session.actionList.Count;

        Link newLink = new Link(LinkType.Dodge, true);
		Session.actionList.Add(newLink);

		if (!fromHistory)
		{
			HistoryAction ha = new HistoryAction( ActionType.AddDodge, ActionType.RemoveDodge ); 
			HistoryManager.instance.AddHistory(ha);
        }
    }

    public void PlusClick( bool fromHistory )
    {
        currentSelected.Increase();
        if (!fromHistory)
        {
            HistoryAction ha = new HistoryAction(ActionType.IncreaseValue, ActionType.DecreaseValue);
            HistoryManager.instance.AddHistory(ha);
        }
    }

    public void MinusSign( bool fromHistory )
    {
        currentSelected.Decrease();
        if ( !fromHistory )
        {
            HistoryAction ha = new HistoryAction(ActionType.DecreaseValue, ActionType.IncreaseValue);
            HistoryManager.instance.AddHistory(ha);
        }
    }

	public FightScriptable database;
	public void WriteToDatabase()
	{
		database.linkList = new Link[Session.actionList.Count];
		for ( int i = 0; i < Session.actionList.Count; i++ )
			database.linkList[i] = (Link)Session.actionList[i];
	}

	public void FightClick()
	{
		WriteToDatabase();
	}
}
