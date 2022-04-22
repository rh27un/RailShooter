using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
public class Options : MonoBehaviour
{
	public bool IsInverted { get; set; }
	public float Sensitivity { get; set; }
	public bool IsFullscreen { get; set; }
	public float FieldOfView { get; set; }

	[SerializeField]
	GameObject gameOptions;
	[SerializeField]
	GameObject videoOptions;
	[SerializeField]
	GameObject audioOptions;

	[SerializeField]
	Button gameButton;
	[SerializeField]
	Button videoButton;
	[SerializeField]
	Button audioButton;

	public Toggle isInvertedToggle;
	public Toggle isFullscreenToggle;

	public TMP_Dropdown resolutionDropdown;
	public TMP_Dropdown qualityDropdown;
	public TMP_Dropdown textureDropDown;
	public TMP_Dropdown aaDropdown;

	public Slider masterVolumeSlider;
	public Slider musicVolumeSlider;
	public Slider soundEffectVolumeSlider;
	public Slider voiceVolumeSlider;
	public Slider fieldOfViewSlider;
	public Slider sensitivitySlider;

	public TMP_Text sensitivityValue;
	public TMP_Text fovValue;
	public TMP_Text masterValue;
	public TMP_Text musicValue;
	public TMP_Text soundEffectValue;
	public TMP_Text voiceValue;

	public AudioMixer audioMixer;
	public float masterVolume;
	public float musicVolume;
	public float soundEffectVolume;
	public float voiceVolume;

	protected Resolution[] resolutions;

	protected void Awake()
	{
		//DontDestroyOnLoad(gameObject);
		

		resolutionDropdown.ClearOptions();
		List<string> options = new List<string>();
		resolutions = Screen.resolutions;
		options = resolutions.Select(r => $"{r.width} x {r.height}").ToList();
		resolutionDropdown.AddOptions(options);

		if (PlayerPrefs.HasKey("FieldOfView"))
		{
			FieldOfView = PlayerPrefs.GetFloat("FieldOfView");
			fieldOfViewSlider.value = FieldOfView;
		} else
		{
			fieldOfViewSlider.value = 90f;
			PlayerPrefs.SetFloat("FieldOfView", 90f);
		}
		if (PlayerPrefs.HasKey("IsInverted"))
		{
			IsInverted = Convert.ToBoolean(PlayerPrefs.GetInt("IsInverted"));
			isInvertedToggle.isOn = IsInverted;
		} else
		{
			isInvertedToggle.isOn = false;
			PlayerPrefs.SetInt("IsInverted", Convert.ToInt32(false));
		}
		if (PlayerPrefs.HasKey("Sensitivity"))
		{
			Sensitivity = PlayerPrefs.GetFloat("Sensitivity");
			sensitivitySlider.value = Sensitivity;
		} else
		{
			sensitivitySlider.value = 1f;
			PlayerPrefs.SetFloat("Sensitivity", 1f);
		}
		if (PlayerPrefs.HasKey("MasterVolume"))
		{
			masterVolume = PlayerPrefs.GetFloat("MasterVolume");
			masterVolumeSlider.value = masterVolume;
		} else
		{
			masterVolumeSlider.value = 1f;
			PlayerPrefs.SetFloat("MasterVolume", 1f);
		}
		if (PlayerPrefs.HasKey("MusicVolume"))
		{
			musicVolume = PlayerPrefs.GetFloat("MusicVolume");
			musicVolumeSlider.value = musicVolume;
		} else
		{
			musicVolumeSlider.value = 1f;
			PlayerPrefs.SetFloat("MusicVolume", 1f);
		}
		if (PlayerPrefs.HasKey("SoundEffectVolume"))
		{
			soundEffectVolume = PlayerPrefs.GetFloat("SoundEffectVolume");
			soundEffectVolumeSlider.value = soundEffectVolume;
		} else
		{
			soundEffectVolumeSlider.value = 1f;
			PlayerPrefs.SetFloat("SoundEffectVolume", 1f);
		}
		if (PlayerPrefs.HasKey("VoiceVolume"))
		{
			voiceVolume = PlayerPrefs.GetFloat("VoiceVolume");
			voiceVolumeSlider.value = voiceVolume;
		} else
		{
			voiceVolumeSlider.value = 1f;
			PlayerPrefs.SetFloat("VoiceVolume", 1f);
		}
		if (PlayerPrefs.HasKey("Resolution"))
		{
			int resolutionIndex = PlayerPrefs.GetInt("Resolution");
			var resolution = resolutions[resolutionIndex];
			Screen.SetResolution(resolution.width, resolution.height, IsFullscreen);
			resolutionDropdown.value = resolutionIndex;
		} 
		else
		{
			var res = Screen.currentResolution;
			resolutionDropdown.value = Array.FindIndex(resolutions, r => r.width == res.width && r.height == res.height);
		}
		if (PlayerPrefs.HasKey("Fullscreen"))
		{
			IsFullscreen = Convert.ToBoolean(PlayerPrefs.GetInt("Fullscreen"));
			isFullscreenToggle.isOn = IsFullscreen;
			Screen.fullScreen = IsFullscreen;
		}
		else
		{
			isFullscreenToggle.isOn = Screen.fullScreen;
		}
		if (PlayerPrefs.HasKey("QualityPreset"))
		{
			int qualityIndex = PlayerPrefs.GetInt("QualityPreset");
			QualitySettings.SetQualityLevel(qualityIndex);
			qualityDropdown.value = qualityIndex;
			if(qualityIndex == 5)
			{
				if (PlayerPrefs.HasKey("TextureQuality"))
				{
					int textureIndex = PlayerPrefs.GetInt("TextureQuality");
					QualitySettings.masterTextureLimit = textureIndex;
					textureDropDown.value = textureIndex;
				}
				if (PlayerPrefs.HasKey("AntiAliasing"))
				{
					int antialiasing = PlayerPrefs.GetInt("AntiAliasing");
					QualitySettings.antiAliasing = antialiasing;
					aaDropdown.value = antialiasing;
				}
			}
		}
	}

