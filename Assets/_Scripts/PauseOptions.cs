using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PauseOptions : Options
{
	protected FPSCharacter player;

	new protected void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<FPSCharacter>();
		base.Awake();
	}

	public override void SetFieldOfView(float fov)
	{
		FieldOfView = fov;
		fovValue.text = fov.ToString();
		Camera.main.fieldOfView = fov;
		PlayerPrefs.SetFloat("FieldOfView", fov);
		player.walkFOV = fov;
		player.sprintFOV = fov + 10f;
	}
	public override void SetInvertMouse(bool inverted)
	{
		IsInverted = inverted;
		PlayerPrefs.SetInt("IsInverted", Convert.ToInt32(IsInverted));
		player.invertLook = inverted;
	}

	public override void SetSensitivity(float sensitivity)
	{
		Sensitivity = sensitivity;
		sensitivityValue.text = sensitivity.ToString("P0");
		PlayerPrefs.SetFloat("Sensitivity", Sensitivity);
		player.sensitivity = player.baseSensitivity * sensitivity;
	}

	public override void SetMasterVolume(float volume)
	{
		masterVolume = volume;
		masterValue.text = volume.ToString("P0");
		PlayerPrefs.SetFloat("MasterVolume", masterVolume);
	}
	public override void SetMusicVolume(float volume)
	{
		musicVolume = volume;
		musicValue.text = volume.ToString("P0");
		PlayerPrefs.SetFloat("MusicVolume", musicVolume);
	}
	public override void SetSoundEffectsVolume(float volume)
	{
		soundEffectVolume = volume;
		soundEffectValue.text = volume.ToString("P0");
		PlayerPrefs.SetFloat("SoundEffectVolume", soundEffectVolume);
	}
	public override void SetVoiceVolume(float volume)
	{
		voiceVolume = volume;
		voiceValue.text = volume.ToString("P0");
		PlayerPrefs.SetFloat("VoiceVolume", voiceVolume);
	}
}
