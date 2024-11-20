using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PackageController : MonoBehaviour
{
    LevelManager levelManager;

    private Coroutine packageControllerCoroutine;

    private void Start()
    {
        levelManager = LevelManager.Instance;

        packageControllerCoroutine = StartCoroutine(CheckBoxes(levelManager.slotsPlatformManager));
    }

    private IEnumerator CheckBoxes(SlotsPlatformManager slotsPlatformManager)
    {
        List<Can> cansList = levelManager.cansGridManager.cans;

        while (true)
        {
            List<KeyValue> slots = slotsPlatformManager.slots;

            foreach (var slot in slots)
            {
                if (slot.Obj == null) continue;

                if (slot.Obj.TryGetComponent<Box>(out var currentBox))
                {
                    if (currentBox == null) continue;

                    if (currentBox.slotsManager.AreSlotsFull())
                    {
                        currentBox.slotsManager.OnSlotsFull(currentBox);
                        Debug.Log("box is full");
                        continue;
                    }

                    if (currentBox.GetColor() == cansList[0].GetColor())
                    {
                        currentBox.slotsManager.SetSlot(cansList[0].gameObject);
                        cansList.RemoveAt(0);
                        levelManager.cansGridManager.ArrangeCans();

                        Debug.Log("Found same color box, placing can!");
                    }
                }
            }

            //Debug.Log("looping......!");

            yield return new WaitForSeconds(0.3f);
        }
    }

    public void RestartCoroutine()
    {
        StopCoroutine(packageControllerCoroutine);
        StartCoroutine(CheckBoxes(levelManager.slotsPlatformManager));
    }
}
