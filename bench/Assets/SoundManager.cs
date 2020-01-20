﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    [SerializeField] private AudioSource sfxSorce;
    [SerializeField] private AudioSource sndSource;

    public static SoundManager instance = null;
    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;



    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySingle(AudioClip clip) {

        sfxSorce.clip = clip;
        sfxSorce.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips) {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        sfxSorce.pitch = randomPitch;
        sfxSorce.clip = clips[randomIndex];

        sfxSorce.Play();
    }
}
