using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Character : MonoBehaviour
{
	[SerializeField] private Sprite[] sprites;
	[SerializeField] private Transform forceTransform;
	[SerializeField] public Transform pfProjectilePhysics;
	private SpriteRenderer spriteRendered;
    private RectTransform forceSpriteMask;
	public HealthBar healthBar;
	const int maxHealth = 10;
	int currentHealth;

    void Awake()
    {
		forceSpriteMask = forceTransform.Find("Mask").GetComponent<RectTransform>();
		spriteRendered = gameObject.GetComponent<SpriteRenderer>();
		HideForce();
		currentHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);
    }

	void Update(){

	}

   	public Transform Throw(Vector3 projectileSpawnPoint, Vector3 projectileAimPoint, float force, float angularVelocity) {
        Vector3 throwDir = (projectileAimPoint - projectileSpawnPoint).normalized;
		Transform projectileTransform = Instantiate(this.pfProjectilePhysics, projectileSpawnPoint, Quaternion.identity);
		projectileTransform.GetComponent<Projectile>().Setup(throwDir, force, angularVelocity);
		//Physics2D.IgnoreCollision(GetComponent<Collider2D>(), projectileTransform.GetComponent<Collider2D>());
		return projectileTransform;
	}

	public void TakeDamage(int damage)
	{
		currentHealth -= damage;
		healthBar.SetHealth(currentHealth);
		if(healthBar.GetHelth() <= 0.0f){ //ToDo
			//End screen
		}
	}

	public void SetSprite(int idx){
		spriteRendered.sprite = sprites[idx];
	}

	public void HideForce() {
		SetSprite(0);
        forceSpriteMask.anchoredPosition  = new Vector3(0.5f, 0.0f, 0.0f);
    }

    public void ShowForce(float force) {
		SetSprite(1);
		float forceNormalized = Mathf.Clamp01(force / CharacterController.MAX_FORCE);
        forceSpriteMask.anchoredPosition  = new Vector3(0.5f + 1.1f * forceNormalized, 0.0f, 0.0f);
    }
}