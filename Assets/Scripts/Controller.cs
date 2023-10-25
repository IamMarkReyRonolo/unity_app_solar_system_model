using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System;
using System.IO;

public class Controller : MonoBehaviour
{
  // public variables
  public GameObject[] planets;
  public GameObject[] overlays;
  public GameObject selectedPlanet;
  public Rigidbody rb;
  public int renderIndex;

  // private variables
  public UserDataList userDataList = new UserDataList();
  public UserData userdata = new UserData();
  private static UdpClient udpClient;
  private Vector3 maxLocalScale;
  private Vector3 minLocalScale;
  private float maxlocalScaleMagnitude;
  private float minlocalScaleMagnitude;
  private int portNum = 22222;
  private bool dragging = false;
  private string command = "";
  private bool received = false;
  private float received_x = 0;
  private float received_y = 0;
  private float scale = 0;
  private bool flip = false;
  private int flipnumber = -1;
  private bool stop = false;
  private float actualLocalScaleMagnitude = 0;
  private float magnitudeP = 0;

  // For Logging
  private bool loggedStopGesture = false;
  private bool loggedSwipeGesture = false;
  private bool loggedDragGesture = false;
  private bool loggedZoomInGesture = false;
  private bool loggedZoomOutGesture = false;
  private bool loggedNextObjectGesture = false;
  private bool loggedPreviousObjectGesture = false;

  void Start()
  {
    LoadFromJson();
    udpClient = new UdpClient(portNum);

    rb = selectedPlanet.GetComponent<Rigidbody>();
    maxLocalScale = new Vector3(1.4f, 1.4f, 1.4f);
    maxlocalScaleMagnitude = maxLocalScale.magnitude;
    minLocalScale = new Vector3(0.4f, 0.4f, 0.4f);

    minlocalScaleMagnitude = minLocalScale.magnitude;
    ChangeModel();
  }

  void Update()
  {
    IPEndPoint remoteEP = null;
    byte[] data = udpClient.Receive(ref remoteEP);
    string message = Encoding.ASCII.GetString(data);
    command = message;
    string[] mess = message.Split(',');
    stop = false;

    if (Input.GetKeyUp(KeyCode.Space))
    {
      print("data saved");
      SaveToJson();
    }

    if (mess.Length == 2)
    {
      string string_x = mess[0];
      string string_y = mess[1];

      received_x = float.Parse(string_x,
      System.Globalization.CultureInfo.InvariantCulture);

      received_y = float.Parse(string_y,
      System.Globalization.CultureInfo.InvariantCulture);
      dragging = false;
      received = true;
    }
    else if (mess.Length == 3)
    {
      string string_x = mess[0];
      string string_y = mess[1];

      received_x = float.Parse(string_x,
      System.Globalization.CultureInfo.InvariantCulture);

      received_y = float.Parse(string_y,
      System.Globalization.CultureInfo.InvariantCulture);
      dragging = true;
      received = true;
    }
    else
    {
      if (command == "Next" || command == "Previous")
      {
        if (command == "Next")
        {
          userdata.NextObjectGesture += 1;
          if (renderIndex + 1 < planets.Length)
          {
            renderIndex += 1;
            ChangeModel();
          }
          ResetLogsFlags("nextgesture");
        }
        else if (command == "Previous")
        {
          userdata.PreviousObjectGesture += 1;
          if (renderIndex != 0)
          {
            renderIndex -= 1;
            ChangeModel();
          }

          ResetLogsFlags("previousgesture");
        }
      }

      else if (command != "" && command != "stop")
      {
        stop = true;
        float scaleFloat = (float.Parse(command, System.Globalization.CultureInfo.InvariantCulture));
        scale = float.Parse(command,
        System.Globalization.CultureInfo.InvariantCulture) / 800;



        if (scaleFloat > 10)
        {
          if (loggedZoomInGesture == false)
          {
            userdata.ZoomInGesture += 1;
          }
          ResetLogsFlags("zoomingesture");
        }
        else if (scaleFloat < -10)
        {
          if (loggedZoomOutGesture == false)
          {
            userdata.ZoomOutGesture += 1;
          }
          ResetLogsFlags("zoomoutgesture");
        }
      }
      else if (command == "")
      {
        ResetLogsFlags("none");
      }

      dragging = false;
      received = false;

      if (command == "stop")
      {

        stop = true;
        if (loggedStopGesture == false)
        {
          userdata.StopGesture += 1;
        }
        ResetLogsFlags("stopgesture");
      }
    }
  }

