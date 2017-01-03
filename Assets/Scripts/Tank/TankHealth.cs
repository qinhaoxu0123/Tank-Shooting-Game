using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f; // the intial health that the tank start.          
    public Slider m_Slider;     // the slider to represent how much health the tank has                   
    public Image m_FillImage;         // the image compoent of the slider            
    public Color m_FullHealthColor = Color.green;  // green color bar will full health
    public Color m_ZeroHealthColor = Color.red;    // red color bar with no health
    public GameObject m_ExplosionPrefab; // a prefab that will be instantiated in awake, then used whenever the tank dies
    
    
    private AudioSource m_ExplosionAudio;       // the audio source will play when the tank explodes   
    private ParticleSystem m_ExplosionParticles;  // the particle sys will play when the tank destroyed 
    private float m_CurrentHealth;  // tank heath 
    private bool m_Dead;     // has the tank been reduced beyond zero heath yet?       


    private void Awake()
    {
		// instantiated the explosion prefab and get a reference to the particle sys on it.
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

		// get a reference to the audio source on the instantiated prefab
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

		// disable the prefab so it can be activated when its required
        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
		// when the tank is enabled reset the tank's health and whether or not it's dead.
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

		// updated the health slider's value and color
        SetHealthUI();
    }
    

    public void TakeDamage(float amount)
    {
     
		// Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
		//reduce current healht by the amount of damge done

		m_CurrentHealth -= amount;

		//change the UI elements appropriately
		SetHealthUI ();

		//if the current heath is at or below zero and it has not yet been registered, call OnDeath.
		if (m_CurrentHealth <= 0f && !m_Dead) {
			OnDeath ();
		}
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
		// se the slider's value approviately
		m_Slider.value =m_CurrentHealth;

		//interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
		m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
		// set the flag so that this function is only called once.
		m_Dead = true;

		//move the instantiated explosion prefab to the tank's pos and turn it on.
		m_ExplosionParticles.transform.position = transform.position;
		m_ExplosionParticles.gameObject.SetActive (true);

		// play the particle system of the tank exploding.
		m_ExplosionParticles.Play();

		// paly the tank explosdion sound effect.
		m_ExplosionAudio.Play();

		// turn the tank off
		gameObject.SetActive(false);

    }
}