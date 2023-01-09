using UnityEngine;

public class MusicScript : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource musicSource2;
    public AudioClip musicStart;
    public bool switchSong;

    void Start()
    {
        musicSource.PlayOneShot(musicStart);
        musicSource.PlayScheduled(AudioSettings.dspTime + 12);
    }
    void Update()
    {
        if (musicSource.time > 144 && !switchSong)
        {
            musicSource2.Play();
            switchSong = true;
        }

        if (musicSource2.time > 144 && switchSong)
        {
            musicSource.Play();
            switchSong = false;
        }
    }
}