	public virtual void SetFieldOfView(float fov)
	{
		FieldOfView = fov;
		fovValue.text = fov.ToString();
		PlayerPrefs.SetFloat("FieldOfView", fov);
	}
	public virtual void SetInvertMouse(bool inverted)
	{
		IsInverted = inverted;
		PlayerPrefs.SetInt("IsInverted", Convert.ToInt32(IsInverted));
	}

	public virtual void SetSensitivity(float sensitivity)
	{
		Sensitivity = sensitivity;
		sensitivityValue.text = sensitivity.ToString("P0");
		PlayerPrefs.SetFloat("Sensitivity", Sensitivity);
	}

	public virtual void SetMasterVolume(float volume)
	{
		masterVolume = volume;
		masterValue.text = volume.ToString("P0");
		PlayerPrefs.SetFloat("MasterVolume", masterVolume);
	}
	public virtual void SetMusicVolume(float volume)
	{
		musicVolume = volume;
		musicValue.text = volume.ToString("P0");
		PlayerPrefs.SetFloat("MusicVolume", musicVolume);
	}
	public virtual void SetSoundEffectsVolume(float volume)
	{
		soundEffectVolume = volume;
		soundEffectValue.text = volume.ToString("P0");
		PlayerPrefs.SetFloat("SoundEffectVolume", soundEffectVolume);
	}
	public virtual void SetVoiceVolume(float volume)
	{
		voiceVolume = volume;
		voiceValue.text = volume.ToString("P0");
		PlayerPrefs.SetFloat("VoiceVolume", voiceVolume);
	}
	public void SetResolution(int resolutionIndex)
	{
		Resolution resolution = resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, IsFullscreen);
		PlayerPrefs.SetInt("Resolution", resolutionIndex);
	}

	public void SetFullscreen(bool fullscreen)
	{
		Screen.fullScreen = fullscreen;
		PlayerPrefs.SetInt("Fullscreen", Convert.ToInt32(fullscreen));
	}

	public void SetQuality(int qualityIndex)
	{
		if (qualityIndex != 5)
		{
			QualitySettings.SetQualityLevel(qualityIndex);
			textureDropDown.value = QualitySettings.masterTextureLimit;
			aaDropdown.value = QualitySettings.antiAliasing;
		}
		qualityDropdown.value = qualityIndex;
		PlayerPrefs.SetInt("QualityPreset", qualityIndex);
	}

	public void SetTextureQuality(int textureIndex)
	{
		QualitySettings.masterTextureLimit = textureIndex;
		qualityDropdown.value = 5;
		PlayerPrefs.SetInt("TextureQuality", textureIndex);
	}

	public void SetAntiAliasing(int antiAliasing)
	{
		QualitySettings.antiAliasing = antiAliasing;
		qualityDropdown.value = 5;
		PlayerPrefs.SetInt("AntiAliasing", antiAliasing);
	}

	public void GameOptions()
	{
		gameOptions.SetActive(true);
		videoOptions.SetActive(false);
		audioOptions.SetActive(false);
		gameButton.interactable = false;
		videoButton.interactable = true;
		audioButton.interactable = true;
	}

	public void VideoOptions()
	{
		gameOptions.SetActive(false);
		videoOptions.SetActive(true);
		audioOptions.SetActive(false);
		gameButton.interactable = true;
		videoButton.interactable = false;
		audioButton.interactable = true;
	}
	public void AudioOptions()
	{
		gameOptions.SetActive(false);
		videoOptions.SetActive(false);
		audioOptions.SetActive(true);
		gameButton.interactable = true;
		videoButton.interactable = true;
		audioButton.interactable = false;
	}
}
