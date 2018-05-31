using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LinkDrag : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerEnterHandler
{
    private bool dragging;
    private Image img;
    private Transform content;

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

                Session.currentDrag = thisIndex;
            }
        }
    }
}
