using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    public GameObject lThink, lWatch, lAttack, lDodge;
    private ArrayList gtDisplayList;
    private Item currentSelected;
    public Transform content;

    private void Awake()
    {
        gtDisplayList = new ArrayList();
    }

    void Highlight( Item item )
    {
        currentSelected = item;
        foreach ( Item i in gtDisplayList )
            i.Dark();
        item.Highlight();
    }

    public void AddWatch()
    {
        GameObject act = Instantiate(lWatch, content);
        content.GetComponent<RectTransform>().sizeDelta += new Vector2(200, 0);
        Transform actBtn = act.transform.GetChild(1);
        actBtn.gameObject.SetActive(true);
        for ( int i = 0; i < 3; i++ )
        {
            GameObject go = actBtn.GetChild(i).gameObject;
            go.SetActive(true);
            Button btn = go.GetComponent<Button>();
            Item item = go.GetComponent<Item>();
            gtDisplayList.Add(item);
            btn.onClick.AddListener(() => Highlight(item));            
        }            

    }

    public void AddThink()
    {

    }

    public void AddAttack()
    {

    }

    public void AddDodge()
    {

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
