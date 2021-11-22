using System.Threading;
using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections;
using System.Collections.Generic;
class Snake: Agent{
  bool dead = false;
  public GameObject snakeBody;
  public GameObject snakeHead;
  public GameObject foodObject;
  GameObject tempFoodObject;

  int[] food = new int[2];
  
  //size of grid
  static int width = 25;
  static int height = 25;

  //Direction
  int x = 0;
  int y = 0;
  int moveAmount = 1;

  GameObject[,] grid = new GameObject[width, height];
  SnakeQueue sq;
  char key = 'w';
  int length = 0;

//ai stuff
  int actionKey = 0;
  bool isheuristic = false;
  int snakeDirection = 0;
  private int squaresWalked = 0;
  void Start() {
    // this.sq = new SnakeQueue();
    // this.sq.add(new int[]{50,50}); //this is it's head
    // this.sq.add(new int[]{50,55});
    // this.sq.add(new int[]{50,60});
    // this.sq.add(new int[]{50,65}); //this is it's tail
    // placeFood();

  }

  //positioning agent environment and location
  public override void OnEpisodeBegin() {
    reset();
  }

  public void reset(){

    Debug.Log("Length "+length);
    this.sq = new SnakeQueue();
    Destroy(tempFoodObject);
    this.sq.add(new int[]{width/2,height/2}); //this is it's head
    this.sq.add(new int[]{width/2,(height/2)-1});
    this.sq.add(new int[]{width/2,(height/2)-2});
    this.sq.add(new int[]{width/2,(height/2)-3}); //this is it's tail
    placeFood();
    key = 'w';
    dead = false;
    squaresWalked = 0;

    length = 0;

  }

  void OnEpisodeEnd() {
    dead = true;
    SetFitness();
    EndEpisode();
  }
  void SetFitness() {
    SetReward(length - squaresWalked / (width * height));
  }

  public override void CollectObservations(VectorSensor sensor){
    SnakeNode currentNode = sq.head;
    //sensor.AddObservation(new Vector3(currentNode.data[0],0,currentNode.data[1]));
    // SnakeNode tail = sq.tail;
    // sensor.AddObservation(new Vector3(tail.data[0],0,tail.data[1]));

    //sensor.AddObservation(length);
      //transform.localPosition);
    //sensor.AddObservation(new Vector3(food[0],0,food[1]));
    //base.CollectObservations(sensor);

    //AddReward(-0.001f);
    List<int> encodedDirection = new List<int>{0,0,0,0};
    encodedDirection[snakeDirection] = 1;

    sensor.AddObservation(encodedDirection[0]);
    sensor.AddObservation(encodedDirection[1]);
    sensor.AddObservation(encodedDirection[2]);
    sensor.AddObservation(encodedDirection[3]);
      
  }

  
  //Everything AI done is turned into concrete action
  public override void OnActionReceived(ActionBuffers actions){
      Debug.Log(actions.DiscreteActions[0]);
      // Debug.Log(actions.DiscreteActions[1]);
      // Debug.Log(actions.DiscreteActions[2]);
      // Debug.Log(actions.DiscreteActions[3]);

      actionKey = actions.DiscreteActions[0];
      if(dead != true) {
        if(actionKey == 0) { 
          key = 'w';
          //logic ('w');
        }
        if(actionKey == 1) {
          key = 's'; 
          //logic ('s');
        }
        if(actionKey == 2) { 
          key = 'd';
          //logic ('a');
        }
        if(actionKey == 3) { 
          key = 'a';
          //logic ('d');
        }
        squaresWalked += 1;
        logic(key);
        draw();
    
      }

 
  }

  public override void Heuristic(in ActionBuffers actionsOut){
  // public override void Heuristic(float[] actionsOut){
    isheuristic = true;
    ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
    discreteActions[0] = snakeDirection;

    // if(Input.GetKeyDown(KeyCode.W)){
    //   key = 'w';
    //   discreteActions[0] = 0;

    // }
    // else if(Input.GetKeyDown(KeyCode.S)){
    //   key = 's';
    //   discreteActions[0] = 1;
    // }
    // else if(Input.GetKeyDown(KeyCode.D)){
    //   key = 'd';
    //   discreteActions[0] = 2;
    // }
    // else if(Input.GetKeyDown(KeyCode.A)){
    //   key = 'a';
    // }

    // discreteActions[2] = Input.GetKey(KeyCode.A) ? 2 : actionKey;
    // discreteActions[3] = Input.GetKey(KeyCode.D) ? 3 : actionKey;
  }


    // Update is called once per frame
  void Update()
  {
    if(dead != true && isheuristic) {
      if(Input.GetKeyDown(KeyCode.W) && key != 's'){
        key = 'w';
        snakeDirection = 0;
      }
      else if(Input.GetKeyDown(KeyCode.S) && key != 'w'){
        key = 's';
        snakeDirection = 1;
      }
      else if(Input.GetKeyDown(KeyCode.D) && key != 'a'){
        key = 'd';
        snakeDirection = 2;
      }
      else if(Input.GetKeyDown(KeyCode.A) && key != 'd'){
        key = 'a';
        snakeDirection = 3;
      }
      // logic(key);
      // draw();  
      //Time.timeScale = 100;

      //Thread.Sleep(500);
    }
  }
  
