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
    public float moveDuration = 0.5f;
    public bool isSlotted = false;
    public List<KeyValue> slots;

    public virtual void SetSlot(GameObject obj)
    {
        foreach (var slot in slots)
        {
            if (slot.Obj != null) continue;

            slot.Obj = obj;
            Vector3 ObjOriginalScale = obj.transform.lossyScale;

            obj.transform.SetParent(slot.Slot.transform);
            //obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            Vector3 parentScale = slot.Slot.transform.lossyScale;
            obj.transform.localScale = new Vector3(
                ObjOriginalScale.x / parentScale.x,
                ObjOriginalScale.y / parentScale.y,
                ObjOriginalScale.z / parentScale.z
            );

            SetSlotAnimation(obj, slot.Slot.transform);

            break;
        }
    }

    public virtual void SetSlotAnimation(GameObject obj, Transform targetPosition)
    {
        LeanTween.move(obj, targetPosition.position, moveDuration)
            .setEaseOutQuad()
            .setOnComplete(() =>
            {
                obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                LevelManager.Instance.packageController.OnSlotFilled.Invoke();
            });
    }

    public bool AreSlotsFull()
    {
        return slots.All(slot => slot.Obj != null);
    }

    public virtual void OnSlotsFull(Box box)
    {

    }
}
