using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    public Slider shootSlider, effectSlider;
    private bool goDown;

    public Rigidbody ball;
    public Transform tail;
    public GameObject target;
    public GameObject printer;

    float vxInicial = 0;
    float vyInicial = 0;
    float vzInicial = 0;

    float xInicial;
    float yInicial;
    float zInicial;

    float x;
    float y;
    float z;

    float aceleracionY;
    public float tiempo;

    int numPrinters = 50;

    GameObject[] printers;
    void Start()
    {
        goDown = true;
        xInicial = ball.transform.position.x;
        yInicial = ball.transform.position.y;
        zInicial = ball.transform.position.z;

        x = target.transform.position.x;
        y = target.transform.position.y;
        z = target.transform.position.z;

        aceleracionY = -10;
        tiempo = 1;
        printers = new GameObject[numPrinters];
        for (int i = 0; i < numPrinters; i++)
        {
            printers[i] = Instantiate(printer);

        }
    }

    
    Vector3 posPrint = new Vector3(0, 0, 0);

    void Update()
    {
        UpdateSolver();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {


            if (shootSlider.value >= shootSlider.maxValue) goDown = false;
            else if (shootSlider.value <= 0) goDown = true;

            if (goDown) shootSlider.value += 1f;
            else shootSlider.value -= 1f;

        }

        if (Input.GetKey(KeyCode.Z)) effectSlider.value -= 1f;
        if (Input.GetKey(KeyCode.X)) effectSlider.value += 1f;
    }

    void UpdateSolver()
    {

        tiempo = Mathf.Lerp(4.0f, 0.5f, shootSlider.value / 100);
        x = target.transform.position.x;
        y = target.transform.position.y;
        z = target.transform.position.z;
        //x = x + v0 * t + 0.5 * a * t^2
        vyInicial = (float)(y - yInicial - 0.5 * aceleracionY * (tiempo * tiempo)) / tiempo;
        vxInicial = (x - xInicial) / tiempo;
        vzInicial = (z - zInicial) / tiempo;
        
        //printar recorrido

        float interval = tiempo / numPrinters;
        float newT;
        for (int i = 0; i < numPrinters; i++)
        {
            newT = i * interval;
            posPrint.x = xInicial + vxInicial * (newT);
            posPrint.z = zInicial + vzInicial * (newT);
            posPrint.y = (float)(yInicial + vyInicial * (newT) + 0.5 * aceleracionY * ((newT) * (newT)));
            printers[i].transform.position = posPrint;
        }


        if (Vector3.Distance(ball.transform.position,tail.position)<=1)
        {
            shoot(new Vector3(vxInicial, vyInicial, vzInicial));
        }
    }

    private void shoot(Vector3 _velocidadInicial)
    {
        ball.useGravity = true;
        ball.velocity = _velocidadInicial;
    }
}

