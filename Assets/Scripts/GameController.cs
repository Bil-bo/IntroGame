using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{

    private GameObject[] pickUps;
    private int closest = 0;
    public GameObject player;
    private float compareClosest;
    private LineRenderer lineRenderer;
    public TextMeshProUGUI closePickUp;
    public TextMeshProUGUI playerPosition;
    public TextMeshProUGUI playerVelocity;
    private PlayerController playerScript;
    private Mode mode;

    private enum Mode
    {
        normal,
        debug,
        vision

    }

    // Start is called before the first frame update
    void Start()
    {
        mode = Mode.normal;
        pickUps = GameObject.FindGameObjectsWithTag("PickUp");
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.enabled = false;
        playerPosition.enabled = false;
        playerVelocity.enabled = false;
        closePickUp.enabled = false;
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == Mode.debug)
        {
            DebugMode();
        }
        else if (mode == Mode.vision)
        {
            VisionMode();
        }


    }
    void OnSwitchMode()
    {
        if (mode == Mode.normal)
        {
            mode = Mode.debug;
            playerPosition.enabled = true;
            playerVelocity.enabled = true;
            closePickUp.enabled = true;
            lineRenderer.enabled = true;   
        }
        else if (mode == Mode.debug)
        {
            mode = Mode.vision;
        }
        else if (mode == Mode.vision)
        {
            mode = Mode.normal;
            for (int i = 0; i < pickUps.Length; i++) 
            {
                pickUps[i].GetComponent<Renderer>().material.color = Color.white;   
            }
            lineRenderer.enabled = false;
            playerPosition.enabled = false;
            playerVelocity.enabled = false;
            closePickUp.enabled = false;
        }

    }



    private void DebugMode()
    {
        try
        {
            pickUps = GameObject.FindGameObjectsWithTag("PickUp");
            closest = 0;
            float closestDistance = Mathf.Infinity;
            for (int i = 0; i < pickUps.Length; i++)
            {
                pickUps[i].GetComponent<Renderer>().material.color = Color.white;
                compareClosest = Vector3.Distance(player.transform.position, pickUps[i].transform.position);
                if (compareClosest <= closestDistance)
                {
                    closest = i;
                    closestDistance = compareClosest;
                }
            }
            lineRenderer.SetPosition(0, pickUps[closest].transform.position);
            lineRenderer.SetPosition(1, player.transform.position);

            closePickUp.text = "Pick Up Distance: " + closestDistance.ToString("0.00");
            pickUps[closest].GetComponent<Renderer>().material.color = Color.blue;

        }
        catch (System.Exception e) {
            lineRenderer.enabled = false;

        }

    }

    private float CountDistance(Vector3 a, Vector3 b) {
        float distance = 0;
        distance = (float)(Math.Pow(Math.Abs(a.x * b.z - b.x * a.z), 0.5) / Math.Pow(Math.Pow(a.x, 2) + Math.Pow(a.z, 2), 0.5));
        return distance;
    }

    private void VisionMode()
    {
        Vector3 towards = (player.transform.position - playerScript.oldPosition) * 50;
        try
        {
            pickUps = GameObject.FindGameObjectsWithTag("PickUp");


            float minimum = Mathf.Infinity;
            int closest = 0;
            for (int i = 0; i < pickUps.Length; i++)
            {
                pickUps[i].GetComponent<Renderer>().material.color = Color.white;
                if (CountDistance(towards, pickUps[i].transform.position - player.transform.position) <= minimum && (towards.x * (pickUps[i].transform.position - player.transform.position).x + towards.z * (pickUps[i].transform.position - player.transform.position).z) >= 0)
                {
                    minimum = CountDistance(towards, pickUps[i].transform.position - player.transform.position);
                    closest = i;
                }
            }

            pickUps[closest].GetComponent<Renderer>().material.color = Color.green;
            pickUps[closest].transform.LookAt(player.transform.position);




        } catch (Exception e) { }
        lineRenderer.SetPosition(0, player.transform.position);
        lineRenderer.SetPosition(1, player.transform.position + towards);
    }
}
