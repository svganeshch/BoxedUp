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
            Vector3 ObjOriginalScale = obj.transform.lossyScale;

            obj.transform.SetParent(slot.Slot.transform);
            obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            Vector3 parentScale = slot.Slot.transform.lossyScale;
            obj.transform.localScale = new Vector3(
                ObjOriginalScale.x / parentScale.x,
                ObjOriginalScale.y / parentScale.y,
                ObjOriginalScale.z / parentScale.z
            );
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
