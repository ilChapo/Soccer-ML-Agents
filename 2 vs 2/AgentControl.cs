using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public enum Team
{
    Blue = 0,
    Purple = 1
}

public class AgentControl : Agent
{

    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private float speed;
    public float DashDistance = 5f;
    public GameObject ballSpawnArea;
    public GameObject ball;
    public GameObject goalArea;
    [HideInInspector]
    public Team team;
    [HideInInspector]
    public Vector3 goalAreaPos;
    [HideInInspector]
    public float ballPreviousDistance;


    private Vector3 inputVector;

    Rigidbody m_ballRb;
    BehaviorParameters m_BehaviorParameters;
    float distance;

    StatsRecorder statsRecorder;
    FieldController fieldController;


    float goals = 0f;
    float episodes = 1f;
    float m_ratio = 0;

    // Initialize is called when the gameObject gets enabled
    public override void Initialize() {

      fieldController = GetComponentInParent<FieldController>();

      playerBody = gameObject.GetComponent<Rigidbody>();
      m_ballRb = ball.GetComponent<Rigidbody>();
      goalAreaPos = goalArea.GetComponent<Transform>().position;

      ballPreviousDistance = 888888;

      // StatsRecorder statsRecorder = Academy.Instance.StatsRecorder;
      // statsRecorder.Add("Goal Ratio", goals/episodes);
      //this.MaxStep = 1000;


      m_BehaviorParameters = GetComponent<BehaviorParameters>();
        if (m_BehaviorParameters.TeamId == (int)Team.Blue)
        {
            team = Team.Blue;
        } else {
          team = Team.Purple;
        }

        //Debug.Log(team);


    }

     public override void CollectObservations(VectorSensor sensor)
     {
         //if(m_BehaviorParameters.TeamId == (int)Team.Blue) {sensor.AddObservation((float)m_BehaviorParameters.TeamId);}
     }

    public override void OnEpisodeBegin() {

      // if (team == Team.Blue) {
      //
      //   Academy.Instance.StatsRecorder.Add("Purple Ratio", m_ratio);
      //
      // } else {
      //
      //   Academy.Instance.StatsRecorder.Add("Blue Ratio", m_ratio);
      //
      // }
        //Academy.Instance.StatsRecorder.Add("Goal Ratio", m_ratio);
      //ballPreviousDistance = Vector3.Distance(m_ballRb.position, goalAreaPos);

      //Debug.Log("New episode");

    }

    public override void OnActionReceived(ActionBuffers actionBuffers) {
        // Move the agent using the action.
        MoveAgent(actionBuffers.DiscreteActions);

        // Penalty given each step to encourage agent to finish task quickly.
       AddReward(-0.001f);

       distance = Vector3.Distance(m_ballRb.position, goalAreaPos);

       //Debug.Log("Distancia segons PREVIOUS distance: " + ballPreviousDistance);
       //Debug.Log("Distancia segons distance: " + distance);

       if(((distance + 1f) < (ballPreviousDistance)) && (m_ballRb.velocity != Vector3.zero) && (fieldController.lastTeamTouched == (int)team)) {
            AddReward((float)fieldController.reward_shaper);
            ballPreviousDistance = distance;
            if((int)team == 0) {Debug.Log("bonus distancia team " + team + " distancia = " + distance + "reward shaper" + (float)fieldController.reward_shaper); }
          }
       if(((distance) > (ballPreviousDistance + 1f)) && (m_ballRb.velocity != Vector3.zero)) {
               AddReward((float)fieldController.reward_shaper*-1f);
               ballPreviousDistance = distance;
               if ((int)team == 0) {Debug.Log("bonus distancia team NEGATIU " + team + " distancia = " + distance + "reward shaper" + (float)fieldController.reward_shaper*-1f);}
             }
       //Debug.Log(GetCumulativeReward());
       // if (this.StepCount > 1000) {
       //
       //   playerBody.position = startingPosition;
       //   playerBody.rotation = startingRotation;
       //   m_fails++;
       //   //Debug.Log("FAIL" + m_fails);
       //   episodes++;
       //   m_ratio = goals/episodes;
       //   EndEpisode();
       // }



       // Debug.Log(GetCumulativeReward());
       // Debug.Log("steps -->"+ this.StepCount);
       // Debug.Log("goals" + goals);
       // Debug.Log("episodes " + episodes);
    }

