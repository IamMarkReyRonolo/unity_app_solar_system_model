
using System.Collections.Generic;


[System.Serializable]
public class UserDataList
{
  // public UserData[] userdata;
  public List<UserData> userdata;

  public void AddToList(UserData data)
  {
    userdata.Add(data);
  }
}
