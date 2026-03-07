using System.Runtime.InteropServices;
using TypTyp;
using UnityEngine;
using UnityEngine.UI;

public class CapsLockedWarning : MonoBehaviour
{
    private Image warning;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

    [DllImport("user32.dll")]
    private static extern short GetKeyState(int nVirtKey);

    private const int VK_CAPITAL = 0x14;

    public static bool IsCapsLockOn()
    {
        return (GetKeyState(VK_CAPITAL) & 0x0001) != 0;
    }

#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX

    [DllImport("libX11")]
    private static extern IntPtr XOpenDisplay(IntPtr display);

    [DllImport("libX11")]
    private static extern int XCloseDisplay(IntPtr display);

    [DllImport("libX11")]
    private static extern int XkbGetIndicatorState(IntPtr display, uint deviceSpec, out uint state);

    private const uint XkbUseCoreKbd = 0x0100;

    public static bool IsCapsLockOn()
    {
        IntPtr display = XOpenDisplay(IntPtr.Zero);
        if (display == IntPtr.Zero)
            return false;

        uint state;
        XkbGetIndicatorState(display, XkbUseCoreKbd, out state);
        XCloseDisplay(display);

        // Bit 0 = Caps Lock
        return (state & 0x01) != 0;
    }

#endif

    private void Awake()
    {
        warning = GetComponent<Image>();
    }

    void Update()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        warning.enabled = Settings.Instance.CapsLockWarning && IsCapsLockOn();
#endif
    }
}