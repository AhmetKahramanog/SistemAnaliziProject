using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float cameraSpeed;
    [SerializeField] private Transform player;
    [SerializeField] private float cameraOffsetZ;
    [SerializeField] private float cameraOffsetY;
    void Start()
    {
        
    }
    void Update()
    {
        CameraMovement();
    }
    private void CameraMovement()
    {
        var targetPosition = new Vector3(player.transform.position.x, player.transform.position.y + cameraOffsetY, cameraOffsetZ);
        var followPlayer = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);
        transform.position = followPlayer;
    }
}
