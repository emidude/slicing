using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem {
    public class SlicingSpheres : MonoBehaviour
    {

        public GameObject sphere;

        public List<Vector3> locatons;

        public int numberOfSpheres = 16;

        public List<GameObject> spheres;

        public List<Vector4> all4Dcoords;

        public float wSlice;

            

        public LinearMapping linearMapping;
        private float currentLinearMapping = float.NaN;
        private int framesUnchanged = 0;

        //HYPERPLANE INFO
        ///////////////////////////////////
        //private float[] unitNormal = new float[4];
        //private Vector4 unitNormal = new Vector4(0.5f,0.5f,0.5f,0.5f);
        private Vector4 unitNormal = new Vector4(0f,1f,0f,0f);
        private Vector4 unitNormalParallelToZ = new Vector4(0f,0f,0f,1f);
        private Vector4 reflectionParallelToZ = new Vector4(0f,0f,0f,-1f);
        public float d =0.2f;
        
        public float r = 1f;
         

        


        void Awake()
        {
            spheres = new List<GameObject>();
            wSlice = 0f;
            all4Dcoords = new List<Vector4>();
            CalculateCoordinates();

            //locatons = new List<Vector3> { Vector3.one, Vector3.one * 2f, Vector3.one * 3f };

            if (linearMapping == null)
            {
                linearMapping = GetComponent<LinearMapping>();
            }

        }

        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < numberOfSpheres; i++)
            {
                CreateSpheres(i);
            }

            for (int i = 0; i < numberOfSpheres; i++)
            {
                Debug.Log("sphere " + i + spheres[i].GetComponent<Info>().Get4DCoords());
            }


        }

        // Update is called once per frame
        void Update()
        {
          
            if (currentLinearMapping != linearMapping.value)
            {
                currentLinearMapping = linearMapping.value;
                d = (currentLinearMapping - 0.5f) * 4f;

            //////    Debug.Log("lin mapping = " + currentLinearMapping);

            //////     test - radius updates
            //////   for (int i = 0; i < numberOfSpheres; i++) {
            //////      spheres[i].transform.localScale = Vector3.one * currentLinearMapping;
            //////   }

            // /////  //update wSlice between -2 to 2 in w direection
            //     wSlice = (currentLinearMapping - 0.5f) * 4f;
            // ////    //Debug.Log("current slice " + wSlice);

            // // /////    //caluclate the position and radius of spheres intersecting that slice.

            // /////    //calucate radius
            // /////    //calculate distance in w direction from centre of slice
            //     for (int i = 0; i < numberOfSpheres; i++) {
            //         if (isSphereInSubDim(spheres[i]))
            //         {
            //             Vector4 coords = getCoords(spheres[i]);
            // ////            //this will need to be changed for different basis vectors
            //             spheres[i].transform.localPosition = new Vector3(coords.x, coords.y, coords.z);
            //             spheres[i].transform.localScale = CalculateDiameter(spheres[i]);
            //         }
            //         else { spheres[i].transform.localScale = Vector3.zero; }
            //     }


                ///////////////////////////////////////////////////
                for(int i = 0 ; i < numberOfSpheres;i++){
                    Vector4 c = spheres[i].GetComponent<Info>().Get4DCoords();
                    float minDis = calcMinDistance(unitNormal,d,c);

                    //if is in inntersection, get centreOfSlice and sliceRadius:
                    if(-r <= minDis && minDis <= r){
                        //calculate sliceCentre (4D)
                        Vector4 sliceCentre4D = c - minDis*unitNormal;
                        //need to convert 4D vectors to 3D local coordinated defined by hyperplane!!
                        //!!!!!!!!!!!!!!!!!!!!!!!
 
                        Vector4 n = reflectionParallelToZ - unitNormal;
                        Vector4 rotated4D;

                        if(n!= Vector4.zero){
                            rotated4D = rotateParallelToW(sliceCentre4D, n);
                            Debug.Log(i + ": " + rotated4D);
                        }
                        else rotated4D = sliceCentre4D;
                                               

                        //calcualte radius (perpendicular to n)
                        float sliceRadius = Mathf.Sqrt(r*r - minDis*minDis);



                        //need to change below to local coords!
                        spheres[i].transform.localPosition = new Vector3(rotated4D.x, rotated4D.y, rotated4D.z);
                        spheres[i].transform.localScale = Vector3.one* sliceRadius*2;
                    }
                    //else sliceradius = 0;
                    else { spheres[i].transform.localScale = Vector3.zero; }
                }
                


            }
        }

        Vector4 getCoords(GameObject s)
        {
            return s.GetComponent<Info>().Get4DCoords();
        }

        void CreateSpheres(int which)
        {
            GameObject s = Instantiate(sphere);
            
            //MAYBE NOT NECESSARY: (NOT RIGHT NOW AT LEAST)
            //s.transform.parent = this.transform;

            Info sphereInfo = s.GetComponent<Info>();
            Vector4 coords = all4Dcoords[which];
            sphereInfo.setCoords4D(coords);

            //s.transform.localPosition = new Vector3(coords.x, coords.y, coords.z);
            //s.transform.localScale = Vector3.one *2;

            //this will need to be changed for different basis vectors
            s.transform.localPosition = new Vector3(coords.x, coords.y, coords.z);

            //if sphere is in subDims, set postion and radius
            if (isSphereInSubDim(s))
            {
                s.transform.localScale = CalculateDiameter(s);
            }
            else { s.transform.localScale = Vector3.zero; }

            
            spheres.Add(s);
        }

        bool isSphereInSubDim(GameObject s) {
            float wCoord = s.GetComponent<Info>().Get4DCoords().w;
            if ((wSlice >= 0 && wSlice <= (wCoord + 1)) ||
                   (wSlice <= 0 && wSlice >= (wCoord - 1))
                   )
            { return true; }
            else return false;
            }

       

        void SetPosition(GameObject s) {
            
        }

        void CalculateCoordinates()
        {
            List<float> numbers = new List<float>();
            float[] options = { 1f, -1f };
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            numbers.Add(options[i]);
                            numbers.Add(options[j]);
                            numbers.Add(options[k]);
                            numbers.Add(options[l]);
                        }
                    }
                }
            }
            //for (int i = 0; i < numbers.Count; i+=4){
            //    Debug.Log(numbers[i] + " " + numbers[i + 1] + " " + numbers[i + 2] + " " + numbers[i + 3]); 
            //}

            for (int i = 0; i < numbers.Count; i += 4)
            {
                Vector4 v = new Vector4(numbers[i], numbers[i + 1], numbers[i + 2], numbers[i + 3]);
                all4Dcoords.Add(v);
            }

            for (int i = 0; i < all4Dcoords.Count; i++)
            {
                Debug.Log(all4Dcoords[i]);
            }
            //Debug.Log("count = " + all4Dcoords.Count);
        }

        Vector3 CalculateDiameter(GameObject s) {
            
            float wCoord = s.GetComponent<Info>().Get4DCoords().w;
            //Debug.Log("w coord =" + wCoord);
                           
                    //update scale (diamter) based on distance of wslice from center of w coordinate
                    float distanceFromCentre =Mathf.Sqrt (1 - (wSlice - wCoord)* (wSlice - wCoord)); //want it to be smallest at max distancwe
                    return Vector3.one * 2 * distanceFromCentre;
             //       Debug.Log("diameter = " + s.transform.localScale);
                
        
        }

    float calcMinDistance(Vector4 n, float d, Vector4 c){
        float minDist = Vector4.Dot(c,n) - d;
        return minDist;
    }

    Vector4 rotateParallelToW(Vector4 coords4D, Vector4 n){
        float dots = Vector4.Dot( coords4D, n) / Vector4.Dot( n, n);
        Vector4 rotatedCoords4D = coords4D - (2 * dots * n );
    return rotatedCoords4D;    
    }

    }

}
