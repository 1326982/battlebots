//------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------
// NOM DE LA CLASSE: SNDFXController
// UTILITÉ: Permet d'avoir une fonction semblable a PlayOneShot() plus adaptée a l'univers 2d 
//------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SNDFXController : MonoBehaviour {

	public static SNDFXController instance; // l'instance du component

	[SerializeField] private AudioSource[] _audioSources; // array d'audioSource (des channels)

//------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------
// NOM DE LA FONCTION: Awake
// UTILITÉ: lancé a l'initialisation
//------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------
	void Awake () {
		if(instance != null){
			Destroy(gameObject);
			for(int i=0;i<20;i++){
				gameObject.AddComponent<AudioSource>();
			}
		} else{
			instance = this;
		}
	}

//------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------
// NOM DE LA FONCTION: JouerFX
// UTILITÉ: Permet de jouer un effet sonore de n'importe quelle classe
//------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------
	public void JouerFX(AudioClip Clip , float Volume) {
		AudioSource audioSource = trouverSourceVide();
		if(audioSource != null) {
			audioSource.volume = Volume;
			audioSource.clip = Clip;
			audioSource.Play();	
		}
	}
//------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------
// NOM DE LA FONCTION: trouverSourceVide
// UTILITÉ: Trouve un channel vide pour jouer le FX, si aucun channel est dispo le son est oublié
//------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------
	private AudioSource trouverSourceVide(){
		foreach(AudioSource audio in _audioSources) {
			if(audio.isPlaying == false) {
				return audio;
			}
		}
		return null;
	}
}
