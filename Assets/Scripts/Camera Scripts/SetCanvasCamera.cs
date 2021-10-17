using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasCamera : MonoBehaviour
{

    Canvas canvas;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    Camera camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    void Start()
    {
        canvas = gameObject.GetComponent<Canvas>();

    }

    private void Update()
    {
        if (camera == null)
        {
            camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            canvas.worldCamera = camera;
        }
    }
}
