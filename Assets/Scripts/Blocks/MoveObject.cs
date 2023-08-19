using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour, IExecutable<bool>
{
    public GameObject targetObject;
    public MoveDirection moveDirection; // Enum to select direction in the Inspector
    public float moveDistance = 1.0f;   // Distance to move in the selected direction

    public bool Execute()
    {
        if (targetObject != null)
        {
            Vector3 moveVector = Vector3.zero;

            // Determine the moveVector based on the selected direction
            switch (moveDirection)
            {
                case MoveDirection.Up:
                    moveVector = Vector3.up;
                    break;
                case MoveDirection.Down:
                    moveVector = Vector3.down;
                    break;
                case MoveDirection.Left:
                    moveVector = Vector3.left;
                    break;
                case MoveDirection.Right:
                    moveVector = Vector3.right;
                    break;
            }

            // Move the targetObject in the specified direction by the moveDistance
            targetObject.transform.position += moveVector * moveDistance;

            // Return true to indicate successful execution
            return true;
        }

        // Return false if execution couldn't be completed
        return false;
    }


}
