using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Primer calculem dist de joint 0 al target. Despres calculem la distancia de joint 0-1, 1-2, 2-3. Si la suma d'aquestes 3 distancies es mes gran que la del vector de J0 a target es imposible arribar
//ja que J0 no es pot moure 8es com si fos l'espatlla.

public class IK_FABRIK2 : MonoBehaviour
{
    public Transform[] joints;
    public Transform target;

    private Vector3[] copy;
    private float[] distances;
    private bool done;

    //private float[] sumDistances = sum.
    void Start()
    {
        done = false;

        copy = new Vector3[joints.Length];

        for (int i = 0; i < joints.Length; i++)
        {

            copy[i] = joints[i].position;

        }

        distances = new float[joints.Length - 1];

        distances[0] = Vector3.Distance(copy[0], copy[1]);
        distances[1] = Vector3.Distance(copy[1], copy[2]);
        distances[2] = Vector3.Distance(copy[2], copy[3]);
    }

    void Update()
    {
        // Copy the joints positions to work with
        for (int i = 0; i < joints.Length; i++)
        {

            copy[i] = joints[i].position;

        }
        //TODO



        done = Vector3.Distance(joints[joints.Length - 1].position, target.position) < 0.5f;


        if (!done)
        {
            float targetRootDist = Vector3.Distance(copy[0], target.position);

            // Update joint positions
            if (targetRootDist > distances.Sum())
            {
                // The target is unreachable

            }
            else
            {
                // The target is reachable
                //while (TODO)
                while (!done)//target) //copy[3] != target.position
                {
                    // STAGE 1: FORWARD REACHING
                    //TODO
                    //copy[1] = copy[0] + copy[0]. * Vector3.forward * copy[0].;
                    //joints[3].position = target.position;
                    copy[3] = target.position;

                    //Vector director(normalitzat) de J2 a J3'. Aplicar la distancia que hi havia abans de J2 a J3 en la direccio trobada abans.
                    //En primera iteració J3 sa mogut es a dir copy[3] es prima(') pero J2 es una copia de la pos del joint encara. 
                    for (int i = 2; i >= 0; i--)
                    {
                        //Vector3 direction = Vector3.Normalize(copy[i] - copy[i - 1]);
                        Vector3 direction = Vector3.Normalize(copy[i] - copy[i + 1]);

                        copy[i] = direction * distances[i] + copy[i + 1]; //Sumem position del joint perque no esta situat al origen de coordenades i es necesari
                        //copy[i - 1].ApplyConstraints(direction);

                        //copy[i] = copy[i - 1] + copy[i - 1]. * Vector3.forward * copy[i - 1];
                    }


                    // STAGE 2: BACKWARD REACHING
                    //TODO
                    //joints[3] = target.transform;
                    //float dist = Vector3.Distance(copy[0], joints[0].position); //NO ES NECESARI, amb la dist trobada abans ja tenim suficient ja que la dist dels ossos no varia!
                    //joints[0].position = copy[0];
                    copy[0] = joints[0].position; //Psem a copy[0] en la pisicio on va començar ja que l'objectiu es mantenir-lo

                    for (int i = 1; i < copy.Length; i++)
                    {
                        //Vector3 direction = Vector3.Normalize(copy[i] - copy[i + 1]);
                        Vector3 direction = Vector3.Normalize(copy[i] - copy[i - 1]);
                        copy[i] = direction * distances[i - 1] + copy[i - 1];
                        //copy[i] = copy[i + 1] + direction * copy[i].sqrMagnitude;
                    }

                    done = true;
                }
            }
            //Trobat Vect 0-1 i 0'-1'. DOT product entre els dos per trobar angle.
            //Fer-ho tot en copy[] i depres modificar els joints.
            // Update original joint rotations
            for (int i = 0; i <= joints.Length - 2; i++)
            {

                Vector3 vec = joints[i + 1].transform.position - joints[i].transform.position;
                Vector3 vec2 = copy[i + 1] - copy[i];

                float angle = Mathf.Acos(Vector3.Dot(vec.normalized, vec2.normalized));
                Vector3 vec3 = Vector3.Cross(vec, vec2).normalized;

                joints[i].transform.Rotate(vec3, angle, Space.World);

            }

        }
    }

}
