using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PackageController : MonoBehaviour
{
    public UnityEvent OnSlotFilled;

    LevelManager levelManager;
    private List<Can> cansList;
    private SlotsPlatformManager slotsPlatformManager;

    private void Start()
    {
        levelManager = LevelManager.Instance;
        slotsPlatformManager = levelManager.slotsPlatformManager;

        OnSlotFilled = new UnityEvent();
        OnSlotFilled.AddListener(CheckBoxes);
    }

    private void CheckBoxes()
    {
        List<KeyValue> slots = slotsPlatformManager.slots;
        cansList = levelManager.cansGridManager.cans;

        foreach (var slot in slots)
        {
            if (slot.Obj == null) continue;

            if (slot.Obj.TryGetComponent<Box>(out var currentBox))
            {
                BoxSlotsManager boxSlotsManager = currentBox.slotsManager;

                if (currentBox.GetColor() == cansList[0].GetColor())
                {
                    currentBox.slotsManager.SetSlot(cansList[0].gameObject);

                    cansList.RemoveAt(0);
                    levelManager.cansGridManager.ArrangeCans(true);

                    Debug.Log("Found same color box, placing can!");
                }

                if (boxSlotsManager.AreSlotsFull())
                {
                    currentBox.slotsManager.OnSlotsFull(currentBox);
                    Debug.Log("Box is full!");
                }
            }
        }
    }
}
