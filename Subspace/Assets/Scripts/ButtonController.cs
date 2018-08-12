using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

public class ButtonController : MonoBehaviour {

    public Color normal;
    public Color clicked;

    public PhotonView view;
    
    public string rpc;
    public bool toogleBool;
    public bool lastBool;

    public string parameter;

    public float coolDown = 2;

    private bool executed;
    private float timer;

    public void Start()
    {
        GetComponentInChildren<MeshRenderer>().sharedMaterials[1].color = normal;
        if (view == null)
            view = PhotonView.Get(this);
        executed = false;
        lastBool = (bool)(GetParams()[0]);
    }

    private void Update()
    {
        if (executed)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                GetComponentInChildren<MeshRenderer>().sharedMaterials[1].color = normal;
                executed = false;
            }
        }
    }

    public void Execute()
    {
        if (executed) return;
        executed = true;
        timer = coolDown;
        GetComponentInChildren<MeshRenderer>().sharedMaterials[1].color = clicked;
        object[] param = GetParams();
        if (toogleBool)
        {
            bool cur = (bool)param[0];
            if (cur == lastBool)
            {
                lastBool = !cur;
            }
            else
            {
                param[0] = !cur;
                lastBool = cur;
            }
        }
        view.RPC(rpc, PhotonTargets.All, param);

        
    }

    object[] GetParams()
    {
        string workParam = parameter;
        string[] commands = workParam.Split(';');
        object[] results = new object[commands.Length - 1];
        
        for (int i = 0; i < commands.Length - 1; i++)
        {
            string[] type = commands[i].Split('=');
            object objs = null;
            if (String.Compare(type[0], "bool") == 0)
            {
                objs = new bool();
                if (String.Compare(type[1], "true") == 0)
                    objs = true;
                else
                    objs = false;
            }
            else if (String.Compare(type[0], "string") == 0)
            {
                objs = new string(type[1].ToCharArray());
            }
            else if (String.Compare(type[0], "int") == 0)
            {
                objs = new int();
                objs = Int32.Parse(type[1]);
            }
            else if (String.Compare(type[0], "float") == 0)
            {
                objs = new float();
                objs = float.Parse(type[1]);
            }
            else if (String.Compare(type[0], "double") == 0)
            {
                objs = new double();
                objs = double.Parse(type[1]);
            }
            results[i] = objs;
        }
       

        return results;
    }

}
