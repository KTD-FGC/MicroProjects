using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    GameObject pointA, pointB;

    private Vector3 posA, posB;

    private float desiredDuration = 3f;
    private float elapsedTime;

    private bool atA = true;
    private bool atB;
    bool isMoving;

    [SerializeField]
    float delayTime = 1;

    [SerializeField]
    GameObject plat;

    private CharacterController cc;
    // Start is called before the first frame update
    void Start()
    {
        posA = pointA.transform.position;
        posB = pointB.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (atA && !isMoving)
        {
            StartCoroutine(MovePosition(posA, posB));
        }

        if (atB && !isMoving)
        {
            StartCoroutine(MovePosition(posB, posA));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.parent = plat.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.parent = null;
    }

    IEnumerator MovePosition(Vector3 pos1, Vector3 pos2)
    {
        isMoving = true;
        float time = 3f;
        while (time > 0)
        {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / desiredDuration;

            transform.position = Vector3.Lerp(pos1, pos2, percentageComplete);
            time -= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(delayTime);
        atB = !atB;
        atA = !atA;
        isMoving = false;
        elapsedTime = 0;
        yield return null;
    }

}
