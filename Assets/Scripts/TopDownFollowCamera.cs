using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Iaroslav Titov (c)
[RequireComponent(typeof(Camera))]
public class TopDownFollowCamera : MonoBehaviour
{
    [Header("Basic")]
    public GameObject target;
    [SerializeField] float flow;
    private Camera cam;
    [SerializeField] float defaultZ;

    [Header("Bounds")]
    [SerializeField] Vector2 minPos;
    [SerializeField] Vector2 maxPos;

    [Header("Zoom")]
    [SerializeField] bool enableZoom;
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;
    [SerializeField] float zoomSpeed;

    [Header("Move")]
    [SerializeField] bool enableMove;
    [SerializeField] float speed;

    MapGenerator map;
    bool flyFlag = false;

    private void Start()
    {
        map = FindObjectOfType<MapGenerator>();
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (enableZoom)
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, minZoom, maxZoom);

        if (!target)
        {
            if (!enableMove) return;
            if (Input.GetKey(KeyCode.W)) transform.position += Vector3.up * Time.deltaTime * speed;
            if (Input.GetKey(KeyCode.A)) transform.position += Vector3.left * Time.deltaTime * speed;
            if (Input.GetKey(KeyCode.S)) transform.position += Vector3.down * Time.deltaTime * speed;
            if (Input.GetKey(KeyCode.D)) transform.position += Vector3.right * Time.deltaTime * speed;
        }
        else
        {
            Vector3 newPos = Vector2.Lerp(transform.position, target.transform.position, flow * Time.deltaTime);
            if (Vector2.Distance(newPos, target.transform.position) > 0.1f)
            {
                newPos.z = defaultZ;
                transform.position = newPos;
                flyFlag = true;
            }
            else if (flyFlag)
            {
                flyFlag = false;
                newPos = target.transform.position;
                newPos.z = defaultZ;
                transform.position = newPos;
                StartCoroutine(map.RenderAll());
            }
        }

        if (transform.position.x < minPos.x) transform.position = new Vector3(minPos.x, transform.position.y, defaultZ);
        if (transform.position.y < minPos.y) transform.position = new Vector3(transform.position.x, minPos.y, defaultZ);
        if (transform.position.x > maxPos.x) transform.position = new Vector3(maxPos.x, transform.position.y, defaultZ);
        if (transform.position.y > maxPos.y) transform.position = new Vector3(transform.position.x, maxPos.y, defaultZ);
    }
}
