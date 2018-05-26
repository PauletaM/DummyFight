using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    private int value;
    private Text label;
    public Button button;
    private Image img;

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
    }

    public void Decrease()
    {
        value--;
        label.text = "" + value;
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
