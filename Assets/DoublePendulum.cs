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
  static public float dT = 0.00001f; // 0.000001f;
  static public int nEuler = 5000;
  static public float convlen = 3.0f;
  static public float z = 2.0f; // sphere position
  static public float t3 = (float)Math.PI / 60.0f;
  static public float cr = -20.0f; // camera position

  static public float len0 = 1.4f;
  static public float m0 = 1.2f;
  static public float th0 = 3.1f;
  static public float w0 = 0.0f; // th0dot
  static public float len1 = 0.7f;
  static public float m1 = 0.3f;
  static public float th1 = -2.0f;
  static public float w1 = 0.0f; // th1dot

  static public float cbrt(float m){ return (float)Math.Pow(m, 1.0f / 3.0f); }
  static public float cos(float th){ return (float)Math.Cos(th); }
  static public float sin(float th){ return (float)Math.Sin(th); }
  static public float atan(float m){ return (float)Math.Atan(m); }
  static public Quaternion tan2q(Vector3 e, Vector3 s){
    float th = -((float)Math.PI / 2.0f) + GV.atan((e.y - s.y) / (e.x - s.x));
    // float th = GV.atan(-(e.x - s.x) / (e.y - s.y));
    return new Quaternion(0.0f, 0.0f, sin(th / 2.0f), cos(th / 2.0f));
  }
  static public void rotpos(GameObject o, Vector3 e, Vector3 s, float len){
    o.transform.position = (s + e) /  2.0f;
    o.transform.rotation = tan2q(e, s);
    o.transform.localScale = new Vector3(0.1f, convlen * len / 2.0f, 0.1f);
    o.GetComponent<Renderer>().material.color = Color.red;
  }
  static public void scapos(GameObject o, Vector3 p, float m, Color c){
    o.transform.position = p;
    o.transform.localScale = cbrt(m) * new Vector3(1.0f, 1.0f, 1.0f);
    Renderer rend = o.GetComponent<Renderer>();
    Material mtrl = rend.material; // .sharedMaterial to change all instance
    // mtrl.shader = Shader.Find("_Color");
    mtrl.color = c; // _Color
    // if(mtrl.HasProperty("_TintColor")) mtrl.SetColor("_TintColor", c);
    // mtrl.shader = Shader.Find("Specular");
    // mtrl.SetColor("_SpecColor", c);
  }
  static public void roofpos(GameObject o, Vector3 p, float a, Color c, int n){
    float th = t3 * n;
    float s = sin(th / 2.0f);
    o.transform.position = p;
    o.transform.rotation = new Quaternion(s, s, s, cos(th / 2.0f));
    o.transform.localScale = a * new Vector3(1.0f, 1.0f, 1.0f);
    o.GetComponent<Renderer>().material.color = c;
  }
}

public class DoublePendulum : MonoBehaviour {
  int cnt = 0;
  GameObject go, cam;
  GameObject cube, cylinder0, sphere0, cylinder1, sphere1;

  void OnDestroy(){
    Debug.Log(GV.Title + " Destroy.");
  }

  void Start(){
    Debug.Log(GV.Title + " 日本語 UTF8 Start.");
    go = GameObject.Find("GameObject"); // default
    Debug.Log(go);
    // Camera cam = go.GetComponent<Camera>(); // null (UnityEngine.Camera)
    // Camera cam = Camera.main; // default (UnityEngine.Camera)
    cam = GameObject.Find("Main Camera"); // default (UnityEngine.GameObject)
    Debug.Log(cam);
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
/*
    // load pre-set objects
    cube = GameObject.Find("Cube");
    cylinder0 = GameObject.Find("Cylinder0");
    sphere0 = GameObject.Find("Sphere0");
    cylinder1 = GameObject.Find("Cylinder1");
    sphere1 = GameObject.Find("Sphere1");
*/
    // create objects
    cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    cylinder0 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
    sphere0 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    cylinder1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
    sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
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

    Vector3 c = new Vector3(0.0f, 4.0f, GV.z);
    Vector3 p0 = new Vector3(c.x + GV.convlen * GV.len0 * GV.sin(GV.th0),
      c.y - GV.convlen * GV.len0 * GV.cos(GV.th0), GV.z);
    Vector3 p1 = new Vector3(p0.x + GV.convlen * GV.len1 * GV.sin(GV.th1),
      p0.y - GV.convlen * GV.len1 * GV.cos(GV.th1), GV.z);
    GV.roofpos(cube, c, 0.7f, Color.blue, cnt / 60);
    GV.rotpos(cylinder0, p0, c, GV.len0);
    GV.scapos(sphere0, p0, GV.m0, Color.yellow);
    GV.rotpos(cylinder1, p1, p0, GV.len1);
    GV.scapos(sphere1, p1, GV.m1, Color.green);

    float pt = GV.t3 * cnt / 30;
    cam.transform.position = new Vector3(
      GV.cr * GV.sin(pt), 4.5f, GV.cr * GV.cos(pt));
    float ps = GV.sin(pt / 2.0f);
    float pc = GV.cos(pt / 2.0f);
    Quaternion qp = new Quaternion(0.0f, ps, 0.0f, pc);
    float tt = GV.t3 * 6 * GV.sin(GV.t3 * cnt);
    float ts = GV.sin(tt / 2.0f);
    float tc = GV.cos(tt / 2.0f);
    Quaternion qx = new Quaternion(ts, 0.0f, 0.0f, tc);
    Quaternion qy = new Quaternion(0.0f, ts, 0.0f, tc);
    // cam.transform.rotation = qp * qy * qx; // water level
    cam.transform.rotation = qy * qx * qp; // yawing pitching rolling
  }
}
