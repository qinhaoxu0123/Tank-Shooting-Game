using System;
using UnityEngine;

[Serializable]
public class TankManager
{
	// this class is to manage various settings on a tank.
	// it works with the gamemanager class to control how the tanks behave 
	// and whether or not players have control of their tank in the 
	//different places of the game 

    public Color m_PlayerColor; // this is the color this tank will be tinted            
    public Transform m_SpawnPoint;  // the position and dir the tank will have when it spawns       
    [HideInInspector] public int m_PlayerNumber; // this specfiies which player this the manager for.            
    [HideInInspector] public string m_ColoredPlayerText; // a string that represents the players with their number colored to match their tank.
    [HideInInspector] public GameObject m_Instance; // a reference to the instance of the tank when it is created.         
    [HideInInspector] public int m_Wins;    // the number of wins this player has so far.                 


    private TankMovement m_Movement; // reference to tank's movement script, used to disable and enable control      
	private TankShooting m_Shooting; // reference to tank's shooting script, used to diaable and enable control

    private GameObject m_CanvasGameObject;// usded to diabled the world spcae UI during the starting and ending phased of each round.


    public void Setup()
    {
		// get reference to components
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

		// set the player numbers to be consistent across the scripts
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

		// create a string using the correct color that says player 1 etc based on the tank's color and the player's number.
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

		// get all of the renderers of the tank
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

		// go through all the renderers 
        for (int i = 0; i < renderers.Length; i++)
        {
			// set their material color to the color specfic to this tank
            renderers[i].material.color = m_PlayerColor;
        }
    }

	//used during the phased of the game wherer the player shouldn't be able to control their tanks.
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }

	// usded during the phsase of the game where the player should be able to control their tanks.
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }

	// usded at the start of each round to put the tank into its's default state.
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
