using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{

    // TODO: Fix bug when one of the players goes beyond the other
    // TODO: Make it smoother/with cooler transition effect

    private Transform p1;
    private Transform p2;

    public float widthOffset = 2f;
    public float heightOffset = 2f;

    private Camera camera;
    private float calc_distance;
    private float screenAspect;

    private float originalOrthoSize;

    // Start is called before the first frame update
    void Start()
    {
        // Cahce camera ref
        camera = Camera.main;

        p1 = GameObject.Find("PlayerLeft").transform;//playerLeft
        p2 = GameObject.Find("PlayerRight").transform;//playerLeft

        screenAspect = (float)Screen.width / (float)Screen.height;
        originalOrthoSize = camera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        calc_distance = Mathf.Abs(p1.position.x - p2.position.x) + widthOffset;
        camera.orthographicSize = Mathf.Clamp(calc_distance / (2 * screenAspect), 1, 10);
        camera.transform.position = new Vector3(
            p1.position.x - widthOffset/2 + (0.5f * calc_distance),
            p1.position.y + (heightOffset * (camera.orthographicSize/ originalOrthoSize)), 
            camera.transform.position.z);
    }
}