  void FixedUpdate()
  {
    SetObjectMagnitudes();

    if (received == true && dragging == true)
    {
      ListenForDragGesture();
    }
    else if (received == true)
    {
      ListenForSwipeGesture();
    }
    else if (stop == false)
    {
      AutoRotate();
    }

    ListenForScaleGesture();
  }

  void ChangeModel()
  {
    overlays[0].SetActive(false);
    overlays[1].SetActive(false);
    overlays[2].SetActive(false);
    for (int i = 0; i < planets.Length; i++)
    {
      if (renderIndex == i)
      {
        maxLocalScale = new Vector3(1.35f, 1.35f, 1.35f);
        planets[i].SetActive(true);
        selectedPlanet = planets[i];
        rb = selectedPlanet.GetComponent<Rigidbody>();
        if (renderIndex == 0)
        {
          selectedPlanet.transform.localScale = new Vector3(2, 2, 2);
          flip = false;
          maxLocalScale = new Vector3(2.5f, 2.5f, 2.5f);
        }
        else if (renderIndex == 1 || renderIndex == 9 || renderIndex == 10)
        {
          selectedPlanet.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
          flip = false;
        }
        else if (renderIndex == 5)
        {
          selectedPlanet.transform.localScale = new Vector3(1, 1, 1);
          flip = false;
        }
        else if (renderIndex == 6 || renderIndex == 7 || renderIndex == 8)
        {
          selectedPlanet.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
          flip = false;

          if (renderIndex == 6)
          {
            selectedPlanet.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            maxLocalScale = new Vector3(1, 1, 1);
          }
        }
        else if (renderIndex == 2 || renderIndex == 3)
        {
          selectedPlanet.transform.localScale = new Vector3(-0.6f, -0.6f, -0.6f);
          flip = true;
          if (renderIndex == 3)
          {
            maxLocalScale = new Vector3(2.5f, 2.5f, 2.5f);
          }
        }
        else
        {
          selectedPlanet.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
          flip = false;
        }

        maxlocalScaleMagnitude = maxLocalScale.magnitude;



      }
      else
      {
        planets[i].SetActive(false);
      }
    }

    if (renderIndex == 3)
    {
      overlays[0].SetActive(true);
      overlays[1].SetActive(true);
      overlays[2].SetActive(true);
    }
  }

  void SetObjectMagnitudes()
  {
    actualLocalScaleMagnitude = selectedPlanet.transform.localScale.magnitude;
    magnitudeP = 0;
    if (renderIndex == 0)
    {
      Vector3 scaleP = new Vector3(2, 2, 2);
      magnitudeP = scaleP.magnitude;
    }
    else
    {
      Vector3 scaleP = new Vector3(1, 1, 1);
      magnitudeP = scaleP.magnitude;
    }
  }

  void AutoRotate()
  {
    if (actualLocalScaleMagnitude > magnitudeP)
    {
      selectedPlanet.transform.Rotate(0, Time.deltaTime * 2, 0, Space.Self);
    }
    else
    {
      selectedPlanet.transform.Rotate(0, Time.deltaTime * 10, 0, Space.Self);
    }
  }

  void ListenForSwipeGesture()
  {
    rb.angularDrag = 1;
    if (actualLocalScaleMagnitude > magnitudeP)
    {
      if (renderIndex == 3)
      {
        rb.AddTorque(new Vector3(0, 0.0000000005f, 0) * received_x);
        rb.AddTorque(new Vector3(0.0000000005f, 0, 0) * received_y);
      }
      else
      {
        rb.AddTorque(new Vector3(0, 0.0000000009f, 0) * received_x);
        rb.AddTorque(new Vector3(0.0000000009f, 0, 0) * received_y);
      }
    }
    else
    {
      if (renderIndex == 3)
      {
        rb.AddTorque(new Vector3(0, 0.0000000001f, 0) * received_x);
        rb.AddTorque(new Vector3(0.0000000001f, 0, 0) * received_y);
      }
      else
      {
        rb.AddTorque(new Vector3(0, 0.0000000005f, 0) * received_x);
        rb.AddTorque(new Vector3(0.0000000005f, 0, 0) * received_y);
      }
    }
    if (loggedSwipeGesture == false)
    {
      userdata.SwipeGesture += 1;
    }
    ResetLogsFlags("swipegesture");
  }

