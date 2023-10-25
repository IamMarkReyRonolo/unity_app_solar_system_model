using UnityEngine;
using UnityEngine.UI;

public class SetColor : MonoBehaviour
{
    public Color initialColor, newColor;
    public ColorPicker colorDialog;
    public Material chooseMaterial, chooseMaterial2;

    Image thisImage;
    //public GameObject MapPanel;
    Vector3 colorPickerPosition;
    private void Start()
    {
        var scaleFactor = GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor;
        colorPickerPosition = new Vector3(transform.GetChild(1).GetChild(0).position.x , transform.GetChild(1).position.y);
        initialColor = chooseMaterial.color;
        var col = initialColor;
        col.a = chooseMaterial.color.a;
        chooseMaterial.color = col;

        thisImage = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        thisImage.color = chooseMaterial.color;
        colorDialog.CurrentColor = (initialColor);
    }

    public void EditColor()
    {
        var scaleFactor = GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor;
        colorPickerPosition = new Vector3(transform.GetChild(1).GetChild(0).position.x , transform.GetChild(1).position.y);
        ColorPickerPanel.Instance.CallColorPicker(chooseMaterial.color, colorPickerPosition, SetNewColor, ChangeColor);
        
    }
    public void SetNewColor(Color color)
    {
        var col = color;
        col.a = chooseMaterial.color.a;
        chooseMaterial.color = col;

        //chooseMaterial.color = color;
        thisImage.color = chooseMaterial.color;
        //initialColor = color;
        
    }

    public void ChangeColor(Color color)
    {
        var col = color;
        col.a = chooseMaterial.color.a;
        chooseMaterial.color = col;

        //chooseMaterial.color = color;
        thisImage.color = chooseMaterial.color;

    }

    public void OnButtonDown()
    {
        colorDialog.gameObject.SetActive(!colorDialog.gameObject.activeSelf);
    }

    void Update()
    {
        if (thisImage.color != chooseMaterial.color)
            thisImage.color = chooseMaterial.color;

        if (chooseMaterial2 != null)
        {
            var col = chooseMaterial.color;
            col.a = 1;
            chooseMaterial2.color = col;
        }
    }
}
