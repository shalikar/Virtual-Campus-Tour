﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QRCodeReaderDemo : MonoBehaviour {

    private IReader QRReader;
    public Text resultText;
    public RawImage image;
    private Singleton singleton;
    private string role;


    void Awake () {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
	}

    // Use this for initialization
    private void Start()
    {
        singleton = Singleton.Instance();
        QRReader = new QRCodeReader();
        QRReader.Camera.Play();
        QRReader.OnReady += StartReadingQR;

        QRReader.StatusChanged += QRReader_StatusChanged;
    }

    private void QRReader_StatusChanged(object sender, System.EventArgs e)
    {
        resultText.text = "Status: " + QRReader.Status;
    }

    private void StartReadingQR(object sender, System.EventArgs e)
    {
        image.transform.localEulerAngles = QRReader.Camera.GetEulerAngles();
        image.transform.localScale = QRReader.Camera.GetScale();
        image.texture = QRReader.Camera.Texture;

        RectTransform rectTransform = image.GetComponent<RectTransform>();
        float height = rectTransform.sizeDelta.x * (QRReader.Camera.Height / QRReader.Camera.Width);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    }

    // Update is called once per frame
    void Update () {

        if (QRReader == null)
        {
            return;
        }

        QRReader.Update();
	}

    public void StartScanning()
    {
        if (QRReader == null)
        {
            Debug.LogWarning("No valid camera - Click Start");
            return;
        }

        // Start Scanning
            QRReader.Scan((barCodeType, barCodeValue) => {
            QRReader.Stop();
            resultText.text = "Found: [" + barCodeType + "] " + "<b>" + barCodeValue +"</b>";
            singleton.setBuildingInfo(barCodeValue);
                
            Debug.Log("Building Information : " + singleton.getBuildingInfo());
            QRReader.Camera.Stop();
            SceneManager.LoadScene("QRInfo");
            
            });


    }



    public void onBackClick()
    {
        singleton = Singleton.Instance();
        role = singleton.getUserRole();
        Debug.Log("Role : " + role);
        if (role == "Student")
        {
            singleton.setARType("schedule");
        }

        else if (role == "Guest")
        {
            singleton.setARType("tour");
        }
        QRReader.Stop();
        QRReader.Camera.Stop();
        SceneManager.LoadScene("AR");
    }
}





