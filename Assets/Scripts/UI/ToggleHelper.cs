namespace UnityEngine.UI
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleHelper : MonoBehaviour
    {
        [SerializeField] Toggle toggle;

        void Awake()
        {
            if (!toggle) toggle = GetComponent<Toggle>();
        }

        public void ToggleValue()
        {
            toggle.isOn = !toggle.isOn;
        }
    }
}