using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasNextExecutable<T>
{
    IExecutable<T> GetNextExecutable();
}


