using UnityEngine;

public class FlotationController : MonoBehaviour
{
    public float floatSpeed = 1.0f;
    public float floatIntensity = 0.1f;
    public float rotationSpeed = 10.0f;

    private Vector3 startPosition;
    private float timeCounter = 0.0f;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        
        float yOffset = Mathf.Sin(timeCounter * floatSpeed) * floatIntensity;
        transform.position = startPosition + new Vector3(0, yOffset, 0);

        
        float rotationAngle = Mathf.Sin(timeCounter * rotationSpeed) * floatIntensity * 10.0f;
        transform.Rotate(Vector3.up, rotationAngle);

        
        timeCounter += Time.deltaTime;
    }
}
