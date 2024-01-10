using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBall : MonoBehaviour
{
    [SerializeField]
    IK_tentacles _myOctopus;

    //movement speed in units per second
    [Range(-1.0f, 1.0f)]
    [SerializeField]
    private float _movementSpeed = 5f;

    public Animator robot1;
    public Animator robot2;
    public Animator robot3;
    public Animator robot4;

    Vector3 _dir;


    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.identity;

        ////get the Input from Horizontal axis
        //float horizontalInput = Input.GetAxis("Horizontal");
        ////get the Input from Vertical axis
        //float verticalInput = Input.GetAxis("Vertical");

        ////update the position
        //transform.position = transform.position + new Vector3(-horizontalInput * _movementSpeed * Time.deltaTime, verticalInput * _movementSpeed * Time.deltaTime, 0);

    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Gol"))
        {
            robot1.SetBool("Win", true);
            robot1.SetBool("Lose", false);

            robot2.SetBool("Win", true);
            robot2.SetBool("Lose", false);

            robot3.SetBool("Win", true);
            robot3.SetBool("Lose", false);

            robot4.SetBool("Win", true);
            robot4.SetBool("Lose", false);

        }
        if (collider.CompareTag("Out"))
        {
            robot1.SetBool("Win", false);
            robot1.SetBool("Lose", true);

            robot2.SetBool("Win", false);
            robot2.SetBool("Lose", true);

            robot3.SetBool("Win", false);
            robot3.SetBool("Lose", true);

            robot4.SetBool("Win", false);
            robot4.SetBool("Lose", true);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        _myOctopus.NotifyShoot();

    }

}

