using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public AgentControl Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }


    /// <summary>
    /// Max Academy steps before this platform resets
    /// </summary>
    /// <returns></returns>
    [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 1000;

    public GameObject ball;
    [HideInInspector]
    public Rigidbody ballRb;
    public GameObject ballSpawnArea;
    Vector3 m_BallStartingPos;

    //List of Agents On Platform
    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();

    [HideInInspector]
    public int lastTeamTouched = 100;


    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_PurpleAgentGroup;

    private int m_ResetTimer;
    float m_boundsRegulator;
    Bounds m_BallSpawnAreaBounds;

    StatsRecorder statsRecorder;
    EnvironmentParameters m_ResetParams;

    private int m_scoreP = 0;
    private int m_scoreB = 0;
    private TextMesh m_agentScore;

    public GameObject score;

    int m_fails = 0;
    float goals = 0f;
    float episodes = 1f;
    float m_ratio = 0;

    void Start()
    {

        // Initialize TeamManager
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_PurpleAgentGroup = new SimpleMultiAgentGroup();
        //ball = GameObject.Find("SoccerBall");
        //ballSpawnArea = GameObject.Find("ballSpawnArea");
        ballRb = ball.GetComponent<Rigidbody>();
        m_BallStartingPos = new Vector3(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z);
        m_BallSpawnAreaBounds = ballSpawnArea.GetComponent<Collider>().bounds;
        ballSpawnArea.SetActive(false);
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        m_agentScore = score.GetComponent<TextMesh>();
        m_agentScore.text = m_scoreB.ToString() + "-" + m_scoreP.ToString() ;
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            if (item.Agent.team == Team.Blue)
            {
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            }
            else
            {
                m_PurpleAgentGroup.RegisterAgent(item.Agent);
            }
        }
        ResetScene();
    }

    void FixedUpdate()
    {
        m_ResetTimer += 1;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            m_BlueAgentGroup.GroupEpisodeInterrupted();
            m_PurpleAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }

        m_boundsRegulator = m_ResetParams.GetWithDefault("my_environment_parameter", 1f);

        //Debug.Log("lastTeamTouched " + lastTeamTouched );
    }


    public void ResetBall(Rigidbody ballRb)
    {
        ballRb.transform.position = GetRandomSpawnPosBall();
        foreach (var item in AgentsList)
        {
            item.Agent.ballPreviousDistance = Vector3.Distance(ballRb.transform.position, item.Agent.goalAreaPos);
        }
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

    public void ResetScene() //RESET POS JUGADORS I RESET BALL
    {
        m_ResetTimer = 0;

        episodes++;


          //Debug.Log("episodes " + episodes + " reward blue" + GetCumulativeReward());
          Debug.Log("episodes " + episodes + "spawnregulator " + m_boundsRegulator);

        //Reset Agents
        foreach (var item in AgentsList)
        {
            // var randomPosX = Random.Range(-5f, 5f);
            // var newStartPos = item.Agent.initialPos + new Vector3(randomPosX, 0f, 0f);
            // var rot = item.Agent.rotSign * Random.Range(80.0f, 100.0f);
            // var newRot = Quaternion.Euler(0, rot, 0);
            // item.Agent.transform.SetPositionAndRotation(newStartPos, newRot);

            item.Rb.position = item.StartingPos;
            item.Rb.rotation = item.StartingRot;

            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }

        //Reset Ball
        ResetBall(ballRb);
    }

    // public void GoalTouched(Team scoredTeam) //SERÀ END EPISODE
    // {
    //     if (scoredTeam == Team.Blue)
    //     {
    //         m_BlueAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
    //         m_PurpleAgentGroup.AddGroupReward(-1);
    //     }
    //     else
    //     {
    //         m_PurpleAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
    //         m_BlueAgentGroup.AddGroupReward(-1);
    //     }
    //     m_PurpleAgentGroup.EndGroupEpisode();
    //     m_BlueAgentGroup.EndGroupEpisode();
    //     ResetScene();
    //
    // }

    public void EndOfEpisode(int option, int scored) {

      switch (option)
      {
          case 1: //GOOOOOOAL

              if (scored == (int)Team.Blue) {

                m_BlueAgentGroup.AddGroupReward(1f);
                m_PurpleAgentGroup.AddGroupReward(-1f);
                Debug.Log("goal blue");

              } else {

                m_PurpleAgentGroup.AddGroupReward(1f);
                m_BlueAgentGroup.AddGroupReward(-1f);
                Debug.Log("goal purple");

              }
              Debug.Log("GOAL "+ m_scoreB + "-" + m_scoreP);
              break;
          case 2: //ball out

              if (lastTeamTouched == 0) {
                m_BlueAgentGroup.AddGroupReward(-0.1f);
                Debug.Log("Out from team blue");

              } else {
                m_PurpleAgentGroup.AddGroupReward(-0.1f);
                Debug.Log("Out from team purple");
              }
              break;
          case 3: //player out
              break;
      }

      m_PurpleAgentGroup.EndGroupEpisode();
      m_BlueAgentGroup.EndGroupEpisode();
      ResetScene();

    }

    public void AddScore(int team) {

      if (team == 0) {
        m_scoreP++;

      }
      else {
        m_scoreB++;
      };
      m_agentScore.text = m_scoreB.ToString() + "-" + m_scoreP.ToString() ;
    }
}
