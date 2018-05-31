using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LinkDrag : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerEnterHandler
{
    private bool dragging;
    private Image img;
    private Transform content;
	private int indexFrom;

    void Awake()
    {
        img = GetComponent<Image>();
        content = transform.parent;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if ( !dragging )
        {
            Session.dragging = true;
            Session.currentDrag = transform.GetSiblingIndex();
			indexFrom = Session.currentDrag;
            dragging = true;
            img.color = Color.yellow;
        }       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if ( dragging )
        {
            Session.dragging = false;
            dragging = false;
            img.color = Color.white;

			HistoryAction ha = new HistoryAction(ActionType.MoveOn, ActionType.MoveBack, transform.GetSiblingIndex(), indexFrom);
			HistoryManager.instance.AddHistory(ha);
        }        
    }    

    public void OnPointerEnter(PointerEventData eventData)
    {
        if ( Session.dragging )
        {
            int index = transform.GetSiblingIndex();
            if ( index != Session.currentDrag )
            {
                //if ( index + 1 > Session.actionList.Count - 1 )
                //    return;

                //if ( index - 1 < 0 )
                //    return;

                int thisIndex = transform.GetSiblingIndex();
                int sessionindex = Session.currentDrag;

                content.GetChild(Session.currentDrag).SetSiblingIndex(thisIndex);
                transform.SetSiblingIndex(sessionindex);

				Link thisLink  = (Link)Session.actionList[thisIndex];
				Session.actionList[thisIndex] = Session.actionList[sessionindex];
				Session.actionList[sessionindex] = thisLink;
				Session.currentDrag = thisIndex;
            }
        }
    }
		
}
