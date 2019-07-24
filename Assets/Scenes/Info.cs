using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info : MonoBehaviour {

    public Vector4 coords4D;

    Vector3 centre3D;

    float radius;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setCoords4D(Vector4 c4d){
        coords4D = c4d;
    }

    public Vector4 Get4DCoords(){
        return coords4D;
    }
}
