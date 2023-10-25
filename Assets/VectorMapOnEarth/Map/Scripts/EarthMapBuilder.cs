using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Models.Factories;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UI;
public class EarthMapBuilder : MonoBehaviour
{
    public static EarthMapBuilder instance;
    #region HideInInspector
    [HideInInspector]
    public uint zoom;
    [HideInInspector]
    public float initialTextSize, previousSize;
    [HideInInspector]
    public bool isInitialMapBuild = true;
    #endregion

    #region Public
    public GameObject Factories;
    public Slider TextSlider;
    public RectTransform SettingsPanel, ColorPicker;
    public TextAsset countries, labels;
    public EarthCountryFactory factory;
    #endregion

    #region Initialization

    public void InitVariables()
    {
        isInitialMapBuild = true;
        instance = this;
        zoom = 0;
        initialTextSize = TextSlider.value;
        previousSize = initialTextSize;
    }
    #endregion

    public void Awake()
    {
        InitVariables();
        //CreateNewLevel(0);
        isInitialMapBuild = false;
    }

    [ContextMenu("CreateMap")]
    public void ContextMenuCreate()
    {
        InitVariables();
        CreateNewLevel(0);
        isInitialMapBuild = false;
    }

    [ContextMenu("DeleteMap")]
    public void ContextMenuDelete()
    {
        var borderList = new List<GameObject>();
        for (int i = 0; i < factory.GeometryTransform.childCount; i++)
        {
            borderList.Add(factory.GeometryTransform.GetChild(i).gameObject);
        }
        foreach(var g in borderList)
        {
            DestroyImmediate(g);
        }

        var labelList = new List<GameObject>();
        for (int j = 0; j < factory.LabelsTransform.childCount; j++)
        {
            labelList.Add(factory.LabelsTransform.GetChild(j).gameObject);
        }
        foreach (var g in labelList)
        {
            DestroyImmediate(g);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region MapBuilding
    public void CreateNewLevel(int zoomLevel)
    {
        StartCoroutine(CreateTile());
    }

    protected virtual IEnumerator CreateTile()
    {
        if (countries != null)
        {
            Debug.Log("File Exists");
            ConstructTile(countries);
        }

        yield return null;
    }

    public async void ConstructTile(TextAsset textAsset)
    {
        JSONObject mapData = ReadDataFromJson(textAsset);

        foreach (var entity in mapData["features"].list)
        {
            factory.Create(entity);
        }

        if (labels != null)
        {
            JSONObject mapData2 = ReadDataFromJson(labels);

            foreach (var entity in mapData2["features"].list)
            {
                factory.CreatePoints(entity);
            }
        }
    }

    private static JSONObject ReadDataFromJson(TextAsset textAsset)
    {
        JSONObject temp = new JSONObject();
        temp = new JSONObject(textAsset.text);

        return temp;
    }
    #endregion

    public void SetPointsVisible()
    {
        factory.LabelsTransform.gameObject.SetActive(!factory.LabelsTransform.gameObject.activeSelf);
    }

    public void SetFontSize()
    {
        var container = factory.LabelsTransform;
        for (int i = 0; i < factory.LabelsTransform.childCount; i++)
        {
            factory.LabelsTransform.GetChild(i).GetComponent<TextMesh>().fontSize += (int)TextSlider.value - (int)previousSize;
        }
        previousSize = (int)TextSlider.value;
    }
}
