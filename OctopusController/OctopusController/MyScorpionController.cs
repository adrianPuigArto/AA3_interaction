using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace OctopusController
{

    public class MyScorpionController
    {
        //TAIL
        Transform tailTarget;
        Transform tailEndEffector;
        MyTentacleController _tail;
        float animationRange;
        private Vector3[] aux;
        private Vector3[] tailBonesStartOffset;
        public float samplingDistance = 0.01f;
        public float learningRate = 50;
        float[] tailBonesAngles;
        private Vector3[] tailBonesAxis;





        //LEGS
        Transform[] legTargets = new Transform[6];
        Transform[] legFutureBases = new Transform[6];
        MyTentacleController[] _legs = new MyTentacleController[6];


        private float[] distances;
        private bool done;

        private float startPos;
        private float endPos;
        private float desiredDuration = 10f;
        private float elapsedTime;

        #region public
        public void InitLegs(Transform[] LegRoots, Transform[] LegFutureBases, Transform[] LegTargets)
        {
            _legs = new MyTentacleController[LegRoots.Length];

            for (int i = 0; i < LegRoots.Length; i++)
            {
                _legs[i] = new MyTentacleController();
                _legs[i].LoadTentacleJoints(LegRoots[i], TentacleMode.LEG);

                legFutureBases[i] = LegFutureBases[i];
                legTargets[i] = LegTargets[i];

            }

            aux = new Vector3[_legs[0].Bones.Length];
            distances = new float[_legs[0].Bones.Length];

        }

        public void InitTail(Transform TailBase)
        {
            _tail = new MyTentacleController();
            _tail.LoadTentacleJoints(TailBase, TentacleMode.TAIL);

            //Gradient
            tailBonesAxis = new Vector3[_tail.Bones.Length];
            tailBonesAngles = new float[_tail.Bones.Length];
            tailBonesStartOffset = new Vector3[_tail.Bones.Length];
            //Initial Position
            for (int i = 0; i < tailBonesAxis.Length; i++)
            {
                tailBonesStartOffset[i] = _tail.Bones[i].transform.localPosition;
            }
            //Initial Angles
            for (int i = 0; i < tailBonesAxis.Length; i++)
            {
                tailBonesAngles[i] = 0;
            }
            //Rotation Axis
            for (int i = 0; i < tailBonesAxis.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        tailBonesAxis[i] = new Vector3(1, 0, 0);
                        break;
                    default:
                        tailBonesAxis[i] = new Vector3(0, 0, 1);
                        break;
                }
            }
        }

        //TODO: Check when to start the animation towards target and implement Gradient Descent method to move the joints.
        public void NotifyTailTarget(Transform target)
        {
            tailTarget = target;
        }

        //TODO: Notifies the start of the walking animation
        public void NotifyStartWalk()
        {

        }

        //TODO: create the apropiate animations and update the IK from the legs and tail

        public void UpdateIK()
        {
            updateLegs();
            updateLegPos();
            updateTail();
        }
        #endregion


        #region private
        //for (int i = 0; i < _legs.Length; i++)
        //{
        //    if (Vector3.Distance(_legs[i].Bones[0].position, legFutureBases[i].position) > 0.35f)
        //    {
        //        _legs[i].Bones[0].position = Vector3.Lerp(_legs[i].Bones[0].position, legFutureBases[i].position,Time.deltaTime);

        //    }

        //}
        //TODO: Implement the leg base animations and logic

        float lerpCounter = 0;
        private void updateLegPos()
        {

            for (int i = 0; i < _legs.Length; i++)
            {
                if (Vector3.Distance(_legs[i].Bones[0].position, legFutureBases[i].position) > 0.7f) //0.35
                {
                    //_legs[i].Bones[0].position = Vector3.Lerp(_legs[i].Bones[0].position, legFutureBases[i].position, lerpCounter);
                    _legs[i].Bones[0].position = legFutureBases[i].position;
                    updateLegs();
                }
            }
    
            

        }
        //TODO: implement Gradient Descent method to move tail if necessary
        private void updateTail()
        {
            if (Vector3.Distance(_tail.Bones[_tail.Bones.Length - 1].position, tailTarget.position)<= 4){
                for (int i = 0; i < _tail.Bones.Length; i++)
                {
                    tailBonesAngles[i] -= PartialGradient(tailTarget.transform.position, tailBonesAngles, i) * learningRate;
                    if (i == 0)
                    {
                         _tail.Bones[i].localEulerAngles = new Vector3(tailBonesAngles[i], _tail.Bones[i].rotation.y, _tail.Bones[i].rotation.z);
                    }
                    else
                    {
                    _tail.Bones[i].localEulerAngles = new Vector3(_tail.Bones[i].rotation.x, _tail.Bones[i].rotation.y, tailBonesAngles[i]);

                    }


                }
            }

        }
        public Vector3 ForwardKinematics(float[] angles)
        {
            Vector3 prevPoint = _tail.Bones[0].transform.position;
            Quaternion rotation = Quaternion.identity;
            for (int i = 1; i < _tail.Bones.Length; i++)
            {
                // Rotates around a new axis
                rotation *= Quaternion.AngleAxis(angles[i - 1], tailBonesAxis[i-1]);
                Vector3 nextPoint = prevPoint + rotation * tailBonesStartOffset[i];

                prevPoint = nextPoint;
            }
            return prevPoint;
        }
        float DistanceFromTarget(Vector3 target, float[] angles)
        {
            Vector3 point = ForwardKinematics(angles);
            return Vector3.Distance(point, target);

        }

        float PartialGradient(Vector3 target, float[] angles, int i) 
        {
            float angle = angles[i];
            float f_x = DistanceFromTarget(target, angles);

            angles[i] += samplingDistance;
            float f_x_plus_d = DistanceFromTarget(target, angles);

            float gradient = (f_x_plus_d - f_x) / samplingDistance;

            angles[i] = angle;


            return gradient;
        }
  
        void ApproachTarget()
        {


        }

        //TODO: implement fabrik method to move legs 

        private Vector3[] Backward(Vector3[] ForwardPos, int legIndex)
        {
            Vector3[] inversePos = new Vector3[ForwardPos.Length];

            inversePos[inversePos.Length - 1] = legTargets[legIndex].position;

            for (int i = ForwardPos.Length - 2; i >= 0; i--)
            {
                Vector3 vecDirection = (ForwardPos[i] - inversePos[i + 1]).normalized;
                inversePos[i] = inversePos[i + 1] + vecDirection * distances[i];
            }

            return inversePos;

        }
        private Vector3[] Forward(Vector3[] backwardPos, int legIndex)
        {
            Vector3[] forwardPos = new Vector3[backwardPos.Length];
            forwardPos[0] = _legs[legIndex].Bones[0].position;

            for (int i = 1; i < backwardPos.Length; i++)
            {
                Vector3 vecDirection = (backwardPos[i] - forwardPos[i - 1]).normalized;
                forwardPos[i] = forwardPos[i - 1] + vecDirection * distances[i-1];
            }
 

            return forwardPos;
        }

        private void updateLegs()
        {
            for (int i = 0; i < _legs.Length; i++)
            {
                //done = Vector3.Distance(_legs[i].Bones[_legs[i].Bones.Length - 1].position, legTargets[i].position) < 0.5f; //
                //float targetRootDist = Vector3.Distance(aux[0], legTargets[i].position);
                Vector3[] finalPos = new Vector3[_legs[i].Bones.Length];
                for (int j = 0; j < _legs[i].Bones.Length; j++)
                {

                    finalPos[j] = _legs[i].Bones[j].transform.position;
                }

                distances[distances.Length - 1] = 0;
                for (int k = 0; k < _legs[i].Bones.Length; k++)
                {
                    if (k < _legs[i].Bones.Length - 1)
                    {
                        distances[k] = (_legs[i].Bones[k + 1].position - _legs[i].Bones[k].position).magnitude;

                    }
                    else
                    {
                        distances[k] = 0;
                    }
                }

                //Aplicamos FABRIK
                for (int h = 0; h < 10; h++)
                {
                    finalPos = Backward(Forward(finalPos, i), i);
                }

                int u;
                Vector3 vec;
                Vector3 vec2;

                float angle;
                Vector3 vec3;

                for (int j = 0; j <= _legs[i].Bones.Length - 2; j++)
                {
                    vec = _legs[i].Bones[j + 1].transform.position - _legs[i].Bones[j].transform.position;
                    vec2 = finalPos[j + 1] - finalPos[j];

                    angle = Mathf.Acos(Vector3.Dot(vec.normalized, vec2.normalized));
                    vec3 = Vector3.Cross(vec, vec2).normalized;

                    _legs[i].Bones[j].transform.Rotate(vec3, angle * Mathf.Rad2Deg, Space.World);
                }

            }
        }

    }
}
#endregion
