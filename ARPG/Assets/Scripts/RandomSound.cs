using UnityEngine;

    public static class RandomSound
    {
        public static AudioClip GetRandomAudioClip(AudioClip[] audioClips)
        {
            // Select a random index from the array
            int randomIndex = Random.Range(0, audioClips.Length);

            // Return the audio clip at the random index
            return audioClips[randomIndex];
        }
    }