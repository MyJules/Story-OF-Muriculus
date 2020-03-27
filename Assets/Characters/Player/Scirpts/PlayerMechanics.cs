using UnityEngine;

public class PlayerMechanics : MonoBehaviour
{
    public void Grabbing(PlayerMechanicsData inputData, ref IGrabbable grabbableObj) 
    {
        if (grabbableObj != null)
        {
            if (inputData.isMovableObjGrabbed == true)
            {
                grabbableObj.Grab();
            }
            else if (inputData.isMovableObjGrabbed == false)
            {
                grabbableObj.Release();
                grabbableObj = null;
            }
        }
    }
}
