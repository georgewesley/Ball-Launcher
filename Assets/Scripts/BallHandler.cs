using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{

    [SerializeField] CircleCollider2D springCollider;
    [SerializeField] private SpringJoint2D currentBallSpringJoint;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float respawnTime;
    CircleCollider2D ballCollider;
    private Rigidbody2D currentBallRigidBody;
    Vector2 touchPosition;
    private Camera mainCamera;
    private bool isDragging;
    private bool isLaunching;
     

    private float gravityScale;
    void Awake()
    {
        Respawn();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if(currentBallRigidBody == null) { return; }
        if(!Touchscreen.current.primaryTouch.press.isPressed) {
            if(isDragging) {
                if(!isLaunching)
                {
                    currentBallRigidBody.gravityScale = gravityScale;
                    LaunchBall();
                }
                if(springCollider.IsTouching(ballCollider)) {
                    DetachBall();
                }
            }
            return;
        }
        if (!isLaunching)
        {
            isDragging = true;
            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
            currentBallRigidBody.position = worldPosition;
        }
        else
        {
            if (springCollider.IsTouching(ballCollider))
            {
                DetachBall();
            }
        }
    }
    private void LaunchBall() {
        isLaunching = true;
        currentBallRigidBody.isKinematic = false;
        currentBallSpringJoint.GetComponent<AudioSource>().Play();
    }

    void DetachBall() {
        isDragging = false;
        currentBallRigidBody = null;
        currentBallSpringJoint.enabled = false;
        Invoke("Respawn", respawnTime);
    }

    void Respawn() {
        isLaunching = false;
        currentBallSpringJoint.enabled = true;
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);
        currentBallRigidBody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint.connectedBody = currentBallRigidBody;
        ballCollider = ballInstance.GetComponent<CircleCollider2D>();
        gravityScale = currentBallRigidBody.gravityScale;
        currentBallRigidBody.gravityScale = 0;
        currentBallRigidBody.isKinematic = true;
    }

}
