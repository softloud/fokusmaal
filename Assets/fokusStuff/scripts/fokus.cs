using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.IO;
using UnityEngine.UI; // not UI Elements
using System;

public class fokus : MonoBehaviour
{
        // Declare pages for navigatiopn
    public GameObject overlineOPage;
    public GameObject sigmaPage;
    public GameObject navigation;
    public GameObject lambdaPage;

    // Buttons 
    // overline o
    public Button phiButton;
    public Button psiButton;
    public Button thetaButton;
    public Button piButton;

    // Navigation buttons
    public Button overlineOButton;
    public Button sigmaButton;
    public Button lambdaButton;

    // Set colours
    public Color buttonStartColor = Color.black;
    public Color buttonStopColor = Color.white;
    private Button[] allButtons;

    // Timer variables
    private DateTime startTime;
    private DateTime stopTime;

     
    // Fokus elements this script acts on
    private Fokus fokusPhi;
    private Fokus fokusPsi;
    private Fokus fokusTheta;
    private Fokus fokusPi;

    // Define current button pressed and previous button pressed
    private Fokus currentFokus;
    private Fokus previousFokus;

    // Data variables to be used across functions

    // Dataset of observations    
    // public DataTable obsDat;
    // Should this be on start?
        // Get data
        // First version: write new table each time
        // Create a new datatable (change to if file exists, don't and read later)
    // private System.Data.DataSet dataSet;  // for relational datatables  
    public System.Data.DataTable obsDat = new DataTable("ObservationalData"); 
    // What is the purpose of name string arg?
    DataRow obsRow;

    // Functions needed by Start() 

    // Setting the button color using the Image.color property
    void SetButtonColor(Button button, Color color)
    {
        var image = button.gameObject.GetComponent<Image>();
        image.color = color;
    }

    void PageButtonsOff(GameObject page)
    {
        allButtons = page.GetComponentsInChildren<Button>();
        foreach (var button in allButtons)
        {
            SetButtonColor(button, buttonStopColor);
        }

    }

     void MakeObsDat()
    {
        DataColumn column;

        // Create fokus column
        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "fokus";
        obsDat.Columns.Add(column);

        // Create timestamp column
        column = new DataColumn();
        column.DataType = System.Type.GetType("System.DateTime");
        column.ColumnName = "start";
        obsDat.Columns.Add(column);
        
        // Create timepoint column
        column = new DataColumn();
        column.DataType = System.Type.GetType("System.DateTime");
        column.ColumnName = "stop";
        obsDat.Columns.Add(column);

    }

    // Start is called before the first frame update
    void Start()
    {

        // Create observational dataset
        MakeObsDat();

        // Set timers as default page 
        overlineOPage.SetActive(true);
        navigation.SetActive(true);
        sigmaPage.SetActive(false);
        lambdaPage.SetActive(false);

        // Navigation UI
        // Set all button colors to the default start and stop colors
        PageButtonsOff(navigation);
        SetButtonColor(overlineOButton, buttonStartColor);

        // Set timer UI to off
        PageButtonsOff(overlineOPage);

        // Create fokus objects for each button
        fokusPhi = new Fokus("phi", phiButton);       
        fokusPsi = new Fokus("psi", psiButton);
        fokusTheta = new Fokus("theta", thetaButton);
        fokusPi = new Fokus("pi", piButton);
       
        // Initialise buttons to none
        currentFokus = null;
        previousFokus = null;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Navigation
    // Pages of app defined by game objects
    // Navigation is always shown
    // Only one other panel is shown at a time:
    // overlineOPage: timers that start and stop fokus
    // sigmaPage: Summary analysis
    // lambdaPage: User input parameters, such as meetings mins

    public void navSigmaCmd()
    {
        // Turn off other stuff
        overlineOPage.SetActive(false);
        lambdaPage.SetActive(false);
        PageButtonsOff(navigation);

        // Turn on sigma
        sigmaPage.SetActive(true);
        SetButtonColor(sigmaButton, buttonStartColor);
    }

    public void navOverlineOCmd()
    {
        // Turn off other stuff
        sigmaPage.SetActive(false);
        lambdaPage.SetActive(false);
        PageButtonsOff(navigation);

        // Turn on overlineO
        overlineOPage.SetActive(true);
        SetButtonColor(overlineOButton, buttonStartColor);
    }

    public void navLambdaCmd()
    {
        // Turn off other stuff
        overlineOPage.SetActive(false);
        sigmaPage.SetActive(false);
        PageButtonsOff(navigation);

        // Turn on lambda
        lambdaPage.SetActive(true);
        SetButtonColor(lambdaButton, buttonStartColor);
    }

    // Timer buttons
    // Clicking fokus button starts or stops timer

    // Functions needed for timer button commands

    // Record timer
    public void TimerRecord(Fokus fokus)
    {
        obsRow = obsDat.NewRow(); 
        obsRow["fokus"] = fokus.text;
        obsRow["start"] = startTime;
        obsRow["stop"] = stopTime;
        obsDat.Rows.Add(obsRow);
        obsDat.WriteXml("Assets/fokusStuff/data/obsdat.xml");
    }
    
    // Commands for starting and stopping fokus sessions
    public void StartFokusSession(Fokus fokus)
    {
        SetButtonColor(fokus.button, buttonStartColor);
        
        // set start time to now 
        startTime = DateTime.Now;
    }

    public void StopFokusSession(Fokus fokus)
    {
        stopTime = DateTime.Now;
        SetButtonColor(fokus.button, buttonStopColor);
        TimerRecord(fokus);
    }

    void updateTimerState()
    {
        // if there are no active timers
        // record start observation and set previous button pressed
        if (previousFokus is null) 
        {
            StartFokusSession(currentFokus);
            previousFokus = currentFokus;
            return;
        }

        // if the button is the same as the button pressed before
        // record stop observation and set previous button pressed to None, i.e., no active tiemr
        if (currentFokus == previousFokus && currentFokus != null) 
        {
            StopFokusSession(currentFokus);
            previousFokus = null;
            return;
        }

        //if another button was pressed, record a stop on that button and set new button to active
        if (currentFokus != previousFokus) 
        {
            StopFokusSession(previousFokus);
            StartFokusSession(currentFokus);
            previousFokus = currentFokus;
            return;
        }
    }

    // Timer button commands
    public void timerPhiCmd() 
    {
        currentFokus = fokusPhi;
        updateTimerState();
    }

    public void timerPsiCmd()
    {
        currentFokus = fokusPsi;
        updateTimerState();
    }

    public void timerThetaCmd()
    {
        currentFokus = fokusTheta;
        updateTimerState();
    }

    public void timerPiCmd()
    {
        currentFokus = fokusPi;
        updateTimerState();
    }



    // Data engineering and analysis 
}


// Fokus data type contains all associated information for a fokus session
public class Fokus
{
    public string text;
    public Button button;

    public Fokus(string _text, Button _button)
    {
        text = _text;
        button = _button;
    }
}