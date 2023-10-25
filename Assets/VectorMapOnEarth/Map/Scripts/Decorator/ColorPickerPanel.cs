using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorPickerPanel : MonoBehaviour
{
    public static ColorPickerPanel Instance;
    public ColorPicker ColorBrain;
    Color CurrentColor, initialColor;
    Action<Color> SetColor, ColorChange;
    Animator PanelAnimator;
    RectTransform Panel;
    Coroutine checkMouseClick, changeColorCoroutine;

    void Start()
    {
        Instance = this;
        PanelAnimator = GetComponent<Animator>();
        Panel = GetComponent<RectTransform>();
    }

    public void CallColorPicker(Color defaultColor, Vector2 buttonPosition, Action<Color> acceptAction, Action<Color> updateAction)
    {
        PanelsHierarchy.SetAsTopPanelNoBlur(transform);
        initialColor = defaultColor;
        SetColor = acceptAction;
        transform.position = buttonPosition;
        ColorBrain.CurrentColor = defaultColor;
        PanelAnimator.SetTrigger("on");

        ColorChange = updateAction;

        changeColorCoroutine = StartCoroutine(ChangeColorLikeUpdate());
        checkMouseClick = StartCoroutine(CheckOutsideClick());
    }

    public void ChangeColor(Color color)
    {
        CurrentColor = color;
    }

    public void Accept()
    {
        SetColor(CurrentColor);
        ColorChange = null;
        if (checkMouseClick != null)
            StopCoroutine(checkMouseClick);
        if (changeColorCoroutine != null)
            StopCoroutine(changeColorCoroutine);
        PanelAnimator.SetTrigger("off");
    }

    IEnumerator CheckOutsideClick()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0) && !RectTransformUtility.RectangleContainsScreenPoint(Panel, Input.mousePosition)
                && GetComponent<CanvasGroup>().blocksRaycasts)
            {
                //----------------------
                if (ColorChange != null)
                    ColorChange(initialColor);

                ColorChange = null;

                if (changeColorCoroutine != null)
                    StopCoroutine(changeColorCoroutine);
                //----------------------

                PanelAnimator.SetTrigger("off");
                yield break;
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator ChangeColorLikeUpdate()
    {
        while (true)
        {
            if (ColorChange != null)
            {
                ColorChange(CurrentColor);

                yield return null;
            }
            else
                yield break;
        }
    }
}
