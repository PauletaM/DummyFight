using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LinkDrag : MonoBehaviour, IDragHandler, IPointerUpHandler
{
    private bool dragging, linkadded;
    private Transform parent, canvas, shadow;
    private RectTransform rectTrans;
    public int index;
    private Link link;

    void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("canvas").transform;
        shadow = GameObject.FindGameObjectWithTag("shadow").transform;
        parent = transform.parent;
        rectTrans = GetComponent<RectTransform>();        
    }


    public void OnDrag(PointerEventData eventData)
    {
        dragging = true;
        if ( !linkadded )
        {
            linkadded = true;
            transform.SetParent(canvas);
            Link empty = new Link(LinkType.Empty, false);
            link = (Link)Session.actionList[ index ];
            Session.actionList[ index ] = empty;            
            Manager.instance.RefreshUI();
        }        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if ( dragging )
        {
            dragging = false;            
            linkadded = false;            
            Session.actionList[ index ] = link;
            Manager.instance.RefreshUI();
            Destroy(gameObject);
        }        
    }

    void Update()
    {
        if ( dragging )
        {
            rectTrans.position = Input.mousePosition;
        }
    }

}
