using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyValue
{
    public GameObject slot;
    public GameObject boxObj;
}

public class PackageSlotsManager : MonoBehaviour
{
    //public GameObject[] slots;
    public List<KeyValue> slots;
    public int currentSlot;

    public void SetSlot(GameObject box)
    {
        foreach (var slot in slots)
        {
            if (slot.boxObj != null) continue;

            slot.boxObj = box;
            box.transform.position = slot.slot.transform.position;
            break;
        }
    }
}
