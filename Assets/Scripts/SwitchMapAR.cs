﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchMapAR : MonoBehaviour
{
    public GameObject mapCamera;
    public GameObject buttonToSwitchView;

    public GameObject startLocationTextBox;
    public GameObject destinationLocaitonTextBox;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClick()
    {
        if(mapCamera.activeSelf)
        {
            mapCamera.SetActive(false);
            buttonToSwitchView.GetComponent<Button>().GetComponentInChildren<Text>().text = "Map";
            startLocationTextBox.SetActive(false);
            destinationLocaitonTextBox.SetActive(false);
        }
        else
        {
            mapCamera.SetActive(true);
            buttonToSwitchView.GetComponent<Button>().GetComponentInChildren<Text>().text = "AR";
            startLocationTextBox.SetActive(true);
            destinationLocaitonTextBox.SetActive(true);
        }

    }
}
