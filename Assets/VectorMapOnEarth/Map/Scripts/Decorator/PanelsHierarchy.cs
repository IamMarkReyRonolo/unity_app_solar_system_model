using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelsHierarchy : MonoBehaviour
{
    [SerializeField] GameObject BlockBackground;
    static Transform BlurBackground;

    static Canvas MainCanvas;

    private void Start()
    {
        //BlurBackground = BlockBackground.transform;
        MainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    public static void SetAsTopPanelNoBlur(Transform panel)
    {
        panel.SetAsLastSibling();
    }

    public static void SetAsTopPanel(Transform panel)
    {
        //BlurBackground.gameObject.SetActive(true);
        //BlurBackground.SetAsLastSibling();
        panel.SetAsLastSibling();
    }

    public static void HideBlocker()
    {
        //BlurBackground.gameObject.SetActive(false);
    }

    public static float GetCanvasScaler()
    {
        return MainCanvas.scaleFactor;
    }
}
