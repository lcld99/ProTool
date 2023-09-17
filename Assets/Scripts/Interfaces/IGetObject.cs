using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGetObject
{
    IGetObject GetSelf();
    public object GetPropertyValue(string propertypropertyName);
    public List<string> GetFieldNames();
}
