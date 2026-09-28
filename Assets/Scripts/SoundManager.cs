using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;    // дл€ коротких звуков
    public AudioSource musicSource;  // дл€ фоновой музыки

    [Header("Clips")]
    public AudioClip walletSound;
    public AudioClip insertCoinSound;
    public AudioClip winSound;

    void Awake()
    {
        // —инглтон Ч чтобы вызывать из любого скрипта
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayWallet() => sfxSource.PlayOneShot(walletSound);
    public void PlayInsertCoin() => sfxSource.PlayOneShot(insertCoinSound);
    public void PlayWin() => sfxSource.PlayOneShot(winSound);
}