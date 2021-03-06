﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class DisplayLocations : MonoBehaviour
{
    bool toursDisplayed;

    ArrayList tours;

    DB_Details dbDetails;
    DatabaseReference reference;

    private string TourName;
    private Singleton singleton;

    public GameObject ContentPanel;
    public GameObject ErrorPanel;
    public GameObject ListItemPrefab;

    public Text ErrorMessage;

    public InputField TourNameText;
    public InputField AddLocationText;

    void Start()
    {
        tours = new ArrayList();
        dbDetails = new DB_Details();

        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(dbDetails.getDBUrl());

        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        singleton = Singleton.Instance();
        TourName = singleton.getTourName();

        TourNameText.text = TourName;

        getTourData();

        toursDisplayed = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (!toursDisplayed && tours.Count > 0)
        {
            createTourList();
        }
    }

    void getTourData()
    {
        try
        {
            reference.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    throw new Exception("ERROR while fetching data from database!!! Please refresh scene(Click Tours)");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result.Child(dbDetails.getTourDBName()).Child(TourName);

                    string str = snapshot.GetRawJsonValue();
                    JObject jsonLocation = JObject.Parse(str);
                    IList<string> keys = jsonLocation.Properties().Select(p => p.Name).ToList();

                    foreach (string key in keys)
                    {
                        Debug.Log(key);
                        this.tours.Add(key);
                    }
                }
            });
        }
        catch (InvalidCastException e)
        {
            // Perform some action here, and then throw a new exception.
            ErrorMessage.text = e.Message;
            ErrorPanel.SetActive(true);
        }
        catch (Exception e)
        {
            // Perform some action here, and then throw a new exception.
            ErrorMessage.text = e.Message;
            ErrorPanel.SetActive(true);
        }
    }

    void createTourList()
    {
        foreach (string s in tours)
        {
            GameObject newSchedule = Instantiate(ListItemPrefab) as GameObject;

            LocationListItem controller = newSchedule.GetComponent<LocationListItem>();
            controller.Name.text = s;

            newSchedule.transform.parent = ContentPanel.transform;
            newSchedule.transform.localScale = Vector3.one;
        }
        toursDisplayed = true;
    }

    void updateTourListOnAdd(string name)
    {
        GameObject newSchedule = Instantiate(ListItemPrefab) as GameObject;
        LocationListItem controller = newSchedule.GetComponent<LocationListItem>();
        controller.Name.text = name;
        newSchedule.transform.parent = ContentPanel.transform;
        newSchedule.transform.localScale = Vector3.one;

    }


    public void onAddLocation()
    {
        try
        {
            if (AddLocationText.text == "")
            {
                throw new Exception("Please enter location!");
            }
            this.tours.Add(AddLocationText.text);
            updateTourListOnAdd(AddLocationText.text);
            AddLocationText.text = null;
        }
        catch (Exception e)
        {
            // Perform some action here, and then throw a new exception.
            ErrorMessage.text = e.Message;
            ErrorPanel.SetActive(true);
        }
    }

    public void onDelete(Text locationName)
    {

        this.tours.Remove(locationName.text);
    }

    public void onSave()
    {
        string dummyString = "dummy";

        //Creating JSON 
        JObject locations = new JObject();

        foreach (string s in tours)
        {
            locations[s] = dummyString;
        }
        string jsonData = locations.ToString();

        try
        {
            if (TourNameText.text == "")
            {
                throw new Exception("Please enter Tour Name!");
            }

            reference.Child(dbDetails.getTourDBName()).Child(singleton.getTourName()).RemoveValueAsync();
            singleton.setTourName(TourNameText.text);
            reference.Child(dbDetails.getTourDBName()).Child(singleton.getTourName()).SetRawJsonValueAsync(jsonData).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    throw new Exception("ERROR while appending values to database.");

                }
                else if (task.IsCompleted)
                {
                    Debug.Log("SUCCESS: DATA ADDED TO DATABASE");
                }
            });
            SceneManager.LoadScene("ManagerTourView");
        }
        catch (InvalidCastException e)
        {
            // Perform some action here, and then throw a new exception.
            ErrorMessage.text = e.Message;
            ErrorPanel.SetActive(true);
        }
        catch (Exception e)
        {
            // Perform some action here, and then throw a new exception.
            ErrorMessage.text = e.Message;
            ErrorPanel.SetActive(true);
        }
    }
}
