/*
  DoublePendulum.cs (Unity)
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DoublePendulum : MonoBehaviour {
  string title = "DoublePendulum";
  int cnt;

  void Start(){
    Debug.Log(title + " 日本語 UTF8 Start.");
    cnt = 0;
  }

  void Update(){
    string s = " 日本語 running...";
    ++cnt;
    if(cnt % 60 == 0) Debug.Log(title + cnt + s);
  }
}
