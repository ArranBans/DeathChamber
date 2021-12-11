using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour, IInteractable
{
    public GameObject buttonRecipient;

    public void Interacted()
    {
        IButtonActivated buttonActivated = (IButtonActivated)buttonRecipient.GetComponent(typeof(IButtonActivated));
        buttonActivated.Activated();
    }
}
