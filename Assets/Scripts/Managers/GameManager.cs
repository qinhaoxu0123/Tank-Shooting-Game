using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;  // the number of round a single player has to win to win the game      
    public float m_StartDelay = 3f; // the delay between start of roundstarting and roundplayoing phase        
    public float m_EndDelay = 3f;  // the delay between the end of roundplaying and roundending phases         
    public CameraControl m_CameraControl;  // reference to the cameracontrol script for contol during different phases. 
    public Text m_MessageText;     // reference to the overlay text to diaplay winning text , etc.         
    public GameObject m_TankPrefab;  // reference to the prefab the players will control/       
    public TankManager[] m_Tanks;  //  a collection of mamagers for enabling and disabling different aspects of the tanks.         


    private int m_RoundNumber;  // which round the game is currently one        
    private WaitForSeconds m_StartWait;   // used to have a delay whilsit the round starts
    private WaitForSeconds m_EndWait;   // usded to have a delay whilst the round or game ends.    
    private TankManager m_RoundWinner; // reference to the winner of the current round. used to make an announcement of who won.
    private TankManager m_GameWinner;  // reference to the winner of the game. used to make an annoucement of who won.     


    private void Start()
    {
		// create the delays so they only have to be made once.
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();
		// once the tanks have been created and the camera is using them as tartgets, start the game 
        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
		// for all the tanks
        for (int i = 0; i < m_Tanks.Length; i++)
        {
			// create them, set their players number and references needed for control.
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
		// create a collection of transform the same size as the number of tanks.
        Transform[] targets = new Transform[m_Tanks.Length];

		// for each of these transforms...
        for (int i = 0; i < targets.Length; i++)
        {
			// set it to the appropriate tnak transform
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

		// these are the target the camera should follow
        m_CameraControl.m_Targets = targets;
    }

	// this is called from start and will run each phase of the game one after another 
    private IEnumerator GameLoop()
    {
		// start off by running the "roundstaring" coroutine but dont return until it is finished 
        yield return StartCoroutine(RoundStarting());


	// once the roundstarting corountine is finished , run the " roundplaying coroutine but dont return until it is finished
        yield return StartCoroutine(RoundPlaying());

	// once execution has returned here, run the " roundedning corountine, again dont return until it is finshed 
        yield return StartCoroutine(RoundEnding());

		// this code is not run until roundending has finished, at which point, check if a game winner has been found
        if (m_GameWinner != null)
        {
			// if there is game winner , restart the level
            Application.LoadLevel(Application.loadedLevel);
        }
        else
        {
			// if there is not a winner yet, restart this coroutine so the loop continues.
			// ps: this coroutine does not yield. this means that the current version of the gameloop will end.
            StartCoroutine(GameLoop());
        }
	}


    private IEnumerator RoundStarting()
    {
		// as soon as the round starts reset the tanks and make sure they can't move.
		ResetAllTanks();
		DisableTankControl ();

		// snap the camera's zoom and postion to something appropriate for the reset tanks.
		m_CameraControl.SetStartPositionAndSize();

		// increment the round number and display text showing the players what rount it is 
		m_RoundNumber++;
		m_MessageText.text = "ROUND " + m_RoundNumber;


		// wait for the specified lenth of time until yeilding control back to the game loop
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
		// as soon as the round begins playing let the players control the tanks.
		EnableTankControl();

		// clear the text from the screen.
		m_MessageText.text = string.Empty;

		//while there is not one tank left...
		while (!OneTankLeft ()) {
			yield return null;
		}
    }


    private IEnumerator RoundEnding()
    {
		// stop tanks from moving
		DisableTankControl();


		// clear the winner from the previous round.
		m_RoundWinner= null;

		// see if there is a winner now the round is over.
		m_RoundWinner = GetRoundWinner ();

		// if there is winner, increment their score
		if (m_RoundWinner != null)
			m_RoundWinner.m_Wins++;

		// now the winner's score has been incremented, see if someone has one the game 
		m_GameWinner = GetGameWinner();

		// get a msg based on the scores and whether or not there is a game winner and display it.
		string message = EndMessage ();
		m_MessageText.text = message;

		// wait for the specified lenght of time until yieding control back to the game loop.
        yield return m_EndWait;
    }

	// this is used to check it there is one or fewer tnaks remaning and thus the round should end.
    private bool OneTankLeft()
    {
		// start the count of tanks left at zero.
        int numTanksLeft = 0;

		// go through all the tanks
        for (int i = 0; i < m_Tanks.Length; i++)
        {
			// and if they are active, increment the counter
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }
		// if there are one or fewer tanks remaining return true, otherwise return false.
        return numTanksLeft <= 1;
    }

	// this function is to find out if there is a winner of the round.
	// this function is called with the assumption that 1 or fewer tanks are currently active.

    private TankManager GetRoundWinner()
    {
		// go through all the tanks
        for (int i = 0; i < m_Tanks.Length; i++)
        {
			// .. and if one of them is active, it is the winner so return it.
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }
		// if none of the tanks are active it is a draw so return null

        return null;
    }

	// this function is to find out if there is a winner of game.
    private TankManager GetGameWinner()
    {
		// go through all the tanks
        for (int i = 0; i < m_Tanks.Length; i++)
        {
			// and if one of them has enough rounds to win the game, return it.
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }
		// if no tanks have enough rounds to win, return null.
        return null;
    }

	// returns a string msg to display at the end of each round.
    private string EndMessage()
    {
		//by default when a round ends there are no winner so the default end message is a draw.
        string message = "DRAW!";
	    
		// if there is a winner then change the msg to reflect that.
        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

		// add some line breka after the intial msg.
        message += "\n\n\n\n";

		// go through all the tnaks and add each of their scores to the msg.
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

		// if there is a game winner, change the entire message to reflect that.
        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }

	// this function is usded to turn all the tanks back on and reset their pos and proproties.
    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}