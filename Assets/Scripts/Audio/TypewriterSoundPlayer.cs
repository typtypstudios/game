using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class TypewriterSoundPlayer : MonoBehaviour
{
    [Header("Key Sounds")]
    [SerializeField] private AudioClip[] keyClips;

    [Header("Volume Variation")]
    [SerializeField, Range(0f, 1f)] private float minVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float maxVolume = 0.7f;

    [Header("Pitch Variation")]
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.1f;

    [Header("Scene")]
    [SerializeField] private string menuSceneName = "MainMenu";

    private AudioSource audioSource;
    private bool isInMenu;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void Start()
    {
        if (AudioManager.Instance != null)
            audioSource.outputAudioMixerGroup = AudioManager.Instance.KeyGroup;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        isInMenu = SceneManager.GetActiveScene().name == menuSceneName;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isInMenu = scene.name == menuSceneName;
    }

    private void Update()
    {
        if (!isInMenu) return;
        if (Keyboard.current == null) return;

        foreach (KeyControl key in Keyboard.current.allKeys)
        {
            if (key.wasPressedThisFrame)
            {
                PlayKeySound();
                return;
            }
        }
    }

    private void PlayKeySound()
    {
        if (keyClips == null || keyClips.Length == 0) return;

        AudioClip clip = keyClips[Random.Range(0, keyClips.Length)];
        if (clip == null) return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        float vol = Random.Range(minVolume, maxVolume);
        audioSource.PlayOneShot(clip, vol);
    }
}