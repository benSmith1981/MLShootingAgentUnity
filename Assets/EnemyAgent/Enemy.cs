using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isDead = false;
	private int startingHealth = 100;

	private int currentHealth;
	public Vector3 startPosition;

    public UnityEngine.AI.NavMeshAgent agent;
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(player.position);
    }

    public void GetShot(int damage, ShootingAgent shooter) {
    	ApplyDamage(damage,shooter);
    }

    private void ApplyDamage(int damage,ShootingAgent shooter){
    	currentHealth -= damage;
    	if(currentHealth <= 0){
    		Die(shooter);
    	}
    }

    private void Die(ShootingAgent shooter){
        Debug.Log("Enemy died");
        isDead = true;
        shooter.numberDead += 1;

        gameObject.SetActive(false);
    	shooter.RegisterKill();
    }

    public void Respawn() {
        isDead = false;
        gameObject.SetActive(true);
    	transform.position = startPosition;
        currentHealth = startingHealth;
    }

    #region Debug 
    private void OnMouseDown(){
    	currentHealth -= 100;
        // if(currentHealth <= 0){
        //     Die(shooter);
        // }
    }
    #endregion
}
