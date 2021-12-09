using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Button : Interactable
{
    public ButtonActivated buttonRecipient;

    public override void Interacted()
    {
        buttonRecipient.Activated();
    }
}
