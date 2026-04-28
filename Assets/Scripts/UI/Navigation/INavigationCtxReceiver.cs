
using UnityEngine;

public interface INavigationCtxReceiver
{
    public void ReceiveContext(Screens prevScreen, bool isGoingBack, GameObject sender = null);
}
