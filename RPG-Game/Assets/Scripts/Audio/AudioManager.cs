using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    public List<Sound> sounds;

    // Use this for initialization
    void Awake() {
        if ( instance == null )
            instance = this;

        DontDestroyOnLoad( instance );

        sounds.ForEach( s => {
            s.Sources.Add( gameObject.AddComponent<AudioSource>() );

            s.Sources[ 0 ].clip = s.Clip;
            s.Sources[ 0 ].volume = s.Volume;
            s.Sources[ 0 ].pitch = s.Pitch;
        } );
    }

    public void Play( string soundName ) { Play( soundName, false ); }
    public void Play( string soundName, bool loop ) {
        List<Sound> sArr = sounds.Where( sound => sound.Name == soundName ).ToList();

        if ( sArr.Count > 0 )
            sArr.ForEach( s => {
                s.Sources[ 0 ].loop = loop;
                s.Sources[ 0 ].Play();
            } );
        else
            Debug.LogWarning( "Could not play sound: \"" + soundName + "\" does not exist." );
    }

    public void Stop( string soundName ) {
        List<Sound> sArr = sounds.Where( sound => sound.Name == soundName ).ToList();

        if ( sArr.Count > 0 )
            sArr.ForEach( s => s.Sources[ 0 ].Stop() );
        else
            Debug.LogWarning( "Could not stop sound: \"" + soundName + "\" does not exist." );
    }
}
