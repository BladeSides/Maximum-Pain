using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class Ambience : MonoBehaviour
{
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }
}