    public void MoveAgent(ActionSegment<int> act)
    {

        var action1 = act[0];
        var action2 = act[1];
                //var horiz = 0;
        //var vert = 0;
        //Debug.Log("accion" + act[0]);
        switch (action1)
        {
            case 1:
                //vert = 1;
                //transform.Translate(Vector3.forward * speed * Time.deltaTime);
                playerBody.AddForce(transform.right * speed * 1.2f,
                    ForceMode.VelocityChange);
                break;
            case 2:
                //vert = -1;
                playerBody.AddForce(transform.right * -speed * 0.75f,
                    ForceMode.VelocityChange);
                break;
            case 3:
                //horiz = 1;
                playerBody.AddForce(transform.forward * -speed *0.75f,
                    ForceMode.VelocityChange);
                break;
            case 4:
                //horiz = -1;
                playerBody.AddForce(transform.forward * speed *0.75f,
                    ForceMode.VelocityChange);
                break;


        }
        switch (action2)
        {
            case 1:
                //horiz = 1;
                transform.Rotate(transform.up * 1f, Time.fixedDeltaTime * 700f);
                break;
            case 2:
                //horiz = -1;
                transform.Rotate(transform.up * -1f * 30, Time.fixedDeltaTime * 400f);
                break;


        }


        //Debug.Log("vert" + vert);
        //Debug.Log("horiz" + horiz);
        //inputVector = new Vector3(horiz * speed * 0.5f, playerBody.velocity.y * 1.3f, vert >= 0 ? vert * speed * 0.8f : vert * 0.6f * speed);
        //transform.LookAt(transform.position + new Vector3(inputVector.x, 0, inputVector.x));
        //transform.LookAt(transform.position + new Vector3(inputVector.x, 0, 0));

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
      //Debug.Log("lmao");
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[1] = 2;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActionsOut[1] = 1;
        }

        /*inputVector = new Vector3(Input.GetAxis("Horizontal") * speed * 0.5f, playerBody.velocity.y * 1.3f, Input.GetAxis("Vertical") >= 0 ? Input.GetAxis("Vertical") * speed * 0.8f : Input.GetAxis("Vertical") * 0.6f * speed);
        transform.LookAt(transform.position + new Vector3(-inputVector.z, 0, inputVector.x));*/
    }

    // Update is called once per frame
    void Update()
    {
        /*inputVector = new Vector3(Input.GetAxis("Horizontal") * speed * 0.5f,
        playerBody.velocity.y * 1.3f, Input.GetAxis("Vertical") >= 0 ? Input.GetAxis("Vertical") * speed * 0.8f : Input.GetAxis("Vertical") * 0.6f * speed);
        transform.LookAt(transform.position + new Vector3(-inputVector.z, 0, inputVector.x));
        if (Input.GetButtonDown("Dash"))
        {
            Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * playerBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * playerBody.drag + 1)) / -Time.deltaTime)));
            playerBody.AddForce(dashVelocity, ForceMode.VelocityChange);
        }*/

    }
    void FixedUpdate() { //happens not every frame,

        //playerBody.velocity = inputVector;
        // if (team == Team.Blue) Debug.Log("team" + team + " " + GetCumulativeReward());

    }
    // Detect when the agent hits the goal

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("out"))
        {
            AddReward(-1f);
            fieldController.EndOfEpisode(3,1);
        }

    }


    void OnCollisionEnter (Collision c) {
      if (c.gameObject.CompareTag("ball")) {
        //Debug.Log("xd");

        fieldController.lastTeamTouched = (int)team;

      }
    }


}
