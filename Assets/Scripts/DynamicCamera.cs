using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    public float rotationSpeed = 5f;

    private Camera cameraComponent;

    [SerializeField] private List<Transform> cameraPositions;

    [SerializeField] private Vector3 centerPointingPosition;

    private void Start()
    {
        cameraComponent = GetComponent<Camera>();
    }

    public void ChangeCamera()
    {
        transform.position = cameraPositions[Random.Range(0, cameraPositions.Count)].position;

        Vector3 targetDirection = centerPointingPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = targetRotation;
    }

    private void Update()
    {
        if (ScenarioManager.instance.cameraTarget != null)
        {
            // Get the direction from the spotlight to the target transform
            Vector3 targetDirection = (ScenarioManager.instance.cameraTarget.position + (Vector3.up * 0.1f)) - transform.position;

            // Create a rotation towards the target direction using Quaternion.LookRotation
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Slerp the current rotation towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Calculate the angle between the current rotation and the target rotation
            float angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);

            cameraComponent.fieldOfView = 30f;
        }
        else
        {
            Vector3 targetDirection = centerPointingPosition - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = targetRotation;
            cameraComponent.fieldOfView = 60f;
        }
    }
}
