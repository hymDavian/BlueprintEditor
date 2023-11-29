using BPJsonType;
using NodeUICtor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramMain : MonoBehaviour
{

    [SerializeField]
    private RectTransform maincanvas;
    [SerializeField]
    private RectTransform mainPanel;
    [SerializeField]
    private RectTransform varlist;
    [SerializeField]
    private RectTransform varcreate;
    [SerializeField]
    private RectTransform filetools;


    public static event Action<float> OnUpdate;

    // Start is called before the first frame update
    void Start()
    {
        NodeFuncJsonClass.init();
        NodeLine.mainPanel = mainPanel;
        var bpui = new BPUI(mainPanel);
        new VariableUI(varlist, varcreate, filetools, bpui);
    }

    // Update is called once per frame
    void Update()
    {
        if(OnUpdate != null)
        {
            OnUpdate.Invoke(Time.deltaTime);
        }
    }
}
