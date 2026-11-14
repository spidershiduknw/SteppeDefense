using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Background Music")]
    [SerializeField] private AudioClip bgmSteppe;

    [Header("Gameplay SFX")]
    [SerializeField] private AudioClip sfxAttack;
    [SerializeField] private AudioClip sfxEnemyHit;
    [SerializeField] private AudioClip sfxEnemyDeath;
    [SerializeField] private AudioClip sfxYurtDamage;

    [Header("Collectible & UI SFX")]
    [SerializeField] private AudioClip sfxPickupAaruul;
    [SerializeField] private AudioClip sfxPickupBell;
    [SerializeField] private AudioClip sfxButtonClick;
    [SerializeField] private AudioClip sfxGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgmSource == null || bgmSteppe == null) return;
        bgmSource.clip = bgmSteppe;
        bgmSource.loop = true;
        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    // Play SFX with slight random pitch variation so repeated pops/swishes stay cute and fresh
    private void PlayClip(AudioClip clip, float pitchVariance = 0.08f)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
        sfxSource.PlayOneShot(clip);
    }

    public void PlayAttack() => PlayClip(sfxAttack, 0.08f);
    public void PlayEnemyHit() => PlayClip(sfxEnemyHit, 0.10f);
    public void PlayEnemyDeath() => PlayClip(sfxEnemyDeath, 0.12f);
    public void PlayYurtDamage() => PlayClip(sfxYurtDamage, 0.05f);
    public void PlayPickupAaruul() => PlayClip(sfxPickupAaruul, 0.06f);
    public void PlayPickupBell() => PlayClip(sfxPickupBell, 0.05f);
    public void PlayButtonClick() => PlayClip(sfxButtonClick, 0.04f);
    public void PlayGameOver() => PlayClip(sfxGameOver, 0f);
}