using UnityEngine;
using System.Collections;

public class TrafficControl : MonoBehaviour {

    public float timer;
    float resetTimer = 20;
    public Transform[] roadHeads;

    void Start()
    {
        roadHeads = new Transform[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            roadHeads[i] = transform.GetChild(i);
        }
    }

	void Update () {
        timer += Time.deltaTime;
        if(timer < resetTimer / 2 - 1)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                if (roadHeads[i].CompareTag("RHC"))
                {
                    roadHeads[i].GetComponent<TrafficLight>().canPass = true;
                    roadHeads[i].GetComponent<Renderer>().material.color = Color.green;
                }
                else
                {
                    roadHeads[i].GetComponent<TrafficLight>().canPass = false;
                    roadHeads[i].GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
        else if(timer > resetTimer / 2 - 1 && timer < resetTimer / 2 + 1)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                roadHeads[i].GetComponent<TrafficLight>().canPass = false;
                roadHeads[i].GetComponent<Renderer>().material.color = Color.red;
            }
        }
        else if (timer > resetTimer / 2 + 1 && timer < resetTimer)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (roadHeads[i].CompareTag("RHL"))
                {
                    roadHeads[i].GetComponent<TrafficLight>().canPass = true;
                    roadHeads[i].GetComponent<Renderer>().material.color = Color.green;
                }
                else
                {
                    roadHeads[i].GetComponent<TrafficLight>().canPass = false;
                    roadHeads[i].GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
        else
        {
            timer = 0;
        }
	}
}
