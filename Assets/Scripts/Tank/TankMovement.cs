using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1; //used to identify which tank belongs to which player. This is set by this tank's manager.        
    public float m_Speed = 12f; // how fast the tank move forward and backward.           
    public float m_TurnSpeed = 180f;// how fast the tank turns in degrees per second.       
    public AudioSource m_MovementAudio;  //reference to the audio source used to play engine sounds. PS: different from the shooting audio source 
    public AudioClip m_EngineIdling;  // audio to play when the tank isn't moving at all.     
    public AudioClip m_EngineDriving;  // audio to play when the tank is moving.    
    public float m_PitchRange = 0.2f;// the amound by which the pitch of the engine noise can vary.

    
    private string m_MovementAxisName;   // the name of the input axis for moving forward and backward.  
    private string m_TurnAxisName;   // the name of the input axis for turning      
    private Rigidbody m_Rigidbody; // reference used to move the tank        
    private float m_MovementInputValue; // the current value of the movement input.   
	private float m_TurnInputValue; // the current value of the turn input.        
    private float m_OriginalPitch;  // the pitch of the audio source at the start of the scene.       


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

    }


    private void OnEnable ()
    {
		// when the tank is turned on, make sure it's not kinematcs which means it does not have the force apply onto the tank
        m_Rigidbody.isKinematic = false;

		// also to reset the input values when the tank is turned on.
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
		// when the tank is off, set it to kinematic so it stops moving.
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
		// the axes name are based on player number 
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

		// store the oringinal pitch of the audio source
        m_OriginalPitch = m_MovementAudio.pitch;
    }
    

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.

		//store the value of both input axes
		m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
		m_TurnInputValue = Input.GetAxis (m_TurnAxisName);

		EngineAudio ();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

		//if there is no input(the tank is stationary)....

		if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f) {
			//... and if the audio source is currently playing the driving clip
			if (m_MovementAudio.clip == m_EngineDriving) {
				//..CHANGE THE clip to idling and play it.

				m_MovementAudio.clip = m_EngineIdling;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play ();
			}
		} else {
			//otherwise if the tank is moving and if the idling clip currenly playing..
			if (m_MovementAudio.clip == m_EngineIdling) {
				//...change the clip to driving and play
				m_MovementAudio.clip = m_EngineDriving;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play ();
			}
		}

    }


    private void FixedUpdate()
    {
        // Move and turn the tank.
		// adjust the rigidbodies position and orientation in FixedUpdate.
		Move ();
		Turn ();
		
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
		//create a vector in the direction the tank is facing with a magnitude based on the input , speed and the time between frames.
		Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

		// apply this movement to the rigidbody's position.
		m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
		//determine the number of degrees to be turned based on the input, speed and time between frames.

		float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

		//make this into a rotation in the y axis;
		Quaternion turnRotation = Quaternion.Euler(0f, turn,0f);

		// applying this rotation to the rigidbody's rotation.
		m_Rigidbody.MoveRotation( m_Rigidbody.rotation * turnRotation);
    }
}