using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Sound : Sound_base {

    public AudioClip Clip;

    public void Play() { Play( gameObject.AddComponent<AudioSource>() ); }
    public override void Play( AudioSource source ) {
        if ( source == null ) source = gameObject.AddComponent<AudioSource>();
        source.clip = Clip;
        base.Play( source );
    }
}

[System.Serializable]
public class SoundMix : Sound_base {

    public List<AudioClip> Clips;

    public void Play() { Play( gameObject.AddComponent<AudioSource>() ); }
    public override void Play( AudioSource source ) {
        AudioClip clip;
        if ( Clips.Count > 0 )
            clip = Clips[ Random.Range( 0, Clips.Count ) ];
        else {
            Debug.LogWarning( "Could not play sound: Clips-array does not contain any clips." );
            return;
        }

        if ( source == null ) source = gameObject.AddComponent<AudioSource>();

        source.clip = clip;
        base.Play( source );
    }
}

[System.Serializable]
public class Sound_base {

    public GameObject gameObject;
    public string Name;

    [Range( 0f, 1f )]
    public float Volume = 1f;
    [Range( .1f, 3f )]
    public float Pitch = 1f;

    [Range( 0f, 1f )]
    public float VolumeVariance = 0f;
    [Range( 0f, 1f )]
    public float PitchVariance = 0f;

    public bool Loop = false;

    [HideInInspector]
    public List<AudioSource> Sources;

    public virtual void Play( AudioSource source ) {
        source.volume = Volume + Random.Range( -VolumeVariance, VolumeVariance );
        source.pitch = Pitch + Random.Range( -PitchVariance, PitchVariance );

        source.Play();
        Sources.Add( source );

        // Remove all non-playing sources
        Sources.Where( s => !s.isPlaying ).ToList().ForEach( s => {
            Sources.Remove( s );
            Object.Destroy( s );
        } );
    }

    public virtual void Stop() {
        Sources.ForEach( s => {
            s.Stop();
            Object.Destroy( s );
        } );
    }

}
