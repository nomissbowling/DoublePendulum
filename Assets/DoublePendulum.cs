/*
  DoublePendulum.cs (Unity)

  Camera        (   .0,  4.5,-10.0) ( -1.0,   .0,   .0) (  1.0,  1.0,  1.0)

  Cube          (   .0,  4.0,  2.0) (   .0,   .0,   .0) (  1.0,  1.0,  1.0)
  Cylinder0     (   .0,  2.5,  2.0) (   .0,   .0,   .0) (   .1,  1.5,   .1)
  Sphere0       (   .0,  1.0,  2.0) (   .0,   .0,   .0) (  1.0,  1.0,  1.0)
  Cylinder1     (   .0, -0.5,  2.0) (   .0,   .0,   .0) (   .1,  1.5,   .1)
  Sphere1       (   .0, -2.0,  2.0) (   .0,   .0,   .0) (  1.0,  1.0,  1.0)
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System; // Math

public static class GV {
  static public string Title = "DoublePendulum";

  static public float gra = 9.8f;
  static public float dT = 0.000001f;
  static public int nEuler = 5000;
  static public float convlen = 3.0f;

  static public float len0 = 1.3f; // 1.4f;
  static public float m0 = 1.2f;
  static public float th0 = 3.1f;
  static public float w0 = 0.0f; // th0dot
  static public float len1 = 0.9f; // 1.2f;
  static public float m1 = 0.1f;
  static public float th1 = -2.0f;
  static public float w1 = 0.0f; // th1dot

  static public float cbrt(float m){ return (float)Math.Pow(m, 1.0f / 3.0f); }
  static public float cos(float th){ return (float)Math.Cos(th); }
  static public float sin(float th){ return (float)Math.Sin(th); }
  static public float atan(float m){ return (float)Math.Atan(m); }
  static public Quaternion tan2q(Vector2 e, Vector2 s){
    // float th = ((float)Math.PI / 2.0f) + GV.atan((e.y - s.y) / (e.x - s.x));
    float th = GV.atan(-(e.x - s.x) / (e.y - s.y));
    return new Quaternion(0.0f, 0.0f, sin(th / 2.0f), cos(th / 2.0f));
  }
  static public void rotpos(GameObject o, Vector2 e, Vector2 s, float z){
    Vector2 m = (s + e) /  2.0f;
    o.transform.position = new Vector3(m.x, m.y, z);
    o.transform.rotation = tan2q(e, s);
  }
}

public class DoublePendulum : MonoBehaviour {
  int cnt = 0;
  GameObject go;
  GameObject cube, cylinder0, sphere0, cylinder1, sphere1;

  void OnDestroy(){
    Debug.Log(GV.Title + " Destroy.");
  }

  void Start(){
    Debug.Log(GV.Title + " 日本語 UTF8 Start.");
    go = GameObject.Find("GameObject"); // default
    Debug.Log(go);
/*
    // Debug.Log(go.GetComponent<...>()); // class name
    GameObject c = GameObject.Find("Cube");
    Debug.Log(c);
    Debug.Log(c.transform.position);
    c.transform.Translate(-2.0f, 0.0f, 0.0f);
    GameObject d = GameObject.CreatePrimitive(PrimitiveType.Cube);
    Debug.Log(d);
    d.transform.Translate(2.0f, 0.0f, 0.0f);
*/
/*
    GameObject e = go.AddComponent<...>(); // class name
    Debug.Log(e);
    e.transform.Translate(-1.0f, 0.0f, 0.0f);
*/
    cube = GameObject.Find("Cube");
    cylinder0 = GameObject.Find("Cylinder0");
    sphere0 = GameObject.Find("Sphere0");
    cylinder1 = GameObject.Find("Cylinder1");
    sphere1 = GameObject.Find("Sphere1");
  }

  void Update(){
    string s = " 日本語 running...";
    ++cnt;
    if(cnt % 60 == 0) Debug.Log(GV.Title + cnt + s);

    float a0 = (GV.m0 + GV.m1) * GV.len0*GV.len0;
    float a1 = GV.m1 * GV.len1*GV.len1;
    for(int skp = 0; skp < GV.nEuler; ++skp){ // Euler() / 1 frame
      float mll = GV.m1 * GV.len0 * GV.len1;
      float b = mll * GV.cos(GV.th0 - GV.th1);
      float d0 = -mll * GV.w1*GV.w1 * GV.sin(GV.th0 - GV.th1)
        -(GV.m0 + GV.m1) * GV.gra * GV.len0 * GV.sin(GV.th0);
      float d1 = mll * GV.w0*GV.w0 * GV.sin(GV.th0 - GV.th1)
        -GV.m1 * GV.gra * GV.len1 * GV.sin(GV.th1);
      float dw0 = (a1 * d0 - b * d1) / (a0 * a1 - b*b); // th0dotdot
      GV.w0 += dw0 * GV.dT;
      GV.th0 += GV.w0 * GV.dT;
      float dw1 = (a0 * d1 - b * d0) / (a0 * a1 - b*b); // th1dotdot
      GV.w1 += dw1 * GV.dT;
      GV.th1 += GV.w1 * GV.dT;
    }

    float z = 2.0f;
    Vector3 r = new Vector3(1.0f, 1.0f, 1.0f);
    Vector2 c = new Vector2(0.0f, 4.0f);
    Vector2 p0 = new Vector2(c.x + GV.convlen * GV.len0 * GV.sin(GV.th0),
      c.y - GV.convlen * GV.len0 * GV.cos(GV.th0));
    Vector2 p1 = new Vector2(p0.x + GV.convlen * GV.len1 * GV.sin(GV.th1),
      p0.y - GV.convlen * GV.len1 * GV.cos(GV.th1));
    cube.transform.position = new Vector3(c.x, c.y, z); // 00FF00FF
    GV.rotpos(cylinder0, p0, c, z); // 000000FF
    sphere0.transform.position = new Vector3(p0.x, p0.y, z); // 0000FFFF
    sphere0.transform.localScale = GV.cbrt(GV.m0) * r;
    GV.rotpos(cylinder1, p1, p0, z); // 000000FF
    sphere1.transform.position = new Vector3(p1.x, p1.y, z); // FF0000FF
    sphere1.transform.localScale = GV.cbrt(GV.m1) * r;
  }
}
