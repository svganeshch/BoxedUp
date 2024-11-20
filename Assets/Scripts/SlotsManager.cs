using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class KeyValue
{
    public GameObject Slot;
    public GameObject Obj;
}

public class SlotsManager : MonoBehaviour
{
    public List<KeyValue> slots;

    public virtual void SetSlot(GameObject obj)
    {
        foreach (var slot in slots)
        {
            if (slot.Obj != null) continue;

            slot.Obj = obj;
            //obj.transform.position = slot.Slot.transform.position;
            obj.transform.SetParent(slot.Slot.transform);
            obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            break;
        }
    }

    public bool AreSlotsFull()
    {
        return slots.All(slot => slot.Obj != null);
    }

    public virtual void OnSlotsFull(Box box)
    {

    }
}
