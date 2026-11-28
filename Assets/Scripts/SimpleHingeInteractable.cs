using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleHingeInteractable : XRSimpleInteractable
{
    private Transform grabHand;

    protected virtual void Update()
    {
        if (grabHand != null)
            transform.LookAt(grabHand, Vector3.up);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        grabHand = args.interactorObject.transform;
        Debug.Log("[Hinge] Selected by " + grabHand.name);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        Debug.Log("[Hinge] Released");
        grabHand = null;
    }
}
