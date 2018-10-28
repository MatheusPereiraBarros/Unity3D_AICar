using UnityEngine;
using System.Collections;

public class DestroyTimer : MonoBehaviour {

    public float destoryTime = 2f;// Destory this object 1 second later
    private float timer;

	void Start () {
	
	}
	
	void Update () {
        timer += Time.deltaTime;
        if (timer > destoryTime)
        {
            Destroy(this.gameObject);
        }
	}
}
