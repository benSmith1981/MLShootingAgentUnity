using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
// using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;

public class ShootingAgent : Agent
{
    public Text winsText;
    public int wins = 0;
    public Text lossesText;
    public int losses = 0;

    public GameObject[] enemies;
    public int numberDead = 0;

	public Transform shootingPoint;
    public int minStepsBetweenShots = 50;
    public int damage = 100;

    private bool shotAvailable = true;
    private int stepsUntilShotAvailable = 0;

    private Vector3 StartPosition;
    public Rigidbody rb;

    // [SerializeField] LineRenderer lineRenderer;
    // private LineRenderer line;
    [SerializeField] GameObject lineRenderer;
    private GameObject line;
//Happens between awake and start function. Agent connects to behaviours sensors and the academy.
//Academy is globally accessible singleton. Communicates to external python process
    public override void Initialize(){
        StartPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

//AI collects observations on everything you ai needs to make a useful decision
    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(transform.localRotation);
        // foreach(GameObject enemy in enemies){
        //     sensor.AddObservation(enemy.transform.localPosition);
        // }
        // base.CollectObservations(sensor);
        AddReward(-0.001f);
        
    }

//Everything AI done is turned into concrete action
    public override void OnActionReceived(float[] ContinuousActions){
        // Debug.Log(actions.ContinuousActions[0]);
        if(Mathf.RoundToInt(ContinuousActions[0]) >= 1){
            Shoot();
        }
        // float moveX = ContinuousActions[1];
        float moveZ = ContinuousActions[2];
        float rotateY = ContinuousActions[1];

        float moveSpeed = 1f;
        float rotateSpeed = 300f;
        transform.localPosition += moveZ * transform.right * Time.deltaTime * moveSpeed;
        //transform.Translate(moveSpeed * (moveZ > 0 ? Vector3.forward : Vector3.back) * Time.deltaTime);
        //transform.localPosition = new Vector3(0, 0, moveZ) * Time.deltaTime * moveSpeed;
        transform.localRotation = Quaternion.AngleAxis(rotateY * Time.deltaTime * rotateSpeed, transform.up) * transform.rotation;

    }
    public void Shoot() {
        if (!shotAvailable) { return; }
        int layerMask = 1 << LayerMask.NameToLayer("enemy");
        int layerMask2 = 1 << LayerMask.NameToLayer("wall");

        var direction = transform.right;
        Debug.DrawRay(shootingPoint.position, direction * 200f, Color.green, 2f);
        line = Instantiate(lineRenderer);
        line.GetComponent<LineRenderer>().SetPositions(new Vector3[] { shootingPoint.position, shootingPoint.position+direction*100});
        Destroy(line,0.2f);
  
        if(Physics.Raycast(shootingPoint.position, direction, out var hit, 100f, layerMask)){
        	Debug.Log("Enemy Shoot");
            hit.transform.GetComponent<Enemy>().GetShot(damage, this);
        } else {
            AddReward(-0.001f);
        }


        shotAvailable = false;
        stepsUntilShotAvailable = minStepsBetweenShots;
    }


    private void FixedUpdate() {
        if (!shotAvailable) { 
            stepsUntilShotAvailable --;
            if(stepsUntilShotAvailable <= 0){
                shotAvailable = true;
            }
        }

    }


    private void OnMouseDown(){
    	Shoot();
    }

//control manually
    //public override void Heuristic(in ActionBuffers actionsOut){
    public override void Heuristic(float[] actionsOut){

    	//ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
    	//continuousActions[0] = Input.GetAxisRaw("Horizontal");
    	//continuousActions[1] = Input.GetAxisRaw("Vertical");

    	actionsOut[0] = Input.GetKey(KeyCode.P) ? 1f : 0f;
    	actionsOut[1] = Input.GetAxisRaw("Horizontal");
    	actionsOut[2] = Input.GetAxisRaw("Vertical");

    }

//positioning agent environment and location
	public override void OnEpisodeBegin() {
    	// transform.localPosition = new Vector3(0, 0, 0);
    	// transform.localPosition = new Vector3(Random.Range(-3f, +3f), 0, Random.Range(-3f, +3f));
    	// targetTransform.localPosition = new Vector3(Random.Range(-3f, +3f), 0, Random.Range(-3f, +3f));
		transform.position = StartPosition;
		rb.velocity = Vector3.zero;
        //AddReward(-0.1f);
		shotAvailable = true;
        foreach(GameObject enemy in enemies){
            enemy.GetComponent<Enemy>().Respawn();
            // enemy.GetComponent<Enemy>().isDead = false;
            // enemy.GetComponent<Enemy>().transform.position = enemy.GetComponent<Enemy>().startPosition;
        }

	}


    public void RegisterKill(){
        AddReward(0.1f); //reward for killing

//DON"T COUNT DEAD ENEMIES... destroy game object look for active objects
        // foreach(GameObject enemy in enemies){
        //     if(enemy.GetComponent<Enemy>().isDead){
        //         numberDead += 1;
        //     } 
        // }
        Debug.Log("Enemy dead: "+numberDead + " of "+enemies.Length);

        if(numberDead >= enemies.Length) { 
            Debug.Log("All Enemy dead!!");
            wins+=1;
            winsText.text = "Wins: "+wins;

            numberDead = 0;
            AddReward(1.0f); //reward for killing all
            EndEpisode();
        }
    }


    private void OnTriggerEnter(Collider other){

    	// if(other.gameObject.tag == "wall"){
			
     //        AddReward(-0.1f); //sets single reward for a collision
	    // 	// EndEpisode();
    	// }
    	if(other.gameObject.tag == "enemy"){
            Debug.Log(" Player died");
            losses += 1;
            lossesText.text = "Losses: "+losses;

            AddReward(-1.0f); //sets single reward for a collision
            EndEpisode();
    	}

    }

    // void OnCollisionEnter(Collision collision)
    // {
    //                 Debug.Log(collision.collider.tag+ "Collision");

    //     if(collision.gameObject.CompareTag("enemy")){
    //         Debug.Log(collision.collider.tag+ " Player died");
    //         losses += 1;
    //         lossesText.text = "Losses: "+losses;

    //         AddReward(-1.0f); //sets single reward for a collision
    //         EndEpisode();
    //     }

    // }
}
