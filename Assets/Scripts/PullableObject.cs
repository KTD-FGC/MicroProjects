using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullableObject : MonoBehaviour
{
    [SerializeField]
    GameObject endPointMarker;

    private Vector3 endPoint;
    private Vector3 startPoint;

    private float desiredDuration = 0.75f;
    private float elapsedTime;
    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position;
        endPoint = endPointMarker.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DoPull()
    {
        float time = 0.75f;
        while (time > 0)
        {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / desiredDuration;

            transform.position = Vector3.Lerp(startPoint, endPoint, percentageComplete);
            time -= Time.deltaTime;
            yield return null;
        }
        Destroy(this);
    }
}
