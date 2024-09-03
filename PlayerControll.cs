using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class PlayerControll : MonoBehaviour, IMixedRealityPointerHandler
{
    public GameObject player; 
    public LayerMask layerMask; 
    private Rigidbody playerRigidbody;
    private Vector3 targetPosition;
    private bool Flag_Player_Moving = false;

    private void Start()
    {
        playerRigidbody = player.GetComponent<Rigidbody>();
        Flag_Player_Moving = false;
    }


    private void FixedUpdate()
    {
        Vector3 player_position = player.transform.position;
        float distance = Vector3.Distance(player_position, targetPosition);

        if (Flag_Player_Moving) // if the player is moving we check all the time if he gets to the target point
        {
            if (targetPosition.y < 0.5f) // we make sure that the player didnt fall from the plane 
            {
                targetPosition.y = 0.5f;

            }
            if (distance <= 0.3f) // 0.3f is a threshold that we gave the player to stop when he is moving towards the target position
            {
                Freeze(); // a function that makes sure that the player stops and update its position stats
            }

        }
        else
        {
            Freeze(); // a function that makes sure that the player stops and update its position stats

        }
    }

    private void Freeze() // a function that makes sure that the player stops and update its position stats
    {
        playerRigidbody.position = new Vector3(targetPosition.x, 0.5f, targetPosition.z);
        player.transform.position = new Vector3(targetPosition.x, 0.5f, targetPosition.z);
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
        playerRigidbody.rotation = Quaternion.identity;
        Flag_Player_Moving = false;
    }
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (Flag_Player_Moving == false)
        {
            print("OnPointerClicked: ball");
            Ray ray = new Ray(eventData.Pointer.Position, eventData.Pointer.Rotation * Vector3.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                targetPosition = hit.point;
                Vector3 player_position = player.transform.position;
                Vector3 direction = (targetPosition - player_position).normalized;
                playerRigidbody.AddForce(direction * 10f, ForceMode.Impulse);
                Flag_Player_Moving = true;
            }
        }
       
    }
    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
    }
    public void OnPointerDown(MixedRealityPointerEventData eventData) { }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }
}
