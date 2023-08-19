using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutionManager : MonoBehaviour
{
    public GameObject firstObject; // Assign the first object in the chain in the Inspector

    private IExecutable<bool> currentExecutable;

    public void OnPlayButtonClick()
    {
        // Start the execution chain
        if (firstObject != null)
        {
            currentExecutable = firstObject.GetComponent<IExecutable<bool>>();
            ExecuteNext();
        }
    }

    public void ExecuteNext()
    {
        // Execute the current object's logic
        if (currentExecutable != null)
        {
            bool shouldProceed = currentExecutable.Execute();

            // Check if the current executable implements IHasNextExecutable<bool>
            if (shouldProceed && currentExecutable is IHasNextExecutable<bool> nextExecutable)
            {
                IExecutable<bool> nextObject = nextExecutable.GetNextExecutable();
                if (nextObject != null)
                {
                    currentExecutable = nextObject;
                    ExecuteNext();
                    return;
                }
            }
            else if(!shouldProceed)
            {
                throw new InvalidOperationException("Condition failed");
            }
        }


    }

    //private IExecutable<bool> GetNextExecutable(IExecutable<bool> current)
    //{
    //    GameObject nextObject = current.GetNextObject();

    //    if (nextObject != null)
    //    {
    //        return nextObject.GetComponent<IExecutable<bool>>();
    //    }

    //    return null;
    //}
}