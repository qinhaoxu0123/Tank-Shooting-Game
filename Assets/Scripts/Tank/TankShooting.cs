using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;   // used to identify the different players.    
    public Rigidbody m_Shell;      // prefab of the shell      
    public Transform m_FireTransform;    // a child of the tank where the shells are spawned 
    public Slider m_AimSlider;        // a child of the tank that displays the current lanuch force.   
    public AudioSource m_ShootingAudio;  // reference to the audio sources to play the shooting audio.ps: different to the movement audio source
    public AudioClip m_ChargingClip;  // audio that plays when each shot is charging up.   
    public AudioClip m_FireClip;   // audio that plays when each shot is fired      
    public float m_MinLaunchForce = 15f; // the force given to the shell if the fire button is not held.
    public float m_MaxLaunchForce = 30f; // the force given to the sehll if the fire button is held for the max charge time.
    public float m_MaxChargeTime = 0.75f; // how  long the shell can charge for before it is fired at max force

    
    private string m_FireButton;       // the input axis that is used for launching shells.  
    private float m_CurrentLaunchForce;  // the force that will be given to the shell when the fire button is released 
    private float m_ChargeSpeed;   // how fast the launch force increases, based on the max charge time.      
    private bool m_Fired;       // whether or not the shell has been launched with this button press.         


    private void OnEnable()
    {
		// when the tank is turned on, reset the launch force and the UI
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
		//the fire axis is baed on the player number
        m_FireButton = "Fire" + m_PlayerNumber;

		// the rate that the launch force charges up is the range of possible forces by the max charge time.
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }
    

    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
		// the slider should have a default value of the min launch force.
		m_AimSlider.value = m_MinLaunchForce;

		// if the max force has been exceeded and the sehll has not yet been launched.
		if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) {
		
			//use the max forrce and launch the shell
			m_CurrentLaunchForce = m_MaxLaunchForce;
			Fire ();
		
		} else if (Input.GetButtonDown (m_FireButton)) {
		
			// reset the fired flag and reset the launch force
			m_Fired = false;
			m_CurrentLaunchForce = m_MinLaunchForce;

			//change the clip to the charging clip and start it playing;
			m_ShootingAudio.clip = m_ChargingClip;
			m_ShootingAudio.Play ();
		}	
		// otherwise, if the fire button is being held and the sehll has not been launched yet....
		else if (Input.GetButton (m_FireButton) && !m_Fired) {
		
			// increment the launch force and update the slider
			m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

			m_AimSlider.value = m_CurrentLaunchForce;
		
		}

		//otherwise m if the fire button is released and the sehll has not been launched yet..
		else if (Input.GetButtonUp (m_FireButton) && !m_Fired) {
		
			//launch the shell;
			Fire();
		
		}
			
    }


    private void Fire()
    {
        // Instantiate and launch the shell.

		// set the fired flag so only fire is only call once;
		m_Fired = true;

		//create an instance of the shell and store a reference to its's rigidbody.
		Rigidbody shellInstance =
			Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

		// set the shell's velocity to the launch force in the fire pos's forward dir.
		shellInstance.velocity = m_CurrentLaunchForce *  m_FireTransform.forward; ;

		//change the clip to the firing clip and play it.
		m_ShootingAudio.clip = m_FireClip;
		m_ShootingAudio.Play ();

		//reset the launch force, this is a precaution in case of missing button events.
		m_CurrentLaunchForce = m_MinLaunchForce;


    }
}