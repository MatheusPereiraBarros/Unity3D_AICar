using UnityEngine;
using System.Collections;

public class Skidding : MonoBehaviour {

    private WheelHit hit;
    private float sidewaysSlip;
    private float skidValue = 0.15f;
    private float soundEmission = 10;//Emit 10 skidSounds per second
    private float soundTime;
    public GameObject theSkidSound;
    private int skidding;
    private float skidWidth = 0.2f;
    private Vector3[] lastPos = new Vector3[2];
    public Material skidMarkMeterial;
    public ParticleSystem skidSmoke;

    void Start () {
        skidSmoke.transform.position = transform.position - new Vector3(0, -0.2f, 0);

    }
	
	void Update () {
        SkidSound();
        SkidMark();
    }

    void SkidSound()
    {
        GetComponent<WheelCollider>().GetGroundHit(out hit);
        sidewaysSlip = Mathf.Abs(hit.sidewaysSlip);
        if (sidewaysSlip > skidValue && soundTime <= 0 && GetComponent<WheelCollider>().isGrounded)//Slipping
        {
            Instantiate(theSkidSound, hit.point, Quaternion.identity);
            soundTime = 1;
        }
        soundTime -= Time.deltaTime * soundEmission; //Time.deltaTime convert per second to per frame
    }

    void SkidMark()
    {
        GetComponent<WheelCollider>().GetGroundHit(out hit);
        sidewaysSlip = Mathf.Abs(hit.sidewaysSlip);
        if (sidewaysSlip > skidValue && GetComponent<WheelCollider>().isGrounded)
        {
            GameObject Mark = new GameObject("Mark");
            MeshFilter MarkFilter = new MeshFilter();
            MeshRenderer MarkRenderer = new MeshRenderer();
            Vector3[] vertices = new Vector3[4];

            MarkFilter = Mark.AddComponent<MeshFilter>();
            MarkRenderer = Mark.AddComponent<MeshRenderer>();
            Quaternion Rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);
            if (skidding == 0)
            {
                vertices[0] = hit.point + Rotation * new Vector3(skidWidth, 0.01f, 0);
                vertices[1] = hit.point + Rotation * new Vector3(-skidWidth, 0.01f, 0);
                vertices[2] = hit.point + Rotation * new Vector3(-skidWidth, 0.01f, 0);
                vertices[3] = hit.point + Rotation * new Vector3(skidWidth, 0.01f, 0);
                lastPos[0] = vertices[2];
                lastPos[1] = vertices[3];
                skidding = 1;
            }
            else
            {
                vertices[0] = lastPos[1];
                vertices[1] = lastPos[0];
                vertices[2] = hit.point + Rotation * new Vector3(-skidWidth, 0.01f, 0);
                vertices[3] = hit.point + Rotation * new Vector3(skidWidth, 0.01f, 0);
                lastPos[0] = vertices[2];
                lastPos[1] = vertices[3];
            }
            MarkFilter.mesh.vertices = vertices;
            if (GetComponentInParent<Rigidbody>().velocity.z > 0)
            {
                int[] triangles = { 0, 1, 2, 0, 2, 3 };
                MarkFilter.mesh.triangles = triangles;
            }
            else
            {
                int[] triangles = { 2, 1, 0, 3, 2, 0 };
                MarkFilter.mesh.triangles = triangles;
            }
            //Set material
            Vector2[] uvs = new Vector2[4];
            uvs[0] = new Vector2(1, 0);
            uvs[1] = new Vector2(0, 0);
            uvs[2] = new Vector2(0, 1);
            uvs[3] = new Vector2(1, 1);
            MarkFilter.mesh.uv = uvs;
            MarkRenderer.material = skidMarkMeterial;
            //Destory the mark
            Mark.AddComponent<DestroyTimer>();
            //Emit smoke
            skidSmoke.transform.rotation = Rotation;
            var em = skidSmoke.emission;
            em.enabled = true;

            if(GetComponentInParent<CarControl>().gasCharge < 100)
                GetComponentInParent<CarControl>().gasCharge += sidewaysSlip * 0.2f;
        }
        else
        {
            skidding = 0;
            var em = skidSmoke.emission;
            em.enabled = false;
        }
    }
    
}
