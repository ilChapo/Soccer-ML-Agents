using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentControl : Agent
{

    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private float speed;
    public float DashDistance = 5f;
    public GameObject ballSpawnArea;
    public GameObject ball;
    public GameObject goalArea;


    private Vector3 inputVector;
    private int m_score = 0;
    private TextMesh m_agentScore;
    private Vector3 m_startingPosition;
    private Quaternion m_startingRotation;

    Bounds m_BallSpawnAreaBounds;
    Rigidbody m_ballRb;
    Vector3 m_goalAreaPos;
    float m_previousDistance;
    float distance;
    int m_fails = 0;
    float goals = 0f;
    float episodes = 1f;
    float m_ratio = 0;
    float m_boundsRegulator;

    StatsRecorder statsRecorder;
    EnvironmentParameters m_ResetParams;

    // Initialize is called when the gameObject gets enabled
    public override void Initialize() {

      playerBody = GetComponent<Rigidbody>();
      m_BallSpawnAreaBounds = ballSpawnArea.GetComponent<Collider>().bounds;
      m_ballRb = ball.GetComponent<Rigidbody>();
      m_goalAreaPos = goalArea.GetComponent<Transform>().position;

      ballSpawnArea.SetActive(false);
      m_startingPosition = playerBody.position;
      m_startingRotation = playerBody.rotation;
      m_previousDistance = 888888;

      m_agentScore = GameObject.Find("agentscore").GetComponent<TextMesh>();
      m_agentScore.text = m_score.ToString();
      // StatsRecorder statsRecorder = Academy.Instance.StatsRecorder;
      // statsRecorder.Add("Goal Ratio", goals/episodes);
      this.MaxStep = 1000;

      m_ResetParams = Academy.Instance.EnvironmentParameters;


    }

    public override void OnEpisodeBegin() {

        m_boundsRegulator = m_ResetParams.GetWithDefault("my_environment_parameter", 1f);
        ResetBall(m_ballRb);
        Academy.Instance.StatsRecorder.Add("Goal Ratio", m_ratio);
      //m_previousDistance = Vector3.Distance(m_ballRb.position, m_goalAreaPos);

    }

    public override void OnActionReceived(ActionBuffers actionBuffers) {
        // Move the agent using the action.
        MoveAgent(actionBuffers.DiscreteActions);

        // Penalty given each step to encourage agent to finish task quickly.
       AddReward(-0.01f);

       distance = Vector3.Distance(m_ballRb.position, m_goalAreaPos);

       //Debug.Log("Distancia segons PREVIOUS distance: " + m_previousDistance);
       //Debug.Log("Distancia segons distance: " + distance);

       if(((distance + 1f) < (m_previousDistance)) && (m_ballRb.velocity != Vector3.zero)) {
         AddReward(10f);
         //Debug.Log("MES A PROP");
         m_previousDistance = distance;
       }
       //Debug.Log(GetCumulativeReward());
       if (this.StepCount > 1000) {

         playerBody.position = m_startingPosition;
         playerBody.rotation = m_startingRotation;
         m_fails++;
         //Debug.Log("FAIL" + m_fails);
         episodes++;
         m_ratio = goals/episodes;
         EndEpisode();
       }



       // Debug.Log(GetCumulativeReward());
       // Debug.Log("steps -->"+ this.StepCount);
       // Debug.Log("goals" + goals);
       // Debug.Log("episodes " + episodes);
    }

    public void MoveAgent(ActionSegment<int> act)
    {

        var action = act[0];
        var horiz = 0;
        var vert = 0;
        //Debug.Log("accion" + act[0]);
        switch (action)
        {
            case 1:
                vert = 1;
                //transform.Translate(Vector3.forward * speed * Time.deltaTime);
                playerBody.AddForce(transform.right * speed * 1.1f,
                    ForceMode.VelocityChange);
                break;
            case 2:
                vert = -1;
                playerBody.AddForce(transform.right * -speed * 0.75f,
                    ForceMode.VelocityChange);
                break;
            case 3:
                //horiz = 1;
                playerBody.AddForce(transform.forward * -speed *0.625f,
                    ForceMode.VelocityChange);
                break;
            case 4:
                //horiz = -1;
                playerBody.AddForce(transform.forward * speed *0.625f,
                    ForceMode.VelocityChange);
                break;
            case 5:
                //horiz = 1;
                transform.Rotate(transform.up * 1f, Time.fixedDeltaTime * 400f);
                break;
            case 6:
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
        else if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[0] = 6;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[0] = 5;
        }

        /*inputVector = new Vector3(Input.GetAxis("Horizontal") * speed * 0.5f, playerBody.velocity.y * 1.3f, Input.GetAxis("Vertical") >= 0 ? Input.GetAxis("Vertical") * speed * 0.8f : Input.GetAxis("Vertical") * 0.6f * speed);
        transform.LookAt(transform.position + new Vector3(-inputVector.z, 0, inputVector.x));*/
    }

    // Update is called once per frame
    void Update()
    {
        /*inputVector = new Vector3(Input.GetAxis("Horizontal") * speed * 0.5f, playerBody.velocity.y * 1.3f, Input.GetAxis("Vertical") >= 0 ? Input.GetAxis("Vertical") * speed * 0.8f : Input.GetAxis("Vertical") * 0.6f * speed);
        transform.LookAt(transform.position + new Vector3(-inputVector.z, 0, inputVector.x));
        if (Input.GetButtonDown("Dash"))
        {
            Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * playerBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * playerBody.drag + 1)) / -Time.deltaTime)));
            playerBody.AddForce(dashVelocity, ForceMode.VelocityChange);
        }*/

    }
    void FixedUpdate() { //happens not every frame, 

        //playerBody.velocity = inputVector;

    }
    //Reset the orange block position
    public void ResetBall(Rigidbody ballRb)
    {
        ballRb.transform.position = GetRandomSpawnPosBall();
        m_previousDistance = Vector3.Distance(ballRb.transform.position, m_goalAreaPos);
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
    }

    public Vector3 GetRandomSpawnPosBall()
    {
        var randomPosX = Random.Range(-m_BallSpawnAreaBounds.extents.x * m_boundsRegulator,
            m_BallSpawnAreaBounds.extents.x * m_boundsRegulator);
        var randomPosZ = Random.Range(-m_BallSpawnAreaBounds.extents.z * m_boundsRegulator,
            m_BallSpawnAreaBounds.extents.z * m_boundsRegulator);

        var randomSpawnPos = ballSpawnArea.transform.position +
            new Vector3(randomPosX, 0.45f, randomPosZ);
        return randomSpawnPos;
    }
    // Detect when the agent hits the goal

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("out"))
        {
            EndOfEpisode(3);
        } else {
          if (col.gameObject.CompareTag("goal"))
          {
              EndOfEpisode(3);
          }
        }
    }

    public void AddScore() {
      m_score++;
      m_agentScore.text = m_score.ToString();
      //Debug.Log("XD");
    }

    public void EndOfEpisode(int option) {

      switch (option)
      {
          case 1: //GOOOOOOAL
              AddReward(100f);
              //Debug.Log(GetCumulativeReward());
              goals++;
              m_ratio = goals/episodes;
              Debug.Log("GOAL "+ m_score);
              break;
          case 2: //ball out
              //AddReward(-0.5f);
              break;
          case 3: //player out
              playerBody.position = m_startingPosition;
              playerBody.rotation = m_startingRotation;
              AddReward(-100f);
              break;
      }
      episodes++;
      m_ratio = goals/episodes;
      Debug.Log("episodes " + episodes + " reward " + GetCumulativeReward());
      Debug.Log("episodes " + episodes + "spawnregulator " + m_boundsRegulator);
      EndEpisode();

    }

}
