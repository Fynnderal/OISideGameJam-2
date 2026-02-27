using KBCore.Refs;
using UnityEngine;


/// <summary>
/// Solves an issue with moving platforms affecting player velocity incorrectly.
/// Script must be executer after PlayerController's FixedUpdate.
/// </summary>
public class MovingPlatformWorkAround : ValidatedMonoBehaviour
{
    [SerializeField, Self] MovingPlatform movingPlatform;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    private void FixedUpdate()
    {
        if (movingPlatform.Player != null)
        {
            //Resets the player's velocity change caused by the moving platform

            if (movingPlatform.Direction == MoveDirection.HORIZONTAL || movingPlatform.Direction == MoveDirection.BOTH)
                movingPlatform.Player.linearVelocityX -= movingPlatform.Speed; 
            if (movingPlatform.Direction == MoveDirection.VERTICAL || movingPlatform.Direction == MoveDirection.BOTH)
                movingPlatform.Player.linearVelocityY -= movingPlatform.Speed;
        }
    }
}
