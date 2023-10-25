using System;
using System.Collections.Generic;
using Helpers;
using Models.Factories;
using UnityEngine;
using System.Linq;

namespace Models.Factories
{
    public class EarthCountryFactory : MonoBehaviour
    {
        public static EarthCountryFactory instance;

        public  string XmlTag { get { return "Earth_countries"; } }
        public virtual Func<JSONObject, bool> Query { get; set; }

        public GameObject Earth;
        public Material Frontiers;
        public Transform GeometryTransform, LabelsTransform;
        public TextMesh MapLabel;

        public void Awake()
        {
            instance = this;
            Query = (geo) => (geo["geometry"]["type"].str == "Polygon" || geo["geometry"]["type"].str == "MultiPolygon" || geo["geometry"]["type"].str == "LineString" || geo["geometry"]["type"].str == "MultiLineString");
        }

        public void Create(JSONObject geo)
        {
            if ((geo["geometry"]["type"].str == "Polygon" || geo["geometry"]["type"].str == "MultiPolygon"))
            {
                foreach (var bb in geo["geometry"]["coordinates"].list)
                {
                    JSONObject jo = null;
                    if (bb.list == null)
                    {
                        //print("-1");
                        jo = bb;
                    }
                    else if (bb.list.Count == 0)
                    {
                        //print("-1.0");
                        jo = bb;
                    }
                    else if (bb.list[0].list == null)
                    {
                        //print("-2");
                        jo = bb;
                    }
                    else if (bb.list[0].list.Count == 0)
                    {
                        //print("-2.0");
                        jo = bb;
                    }
                    else jo = (bb.list[0].list[0].IsArray) ? bb.list[0] : bb;

                    if (jo.list == null) continue;

                    var count = jo.list.Count; //-1

                    if (count < 3)
                        continue;

                    List<Vector3> earthBoundarEnds = new List<Vector3>();
                    earthBoundarEnds.Clear();

                    bool lonSign = false;
                    int plus = 0, minus = 0;

                    for (int i = 0; i < count; i++)
                    {
                        var c = jo.list[i];
                        if (c[0].f > 0)
                        {
                            plus++;
                        }
                        else
                            minus++;
                        var dotMerc = GeoConvert.LatLonToMetersForEarth(c[1].f, c[0].f);
                        dotMerc += new Vector3d(Earth.transform.position.x, Earth.transform.position.y, Earth.transform.position.z);
                        earthBoundarEnds.Add((Vector3)dotMerc);
                    }

                    var earthMeshBoundary = new GameObject("mesh").AddComponent<MeshFilter>();
                    earthMeshBoundary.gameObject.AddComponent<MeshRenderer>();
                    var earthMesh = earthMeshBoundary.mesh;

                    earthMesh.SetVertices(earthBoundarEnds);
                    earthMesh.SetIndices((new List<int>(System.Linq.Enumerable.Range(0, earthBoundarEnds.Count))).ToArray(), MeshTopology.LineStrip, 0);

                    if (geo["properties"]["id"] != null)
                        earthMeshBoundary.name = geo["properties"]["id"].ToString();
                    if (geo["properties"]["name"] != null)
                        earthMeshBoundary.name = geo["properties"]["name"].ToString();

                    if (GeometryTransform.childCount == 0)
                    {
                        var newG = new GameObject(earthMeshBoundary.name);
                        newG.transform.parent = GeometryTransform;
                        earthMeshBoundary.transform.parent = newG.transform;
                    }
                    else
                    {
                        bool childFlag = false;
                        for (int ch = 0; ch < GeometryTransform.childCount; ch++)
                        {
                            if (GeometryTransform.GetChild(ch).name == earthMeshBoundary.name)
                            {
                                earthMeshBoundary.transform.parent = GeometryTransform.GetChild(ch);
                                childFlag = true;
                            }
                        }
                        if (!childFlag)
                        {
                            var newG = new GameObject(earthMeshBoundary.name);
                            newG.transform.parent = GeometryTransform;
                            earthMeshBoundary.transform.parent = newG.transform;
                        }
                    }

                    earthMeshBoundary.GetComponent<MeshRenderer>().material = Frontiers;
                    //earthMeshBoundary.transform.parent = GeometryTransform;

                }
            }
        }

        public void CreatePoints(JSONObject geo)
        {
            if (geo["area"].f > 40000f)
            {
                List<Vector3> earthBoundarEnds = new List<Vector3>();
                earthBoundarEnds.Clear();

                var name = geo["name"]["common"].str;
                name = System.Text.RegularExpressions.Regex.Unescape(name);
                var c = geo["latlng"].list;
                var dotMerc = GeoConvert.LatLonToMetersForEarth(c[0].f, c[1].f);
                dotMerc += new Vector3d(Earth.transform.position.x, Earth.transform.position.y, Earth.transform.position.z);
                earthBoundarEnds.Add((Vector3)dotMerc);
                var label = GameObject.Instantiate(MapLabel);
                label.transform.parent = LabelsTransform;
                label.transform.position = (Vector3)dotMerc;
                label.transform.LookAt(Earth.transform);
                label.text = name;
                label.fontSize += (int)EarthMapBuilder.instance.TextSlider.value - (int)EarthMapBuilder.instance.previousSize;
                if (name.Contains("of the Congo") || name.Contains("Norway") || name.Contains("Ivory Coast"))
                    label.anchor = TextAnchor.UpperCenter;

                label.transform.name = name;

                int spaceCount = 0;
                int symbNum = 0;
                string newString = "";
                
                foreach (var symb in label.text)
                {
                    newString += symb;
                    symbNum++;
                    if (symb == ' ')
                        spaceCount++;
                    if (spaceCount == 2)
                    {
                        spaceCount = 0;
                        newString += "\n";
                    }
                }
                label.text = newString;

                if (geo["area"].f > 170000f && geo["area"].f < 380000f)
                    label.characterSize = 4;
                else if (geo["area"].f <= 170000f && geo["area"].f > 100000f)
                    label.characterSize = 3;
                else if (geo["area"].f <= 100000f && geo["area"].f > 40000f)
                    label.characterSize = 2;
                else if (geo["area"].f <= 40000f)
                    label.characterSize = 1;
                else if (geo["area"].f < 550000f && geo["area"].f >= 380000f)
                    label.characterSize = 5;
                else if (geo["area"].f >= 550000f && geo["area"].f < 800000f)
                    label.characterSize = 6;
                else if (geo["area"].f >= 800000f && geo["area"].f < 1600000f)
                    label.characterSize = 7;
                else if (geo["area"].f >= 1600000f && geo["area"].f < 3000000f)
                    label.characterSize = 8;
                else if (geo["area"].f >= 3000000f)
                    label.characterSize = 13;
            }
        }

    }
}
