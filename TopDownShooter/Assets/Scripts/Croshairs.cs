using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Croshairs : MonoBehaviour {

    public LayerMask targetMask;
    public Color dotHighLightColor;
    public SpriteRenderer[] dots;

    Color originalDotColor;

    private void Start()
    {
        Cursor.visible = false;
        originalDotColor = dots[0].color;
    }

    void Update () {
        transform.Rotate(Vector3.forward * 40 * Time.deltaTime);
	}

    public void DetectTargets(Ray ray) {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            foreach (SpriteRenderer dot in dots)
            {
                dot.color = dotHighLightColor;
            }
            
        }
        else {
            foreach (SpriteRenderer dot in dots)
            {
                dot.color = originalDotColor;
            }
        }
    }
}
