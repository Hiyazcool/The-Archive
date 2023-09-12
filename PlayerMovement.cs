using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Hiyazcool;

public class PlayerMovement : MonoBehaviour
{
    /*
     * Refactor this eventually but for now finish implementing mechanics 
     */
    protected Rigidbody2D characterRigidbody2D;
    protected BoxCollider2D boxCollider2D;
    protected Vector2 inputVector;
    [SerializeField]
    protected float movementSpeed;
    void Start()
    {
        ResetComponent();
    }
    void Update()
    {
        inputVector = InputHandler.gameInput.CharacterInput.MovementControls.ReadValue<Vector2>();
        characterRigidbody2D.MovePosition(
            transform.position + new Vector3(inputVector.x, inputVector.y) * movementSpeed * Time.deltaTime
            );
    }
    protected void ResetComponent()
    {
        characterRigidbody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    protected void MovementPerformed(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }
}
