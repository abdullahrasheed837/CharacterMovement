using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    int isWalkingHash;
    int isRunningHash;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isRunPressed;
    float rotationFactor=15.0f;

    void Awake()
    {
      
    playerInput=new PlayerInput();
    characterController=GetComponent<CharacterController>();
    animator=GetComponent<Animator>();
    isWalkingHash=Animator.StringToHash("isWalking");
    isRunningHash=Animator.StringToHash("isRunning");


            playerInput.CharacterControls.Move.started +=onMovementInput;
            playerInput.CharacterControls.Move.canceled +=onMovementInput; 
            playerInput.CharacterControls.Move.performed += onMovementInput;
            playerInput.CharacterControls.Run.started+=onRun;
            playerInput.CharacterControls.Run.canceled+=onRun;
    }
    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed=context.ReadValueAsButton();
    }
    void handleRotation(){
        Vector3 position;
        position.x=currentMovement.x;
        position.y=0.0f;
        position.z=currentMovement.z;
        Quaternion currentRotation=transform.rotation;
        if( isMovementPressed){
            Quaternion targetRotation=Quaternion.LookRotation(position);
            transform.rotation=Quaternion.Slerp(currentRotation,targetRotation,rotationFactor*Time.deltaTime);
        }
        


    }
    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput=context.ReadValue<Vector2>();
        currentMovement.x=currentMovementInput.x*15.0f;
        currentMovement.z=currentMovementInput.y*15.0f;
        currentRunMovement.x=currentMovementInput.x*45.0f;
        currentRunMovement.z=currentMovementInput.y*45.0f;
        isMovementPressed=currentMovementInput.x!=0 || currentMovementInput.y !=0;
    }

    void handleAnimation()
    {
        bool isWalking=animator.GetBool(isWalkingHash);
        bool isRunning=animator.GetBool(isRunningHash);

        if(isMovementPressed && !isWalking){
            animator.SetBool(isWalkingHash,true);
        }
        else if(!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash,false);
        }
        if((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash,true);
        }
        else if((!isMovementPressed || !isRunPressed) && isRunning){
            animator.SetBool(isRunningHash,false);
        }
    }

    void handleGravity(){
        if(characterController.isGrounded){
            float groundedGravity=-0.5f;
            currentMovement.y=groundedGravity;
            currentRunMovement.y=groundedGravity;
        }
        else{
            float gravity=-9.8f;
            currentMovement.y+=gravity;
            currentRunMovement.y+=gravity;
        }
    }
    // Update is called once per frame
    void Update()
    {
        handleGravity();
        handleRotation();
        handleAnimation();
        if(isRunPressed){
            characterController.Move(currentRunMovement*Time.deltaTime);
        }
        else{
        characterController.Move(currentMovement*Time.deltaTime);
        }

    }
    void OnEnable()
    {
       playerInput.CharacterControls.Enable();
    }
    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
