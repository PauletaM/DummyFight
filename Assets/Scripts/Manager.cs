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
		Session.actionList = new ArrayList();
		HistoryManager.instance.OnRedo += CheckActionAndExec;
		HistoryManager.instance.OnUndo += CheckActionAndExec;

        instance = this;
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
        case ActionType.SelectLink:
            SelectLinkItem(HistoryManager.instance.GetCurrent().index, true);
            break;
        case ActionType.BackSelect:
            SelectLinkItem(HistoryManager.instance.GetCurrent().backIndex, true);
            break;
        }
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

        SelectLinkItem(Session.actionList.Count - 1, true);
	}

    private void SelectLinkItem(int index, bool fromHistory)
    {
        if ( index < 0 )
            return;

        for ( int i = 0; i < content.childCount; i++ )        
            content.GetChild(i).GetComponent<Image>().color = Color.white;
        content.GetChild(index).GetComponent<Image>().color = Color.blue;

        for ( int i = 0; i < Session.actionList.Count; i++ )
        {
            Link l = ( Link )Session.actionList[ index ];
            l.selected = false;
        }

        Link link = (Link)Session.actionList[ index ];
        link.selected = true;        
        
        if ( !fromHistory )
        {
            HistoryAction ha = new HistoryAction(ActionType.SelectLink, ActionType.BackSelect, index, Session.currentSelected);
            HistoryManager.instance.AddHistory(ha);
        }

        Session.currentSelected = index;

    }
    
    IEnumerator RemoveChilds()
    {
        for ( int i = 0; i < content.childCount; i++ )
        {
            GameObject go = content.GetChild(0).gameObject;
            go.transform.SetParent(null);
            Destroy(go);
            yield return new WaitForEndOfFrame();
        }

        for ( int i = 0; i < Session.actionList.Count; i++ )
        {
            Link link = ( Link )Session.actionList[ i ];
            switch ( link.type )
            {
                case LinkType.Attack:
                    GameObject attack = Instantiate(lAttack, content);                    
                    Button btna = attack.GetComponent<Button>();
                    btna.onClick.RemoveAllListeners();
                    int indexa = i;
                    btna.onClick.AddListener(() => SelectLinkItem(indexa, false));
                    break;
                case LinkType.Dodge:
                    GameObject dodge = Instantiate(lDodge, content);                    
                    Button btnd = dodge.GetComponent<Button>();
                    btnd.onClick.RemoveAllListeners();
                    int indexd = i;
                    btnd.onClick.AddListener(() => SelectLinkItem(indexd, false));
                    break;
                case LinkType.Think:
                    GameObject think = Instantiate(lThink, content);                    
                    Button btn = think.GetComponent<Button>();
                    btn.onClick.RemoveAllListeners();
                    int index = i;
                    btn.onClick.AddListener(() => SelectLinkItem(index, false));
                    think.transform.GetChild(1).gameObject.SetActive(true);
                    think.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "" + link.caseIdle;
                    think.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = "" + link.caseAttack;
                    think.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<Text>().text = "" + link.caseDodge;
                    AddSubItem(think.transform.GetChild(1), link);
                    break;
                case LinkType.Watch:
                    GameObject watch = Instantiate(lWatch, content);                    
                    Button btnw = watch.GetComponent<Button>();
                    btnw.onClick.RemoveAllListeners();
                    int indexw = i;
                    btnw.onClick.AddListener(() => SelectLinkItem(indexw, false));
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

        content.GetChild(Session.currentSelected).GetComponent<Image>().color = Color.blue;
    }

    public void RefreshUI()
    {
        gtDisplayList.Clear();
        StartCoroutine(RemoveChilds());        
    }

    void MoveLink( int current, int target, bool fromHistory )
    {        
        if ( !fromHistory )
        {
            HistoryAction ha = new HistoryAction(ActionType.MoveOn, ActionType.MoveBack, current, target);
            HistoryManager.instance.AddHistory(ha);
        }

        Link pivot = ( Link )Session.actionList[ current ];
        Session.actionList[ current ] = Session.actionList[ target ];
        Session.actionList[ target ] = pivot;

        Session.currentSelected = target;
        RefreshUI();
    }

    public void MoveLeft()
    {
        if ( Session.currentSelected - 1 == -1 )
        {
            Mathf.Clamp(Session.currentSelected, 0, Session.actionList.Count - 1);
            return;
        }
        MoveLink( Session.currentSelected, Session.currentSelected - 1, false );
        
    }    

    public void MoveRight()
    {
        if ( Session.currentSelected + 1 == Session.actionList.Count )
        {
            Mathf.Clamp(Session.currentSelected, 0, Session.actionList.Count - 1);
            return;
        }

        MoveLink(Session.currentSelected, Session.currentSelected + 1, false);
        
        
    }

    public void AddWatch(bool fromHistory)
    {
        GameObject act = Instantiate(lWatch, content);
        act.AddComponent<LinkDrag>();
        Button btn = act.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        int count = Session.actionList.Count;        
        btn.onClick.AddListener(() => SelectLinkItem(count, false));        
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
            SelectLinkItem(count, true);
        }
        else
        {
            SelectLinkItem(count, fromHistory);
        }

        
    }

    public void AddThink( bool fromHistory )
    {
		GameObject act = Instantiate(lThink, content);
        act.AddComponent<LinkDrag>();
        Button btn = act.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        int count = Session.actionList.Count;
        btn.onClick.AddListener(() => SelectLinkItem(count, false));

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
            SelectLinkItem(count, true);
        }
        else
        {
            SelectLinkItem(count, fromHistory);
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
        btn.onClick.AddListener(() => SelectLinkItem(count, false));

        Link newLink = new Link(LinkType.Attack, true);
		Session.actionList.Add(newLink);        

        if (!fromHistory)
		{
			HistoryAction ha = new HistoryAction( ActionType.AddAttack, ActionType.RemoveAttack );
            HistoryManager.instance.AddHistory(ha);
            SelectLinkItem(count, true);
        }
        else
        {
            SelectLinkItem(count, fromHistory);
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
        btn.onClick.AddListener(() => SelectLinkItem(count, false));

        Link newLink = new Link(LinkType.Dodge, true);
		Session.actionList.Add(newLink);

		if (!fromHistory)
		{
			HistoryAction ha = new HistoryAction( ActionType.AddDodge, ActionType.RemoveDodge ); 
			HistoryManager.instance.AddHistory(ha);
            SelectLinkItem(count, true);
        }
        else
        {
            SelectLinkItem(count, fromHistory);
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

}
