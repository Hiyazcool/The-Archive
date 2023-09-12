using UnityEngine.InputSystem;
using Hiyazcool;
using Hiyazcool.Unity;
/*
 * Finish Implementing Various Methods 
 * Main use is to control flow of game
 * Prevent Multiple scripts having different Inputs at the same time
 *      everything should use this as the intermidiary
 * 
 */
public class InputHandler : MonoBehaviourSingleton<InputHandler>
{
    public static InputKey gameInput {  get; private set; }
    #region Main
    void Start()
    {
        gameInput = new InputKey();
        gameInput.Enable();
        gameInput.Mouse.Enable();
        gameInput.CharacterInput.Enable();
        gameInput.CharacterInput.MovementControls.Enable();
        
    }
    void Update()
    {
        
    }

    #endregion
    /* Implementation
     * 
     * Custom Events that display information related to what KeyMap is enabled such as the Vector3 on where the mouse clicked/ Which Diection it is
     * State manager
     * need to make this class persistent
     */
}