  void ListenForDragGesture()
  {
    rb.angularDrag = 30;
    if (actualLocalScaleMagnitude > magnitudeP)
    {
      if (renderIndex == 3)
      {
        rb.AddTorque(new Vector3(0, 0.000000006f, 0) * received_x);
        rb.AddTorque(new Vector3(0.000000006f, 0, 0) * received_y);
      }
      else
      {
        rb.AddTorque(new Vector3(0, 0.00000002f, 0) * received_x);
        rb.AddTorque(new Vector3(0.00000002f, 0, 0) * received_y);
      }
    }
    else
    {
      if (renderIndex == 3)
      {
        rb.AddTorque(new Vector3(0, 0.000000005f, 0) * received_x);
        rb.AddTorque(new Vector3(0.000000005f, 0, 0) * received_y);
      }
      else
      {
        rb.AddTorque(new Vector3(0, 0.00000001f, 0) * received_x);
        rb.AddTorque(new Vector3(0.00000001f, 0, 0) * received_y);
      }
    }

    if (loggedDragGesture == false)
    {
      userdata.DragGesture += 1;
    }
    ResetLogsFlags("draggesture");
  }

  void ListenForScaleGesture()
  {
    if (flip == true)
    {
      flipnumber = -1;
      maxLocalScale *= -1;
      minLocalScale *= -1;

      if (scale != 0)
      {
        if (scale > 0 && (actualLocalScaleMagnitude < maxlocalScaleMagnitude))
        {
          selectedPlanet.transform.localScale += new Vector3(scale * flipnumber, scale * flipnumber, scale * flipnumber);
        }

        if (scale < 0 && (actualLocalScaleMagnitude > minlocalScaleMagnitude))
        {
          selectedPlanet.transform.localScale += new Vector3(scale * flipnumber, scale * flipnumber, scale * flipnumber);
        }
        scale = 0;
      }
    }
    else
    {
      flipnumber = 1;
      maxLocalScale = new Vector3(Math.Abs(maxLocalScale[0]), Math.Abs(maxLocalScale[1]), Math.Abs(maxLocalScale[2]));
      minLocalScale = new Vector3(Math.Abs(minLocalScale[0]), Math.Abs(minLocalScale[1]), Math.Abs(minLocalScale[2]));


      if (scale != 0)
      {

        if (scale > 0 && (actualLocalScaleMagnitude < maxlocalScaleMagnitude))
        {
          selectedPlanet.transform.localScale += new Vector3(scale * flipnumber, scale * flipnumber, scale * flipnumber);
        }

        if (scale < 0 && (actualLocalScaleMagnitude > minlocalScaleMagnitude))
        {
          selectedPlanet.transform.localScale += new Vector3(scale * flipnumber, scale * flipnumber, scale * flipnumber);
        }
        scale = 0;
      }
    }



  }


  public void SaveToJson()
  {
    userdata.UserId = userDataList.userdata.Count;
    userDataList.AddToList(userdata);
    string json = JsonUtility.ToJson(userDataList);
    File.WriteAllText(Application.dataPath + "/UserDataFile.json", json);
    ResetLogs();
    LoadFromJson();
  }

  public void LoadFromJson()
  {
    string json = File.ReadAllText(Application.dataPath + "/UserDataFile.json");
    userDataList = JsonUtility.FromJson<UserDataList>(json);
  }

  public void ResetLogs()
  {
    userdata.UserId = 0;
    userdata.StopGesture = 0;
    userdata.SwipeGesture = 0;
    userdata.DragGesture = 0;
    userdata.ZoomInGesture = 0;
    userdata.ZoomOutGesture = 0;
    userdata.NextObjectGesture = 0;
    userdata.PreviousObjectGesture = 0;
  }

  public void ResetLogsFlags(string flag)
  {
    loggedStopGesture = false;
    loggedSwipeGesture = false;
    loggedDragGesture = false;
    loggedZoomInGesture = false;
    loggedZoomOutGesture = false;
    loggedNextObjectGesture = false;
    loggedPreviousObjectGesture = false;

    if (flag == "stopgesture")
    {
      loggedStopGesture = true;
    }
    else if (flag == "swipegesture")
    {
      loggedSwipeGesture = true;
    }
    else if (flag == "draggesture")
    {
      loggedDragGesture = true;
    }
    else if (flag == "zoomingesture")
    {
      loggedZoomInGesture = true;
    }
    else if (flag == "zoomoutgesture")
    {
      loggedZoomOutGesture = true;
    }
    else if (flag == "nextgesture")
    {
      loggedNextObjectGesture = true;
    }
    else if (flag == "previousgesture")
    {
      loggedPreviousObjectGesture = true;
    }
  }

  void OnApplicationQuit()
  {
    SaveToJson();
  }
}
