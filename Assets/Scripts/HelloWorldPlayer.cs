using Unity.Netcode;
using UnityEngine;

public class HelloWorldPlayer : NetworkBehaviour
{
    private CharacterController characterController;

    private Animator animator;

    private Transform playerCamera;

    public float walkingSpeed = 2f;
    public float runningSpeed = 4f;

    public float gravity = -9.8f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerCamera = FindFirstObjectByType<Camera>().transform;
    }

    [ServerRpc]
    void AnimatorSetTriggerServerRpc(string triggerName)
    {
        AnimatorSetTriggerClientRpc(triggerName);
    }

    [ClientRpc]
    void AnimatorSetTriggerClientRpc(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }


    [ServerRpc]
    void AnimatorSetBoolServerRpc(string name, bool value)
    {
        AnimatorSetBoolClientRpc(name, value);
    }

    [ClientRpc]
    void AnimatorSetBoolClientRpc(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    [ServerRpc]
    void AnimatorSetPosServerRpc(float posX, float posY)
    {
        AnimatorSetPosClientRpc(posX, posY);
    }

    [ClientRpc]
    void AnimatorSetPosClientRpc(float posX, float posY)
    {
        animator.SetFloat("PosX", posX);
        animator.SetFloat("PosY", posY);
    }

    bool lastPositionSentWasZero = true;

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;

        float posX = Input.GetAxis("Horizontal");
        float posY = Input.GetAxis("Vertical");

        bool isZero = posX == 0 && posY == 0;

        // If the last position sent was (0, 0), don't send anything
        // more until we move again. This is to avoid spamming (0, 0) positions
        // to the server while the player is not moving.
        if ((isZero && !lastPositionSentWasZero) || !isZero)
        {
            lastPositionSentWasZero = isZero;

            AnimatorSetPosServerRpc(posX, posY);
        }

        bool isRunning = Input.GetButton("Fire3");

        AnimatorSetBoolServerRpc("Running", isRunning);

        Vector3 movement = Vector3.zero;

        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Punching"
            && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Standing React Death Backward"
            && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Hip Hop Dancing"
            && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Rumba Dancing"
            && (posX != 0 || posY != 0))
        {
            Vector3 forward = playerCamera.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = playerCamera.right;
            right.y = 0;
            right.Normalize();

            Vector3 direction = forward * posY + right * posX;
            direction.Normalize();

            movement = (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.StartsWith("Running") ? runningSpeed : walkingSpeed) * Time.deltaTime * direction;
        }

        if (Input.GetButtonDown("Jump"))
            AnimatorSetTriggerServerRpc("Jump");
        else if (Input.GetButtonDown("Submit"))
            AnimatorSetTriggerServerRpc("Hip Hop");
        else if (Input.GetButtonDown("Fire2"))
            AnimatorSetTriggerServerRpc("Rumba");
        else if (Input.GetButtonDown("Enable Debug Button 2"))
            AnimatorSetTriggerServerRpc("Dead");
        else if (Input.GetButtonDown("Fire1"))
        {
            movement = Vector3.zero;
            AnimatorSetTriggerServerRpc("Punch");
        }

        movement.y += gravity * Time.deltaTime;
        characterController.Move(movement);
    }
}
