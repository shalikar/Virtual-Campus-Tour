﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Newtonsoft.Json;
using UnityEngine.UI;

public class DeptDisplayLoc : MonoBehaviour
{
    public GameObject ContentPanel;
    public GameObject ListItemPrefab;
    ArrayList gameObjectsList = new ArrayList();
    ArrayList tours;
    DB_Details dbDetails;
    DatabaseReference reference;
    bool locationsDisplayed;
    bool updateLocationsDisplayed;
    DataSnapshot snapshot;

    private Dictionary<string, string> scheduleData;


    ArrayList locations;
    PSLocationArraySingleton singletonObject = PSLocationArraySingleton.Instance();
    public Text DepartmentTour;

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        tours = new ArrayList();
        scheduleData = new Dictionary<string, string>();
        locations = new ArrayList();

        dbDetails = new DB_Details();

        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(dbDetails.getDBUrl());

        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        getLocationData();
        locationsDisplayed = false;
        updateLocationsDisplayed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!locationsDisplayed && locations.Count > 0)
        {
            createLocationsList();
        }

        
        PSLocationArraySingleton s = PSLocationArraySingleton.Instance();
        
        if (!updateLocationsDisplayed && s.getUpdateStatus() == 1)
        {
            foreach (GameObject g in gameObjectsList)
            {
                
                g.Destroy();
            }
            gameObjectsList.Clear();
            updateLocationsList(s.getLocations());
        }
        if(gameObjectsList.Count >=1 && s.getUpdateStatus() == 1)
        {
            foreach (GameObject g in gameObjectsList)
            {
             
                g.Destroy();
            }
            gameObjectsList.Clear();
        }
    }

    void getLocationData()
    {
        Singleton s = Singleton.Instance();
        string scheduleName = s.getTourName();

        DepartmentTour.text = scheduleName + " Locations";
        

        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("error fetching data");
            }
            else if (task.IsCompleted)
            {
                // getting schedules for a particular user.


                snapshot = task.Result.Child(dbDetails.getTourDBName()).Child(scheduleName.ToString());
                
                
                scheduleData = JsonConvert.DeserializeObject<Dictionary<string, string>>(snapshot.GetRawJsonValue());

                foreach (KeyValuePair<string, string> schedule in scheduleData)
                {
                    this.locations.Add(new DeptLocation(schedule.Key));
                    this.tours.Add(schedule.Key);

                    //print(schedule.Key);
                }

                
                singletonObject.setLocations(tours);

                //ArrayList allLocations = singletonObject.getLocations();
                //foreach (string location in allLocations)
                //{
                //    print(location + "\n");
                //}

            }
        });
    }

    void createLocationsList()
    {
        foreach (DeptLocation s in locations)
        {
            ListItemPrefab.SetActive(true);
            GameObject newSchedule = Instantiate(ListItemPrefab);
            gameObjectsList.Add(newSchedule);
            DeptTourListitem controller = newSchedule.GetComponent<DeptTourListitem>();
            string name1 = s.Name;
            controller.Name.text = name1;

            newSchedule.transform.parent = ContentPanel.transform;
            newSchedule.transform.localScale = Vector3.one;
        }
        locationsDisplayed = true;
        

    }

    void updateLocationsList(ArrayList updateLocations)
    {
        foreach (string s in updateLocations)
        {
            // Destroy(ListItemPrefab);
            // Destroy(ContentPanel);
            ListItemPrefab.SetActive(true);
            GameObject newToursLocation = Instantiate(ListItemPrefab);
            
            DeptTourListitem controller = newToursLocation.GetComponent<DeptTourListitem>();
           
            controller.Name.text = s;

            newToursLocation.transform.parent = ContentPanel.transform;
            newToursLocation.transform.localScale = Vector3.one;
        }
        updateLocationsDisplayed = true;
        
    }


    //public void onDelete(Text locationName)
    //{
    //    locations.Remove(locationName.text);
    //}
}
