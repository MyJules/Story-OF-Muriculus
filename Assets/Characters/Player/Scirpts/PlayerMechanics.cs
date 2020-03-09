using UnityEngine;

public class PlayerMechanics : MonoBehaviour
{
    public void Grabbing(PlayerMechanicsData inputData, ref IGrabbable grabbable) 
    {
        if (grabbable != null)
        {
            if (inputData.isMovableObjGrabbed)
            {
                grabbable.Grab();
            }
            else if (!inputData.isMovableObjGrabbed)
            {
                grabbable.Release();
                grabbable = null;
            }
        }

    }
}