  // public Snake() {
  //   //Console.CursorVisible = false;
    
  // }

  public void placeFood() {
    System.Random rnd = new System.Random();
    food[0] = rnd.Next(0, height);
    food[1] = rnd.Next(0, width);
    //Instantiate(foodObject, new Vector3(food[0],0,food[1]), Quaternion.Euler(0, 180, 0));
    tempFoodObject = Instantiate(foodObject, new Vector3(food[0],0,food[1]), Quaternion.Euler(0, 180, 0));
    //Instantiate(foodObject, new Vector3(food[0],0,food[1]), Quaternion.Euler(0, 180, 0));
    tempFoodObject.gameObject.tag="food";     

  }

  public void draw() {
    //create empty grid
    //Console.Clear();


    //set snake position in grid
    SnakeNode head = sq.head;
    int count = 0;
    do {
      if(head.next == null){
        grid[head.data[0],head.data[1]] = snakeHead;
      } else {
        grid[head.data[0],head.data[1]] = snakeBody;
      }

      GameObject body = Instantiate(grid[head.data[0],head.data[1]], new Vector3(head.data[0],0,head.data[1]), Quaternion.Euler(0, 180, 0));       
      body.gameObject.tag="body"; 
      Destroy(body,0.1f);
      head = head.next;
    } while(head != null);

    

  }

  public void logic(char key) {

    //We need to add a node onto the tail (in the direction it is going) and delete the head of the queue (it is in reverse but it works)
    SnakeNode tail = sq.tail;
    int[] currentHead = sq.tail.data;
    
    x = 0;
    y = 0;
    //Depeding on key pressed, the direction, we need to update its vector to be moving +-1 in the y or x direction
    switch(key) {
      case 'w':
        x = -moveAmount;
        //check for out of bounds
        if((currentHead[0] + x) < 0) {
          currentHead[0] = grid.GetLength(1)-1;
        }
        break;
      case 's':
        x = moveAmount;
        if((currentHead[0] + x) > grid.GetLength(1)-1) {
          currentHead[0] = 0;
        }
        break;
      case 'a':
        y = -moveAmount;
        if((currentHead[1] + y) < 0) {
          currentHead[1] = grid.GetLength(0)-1;
        }
        break;
      case 'd':
        y = moveAmount;
        if((currentHead[1] + y) > grid.GetLength(0)-1) {
          currentHead[1] = 0;
        }
        break;
    }
    foodCollision(currentHead);
    //Add on our new head of our snake to the tail of the queue (it is in reverse)
    sq.add(new int[]{currentHead[0] + x, currentHead[1] + y});
    checkBodyCollision();

  }

  public void checkBodyCollision(){
    SnakeNode currentNode = sq.head;
    while(currentNode.next != null && !dead) {
      //Debug.Log("Check" + " currentNode.data[0] " + currentNode.data[0] + " currentNode.data[1] " + currentNode.data[1]);

      if(currentNode.data[0] == sq.tail.data[0] 
      && currentNode.data[1] == sq.tail.data[1]){
        Debug.Log("Body collision" + " sq.tail.data[0] " + sq.tail.data[0] + " sq.tail.data[1] " + sq.tail.data[1]);
        //Debug.Log("Body collision" + " currentNode.data[0] " + currentNode.data[0] + " currentNode.data[1] " + currentNode.data[1]);
        OnEpisodeEnd();
      }

      currentNode = currentNode.next;
    }
    Debug.Log("****************"); 
  }

  public void foodCollision(int[] currentHead) {
    //if snake collides with food
    if(currentHead[0] + x == food[0] 
    && currentHead[1] + y == food[1]){ 
      Destroy(tempFoodObject);
      length++;
      //AddReward(0.1f);
      placeFood();
    }else {
      //Remove the head to give appearance it is moving
      int[] deletedHead = sq.remove();
      //Debug.Log("deletedHead {0}", string.Join(",",deletedHead));
    }
  }

}

public class SnakeNode {
  public int[] data;
  public SnakeNode next;

  public SnakeNode(int[] data){
    this.data = data;
  }
}
public class SnakeQueue{

  public SnakeNode head;
  public SnakeNode tail;
  public bool isEmpty(){
    return head == null;
  }
  public int[] peak() {
    return head.data;
  }
  public void add(int[] data){
    SnakeNode node = new SnakeNode(data);
    if(tail != null) {
      tail.next = node; //change the pointer
    }
    tail = node; //update the tail of the queue
    //if the head is null (maybe the queue is empty, make the head the tail node)
    if(head == null) {
      head = node;
    }
  }
  public int[] remove() {
    int[] data = head.data;
    head = head.next; //remove from Queue
    //if head is null, set tail to null too, so nothing in list
    if(head == null){
      tail = null;
    }
    return data;
  }
}