using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentControl12 : Agent
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

    Bounds m_BallSpawnAreaBounds;
    Rigidbody m_ballRb;
    // Start is called before the first frame update
    /*void Start()
    {


    }*/

    public override void Initialize() {

      playerBody = GetComponent<Rigidbody>();
      m_BallSpawnAreaBounds = ballSpawnArea.GetComponent<Collider>().bounds;
      m_ballRb = ball.GetComponent<Rigidbody>();
      ballSpawnArea.SetActive(false);
      m_startingPosition = playerBody.position;

      m_agentScore = GameObject.Find("agentscore").GetComponent<TextMesh>();
      m_agentScore.text = m_score.ToString();

    }

    public override void OnEpisodeBegin() {

    }

    public override void OnActionReceived(ActionBuffers actionBuffers) {
        // Move the agent using the action.
        MoveAgent(actionBuffers.DiscreteActions);

        // Penalty given each step to encourage agent to finish task quickly.
       AddReward(-1f / 1000f);
    }

    public void MoveAgent(ActionSegment<int> act)
    {

        var action = act[0];
        var horiz = 0;
        var vert = 0;

        switch (action)
        {
            case 1:
                vert = 1;
                break;
            case 2:
                vert = -1;
                break;
            case 3:
                horiz = 1;
                break;
            case 4:
                horiz = -1;
                break;
        }
        inputVector = new Vector3(horiz * speed * 0.5f, playerBody.velocity.y * 1.3f, vert >= 0 ? vert * speed * 0.8f : vert * 0.6f * speed);
        transform.LookAt(transform.position + new Vector3(-inputVector.z, 0, inputVector.x));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
      Debug.Log("lmao");
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

        inputVector = new Vector3(Input.GetAxis("Horizontal") * speed * 0.5f, playerBody.velocity.y * 1.3f, Input.GetAxis("Vertical") >= 0 ? Input.GetAxis("Vertical") * speed * 0.8f : Input.GetAxis("Vertical") * 0.6f * speed);
        transform.LookAt(transform.position + new Vector3(-inputVector.z, 0, inputVector.x));
    }


    // Update is called once per frame
    void Update()
    {

        /*if (Input.GetButtonDown("Dash"))
        {
            Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * playerBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * playerBody.drag + 1)) / -Time.deltaTime)));
            playerBody.AddForce(dashVelocity, ForceMode.VelocityChange);
        }*/

    }
    void FixedUpdate() {

        playerBody.velocity = inputVector;

    }
    //Reset the orange block position
    public void ResetBall(Rigidbody ballRb)
    {
        ballRb.transform.position = GetRandomSpawnPosBall();
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
    }

    public Vector3 GetRandomSpawnPosBall()
    {
        var randomPosX = Random.Range(-m_BallSpawnAreaBounds.extents.x,
            m_BallSpawnAreaBounds.extents.x);
        var randomPosZ = Random.Range(-m_BallSpawnAreaBounds.extents.z,
            m_BallSpawnAreaBounds.extents.z);

        var randomSpawnPos = ballSpawnArea.transform.position +
            new Vector3(randomPosX, 0.45f, randomPosZ);
        return randomSpawnPos;
    }
    // Detect when the agent hits the goal

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("out"))
        {
            playerBody.position = m_startingPosition;
        }
    }

    public void AddScore() {
      m_score++;
      m_agentScore.text = m_score.ToString();
      Debug.Log("XD");
    }

}
