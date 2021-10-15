using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

  public GameObject area;
  private FieldController fieldController;


    // Start is called before the first frame update
    void Start()
    {
       fieldController = area.GetComponent<FieldController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("goalP"))
        {
            fieldController.AddScore(1);
            fieldController.EndOfEpisode(1,1);
        }
        else if (col.gameObject.CompareTag("goalB"))
        {
            fieldController.AddScore(0);
            fieldController.EndOfEpisode(1,0);

        }
        else if (col.gameObject.CompareTag("out"))
        {

            fieldController.EndOfEpisode(2,1);

        }
    }
}
