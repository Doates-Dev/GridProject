using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioSource src;
    public AudioClip KickSound, NetSound;
    // Start is called before the first frame update
    public void KickSoundMethod()
    {
        src.clip = KickSound;
        src.Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
