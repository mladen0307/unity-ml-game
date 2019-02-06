using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {


    public int id;
    public bool IsMoving = true;
    [SerializeField]
    float moveSpeed = 6;

    [SerializeField]
    float rotateSpeed = 3;

    Animator anim;    
    float axisHorizontal;

    int wallMask;
    Ray[] shootRays = new Ray[5];
    RaycastHit[] shootHit = new RaycastHit[5];
    public float range = 100f;
    double[] distances= new double[5];
    //LineRenderer lineRender;

    public NeuralNetwork myNeuralNet;
    public float currentDistance;
    public float maxDistance = 0f;
    float lastMaxChange = 0f;
    NeuralController neuralController;

    void Start()
    {        
        anim = GetComponent<Animator>();
        anim.SetBool("IsMoving", true);
        wallMask = LayerMask.GetMask("Wall");
        neuralController = transform.parent.GetComponent<NeuralController>();
        //lineRender = GetComponent<LineRenderer>();
        
    }

  
    void Update()
    {
        if (IsMoving)
        {
            CastRays();
            GetInputsFromNN();
            //PollInputs();        
            Move();
            SetMaxDistance();
        }
    }

    void SetMaxDistance()
    {
        currentDistance = transform.position.magnitude;
        if (currentDistance > maxDistance + 1f)
        {
            lastMaxChange = Time.time;
            maxDistance = currentDistance;
        }
        if (Time.time - lastMaxChange > 2.5f)
        {            
            Die();
        }
    }

    void GetInputsFromNN()
    {
        double[] res = myNeuralNet.FeedForward(distances);
        axisHorizontal = Mathf.Clamp((float)res[0], -1f, 1f);        
    }

    void PollInputs()
    {
        axisHorizontal = Input.GetAxisRaw("Horizontal");       
    }

    void Move()
    {
        transform.Rotate(transform.up, axisHorizontal * rotateSpeed);        
        transform.position = transform.position + transform.forward * moveSpeed * Time.deltaTime;
        //transform.position = transform.position + transform.forward * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;  
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            if (IsMoving)
            Die();
        }
    }

    public void Die()
    {
        anim.SetBool("IsDead", true);
        anim.SetBool("IsMoving", false);
        neuralController.Death(id);
        IsMoving = false;

    }

    public void Respawn()
    {
        anim.SetBool("IsMoving", true);
        anim.SetBool("IsDead", false);

        transform.position = new Vector3(0f, 0f, 0f);
        transform.rotation = Quaternion.identity;
        maxDistance = 0f;
        lastMaxChange = Time.time;

        IsMoving = true;
    }



    void CastRays()
    {
        for (int i = 0; i < 5; i++)  
            shootRays[i].origin = transform.position;
        shootRays[0].direction = transform.forward;
        shootRays[1].direction = transform.right;
        shootRays[2].direction = -transform.right;
        shootRays[3].direction = transform.forward + transform.right;
        shootRays[4].direction = transform.forward - transform.right;

        //lineRender.SetPosition(0, transform.position);

        for (int i = 0; i < 5; i++)
        {
            distances[i] = range;
            if (Physics.Raycast(shootRays[i], out shootHit[i], range, wallMask))
            {
                
                distances[i] = shootHit[i].distance;

                //if (i == 4)
                //{
                //    lineRender.SetPosition(1, shootHit[i].point);
                //    Debug.Log(i + ": " + distances[i]);
                //}
            }
            //else
            //{
            //    if (i == 4)
            //    {
            //        lineRender.SetPosition(1, shootRays[i].origin + shootRays[i].direction * range);
            //        Debug.Log(i + ": " + distances[i]);
            //    }
            //}
        }

    }


}
