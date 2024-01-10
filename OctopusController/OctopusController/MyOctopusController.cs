using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace OctopusController
{
    public enum TentacleMode { LEG, TAIL, TENTACLE };

    public class MyOctopusController 
    {
        
        MyTentacleController[] _tentacles =new  MyTentacleController[4];

        Transform _currentRegion;
        Transform _target;

        Transform[] _randomTargets;// = new Transform[4];


        float _twistMin, _twistMax;
        float _swingMin, _swingMax;

        #region public methods
        //DO NOT CHANGE THE PUBLIC METHODS!!

        public float TwistMin { set => _twistMin = value; }
        public float TwistMax { set => _twistMax = value; }
        public float SwingMin {  set => _swingMin = value; }
        public float SwingMax { set => _swingMax = value; }
        

        public void TestLogging(string objectName)
        {

           
            Debug.Log("hello, I am initializing my Octopus Controller in object "+objectName);

            
        }

        public void Init(Transform[] tentacleRoots, Transform[] randomTargets)
        {
            _tentacles = new MyTentacleController[tentacleRoots.Length];

            // foreach (Transform t in tentacleRoots)
            for(int i = 0;  i  < tentacleRoots.Length; i++)
            {

                _tentacles[i] = new MyTentacleController();
                _tentacles[i].LoadTentacleJoints(tentacleRoots[i],TentacleMode.TENTACLE);
                //TODO: initialize any variables needed in ccd
            }

            _randomTargets = randomTargets;
            //TODO: use the regions however you need to make sure each tentacle stays in its region

        }

              
        public void NotifyTarget(Transform target, Transform region)
        {
            _currentRegion = region;
            _target = target;
        }

        public void NotifyShoot() {
            //TODO. what happens here?
            Debug.Log("Shoot");
        }


        public void UpdateTentacles()
        {
            //TODO: implement logic for the correct tentacle arm to stop the ball and implement CCD method
            update_ccd();
        }




        #endregion


        #region private and internal methods
        //todo: add here anything that you need
        
        
        int _mtries = 3;
        float angleRot;
        float _epsilon = 0.1f;
        float cosinus;
        void update_ccd()
        {

            for (int i = 0; i < _tentacles.Length; i++) 
            {

                bool _done = false; // con esto fuera del update no se mueve lo mismo con el _tries
                int _tries = 0;


                while (!_done && _tries < _mtries) // funciona con o sin el _done
                {
                    for (int j = _tentacles[i].Bones.Length - 1; j >= 0; j--) 
                    {
                        // The vector from the ith joint to the end effector
                        //Vector3 r1 = joints[joints.Length - 1].transform.position - joints[i].transform.position;
                        Vector3 r1 = _tentacles[i].EndEffector.transform.position - _tentacles[i].Bones[j].transform.position;

                        // The vector from the ith joint to the target
                        //Vector3 r2 = tpos - joints[i].transform.position;
                        Vector3 r2 = _randomTargets[i].transform.position - _tentacles[i].Bones[j].transform.position;

                        if (r1.magnitude * r2.magnitude <= 0.001f){
                            // cos ? sin?
                            //_sin[i] = ;
                            //_cos[i] = ;
                        }
                        else
                        {
                            
                            angleRot = Mathf.Acos(Vector3.Dot(r1.normalized, r2.normalized));
                        }

                        // The axis of rotation ( quaternion del cross product )
                        Vector3 axisRot = Vector3.Cross(r1.normalized, r2.normalized);
                        axisRot = axisRot.normalized;

                        // find the angle between r1 and r2 (and clamp values if needed avoid errors)
                        //angleRot = Mathf.Rad2Deg * angleRot;  //con esto va a trompicones

                        _tentacles[i].Bones[j].Rotate(axisRot, angleRot, Space.World); 

                    }
                    _tries++;

                    float diference = Vector3.Distance(_tentacles[i].EndEffector.transform.position, _randomTargets[i].transform.position);

                    if (diference < _epsilon) _done = true;
                    else _done = false;
                }
            }


        }

    }
}


        

        #endregion








