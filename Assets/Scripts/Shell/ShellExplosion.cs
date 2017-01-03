using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask; // used to filter the explosion affects, this should be set to " players".
    public ParticleSystem m_ExplosionParticles;   // reference to the particles that will play on explosion.    
    public AudioSource m_ExplosionAudio;  //reference to the audio that will play on explosion            
    public float m_MaxDamage = 100f; // the amount of damage done if th explosion is center on a tank                  
    public float m_ExplosionForce = 1000f; // the amount of force added to a tank at the centre of the explosion.           
    public float m_MaxLifeTime = 2f;// the time in seconds before the shell is removed.                  
    public float m_ExplosionRadius = 5f;   // the max distance away from the explosion tanks can be and are still affected.           


    private void Start()
    {
		// if it isn't destroyed by then, destroy the shell after it is lifetime.
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.

		//collect all the colliders in a sphere from the shell/s current pos to a radius of the explosion radius.
		Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);

		//go through all the colliders...
		for (int i = 0; i < colliders.Length; i++) {
			// find their rigidbody.
			Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();

			// if they dont have a rigidbody , go on to the next collider.
			if (!targetRigidbody)
				continue;

			//add an explosion force.
			targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

			//find the tankhealth script associated with the rigidbody.
			TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> ();

			// if there is no tankhealth script attached to the gameobject , go on to the next collider.
			if (!targetHealth)
				continue;

			//calculate the amound of damge the target should take based on it is distance from the shell.
			float damage = CalculateDamage (targetRigidbody.position);

			//deal this damge to the tank.
			targetHealth.TakeDamage (damage);
		}

		// unparent the particles from the shell.
		m_ExplosionParticles.transform.parent = null;

		//play the particle sys
		m_ExplosionParticles.Play();

		//play the explosion sound affect.
		m_ExplosionAudio.Play();

		// once the particles have finished , destroy the gameobject they are on 
		Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);

		// destoy the shell
		Destroy(gameObject);

    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.

		//create a vector from te shell to the target.
		Vector3 explosionToTarget = targetPosition - transform.position;

		//calculate the distance from the shell to the target.
		float explosionDistance = explosionToTarget.magnitude;

		// cal the proportion of the max distance(explosionradius) the target is away/
		float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

		// cal damage as this proportion fo the max possible damage.
		float damage = relativeDistance * m_MaxDamage;

		// make sure the min damage is always 0.
		damage = Mathf.Max(0f, damage);

		return damage;
    }
}