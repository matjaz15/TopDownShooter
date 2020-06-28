using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleLightWithScreen : MonoBehaviour {


    Light targetLight;

    float initialRadius;
    float targetRadius;

    private void Start()
    {
        targetLight = GetComponent<Light>();
        initialRadius = targetLight.range;
        targetRadius = initialRadius;
        
    }

    private void Update()
    {
        float scaleFactor = Vector2.SqrMagnitude(new Vector2(Screen.width/2, Screen.height/2)) / 100000;
        targetLight.range = scaleFactor * initialRadius;
    }



}
