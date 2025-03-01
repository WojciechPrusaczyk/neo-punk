using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OptionsManager
{
    /*
     * Sounds
     */
    /*private static float MainSoundVolume = 1.00f;
    private static float DialogsVolume = 1.00f;
    private static float MusicVolume = 1.00f;
    private static float EffectsVolume = 1.00f;*/

    public static void SetMainSoundVolume(float volume) { 
        if (volume >= 0.00 && volume <= 1.00)
        {
            PlayerPrefs.SetFloat("MainSoundVolume", volume);
        }
    }

    public static void SetDialogsVolume(float volume)
    {
        if (volume >= 0.00 && volume <= 1.00)
        {
            PlayerPrefs.SetFloat("DialogsVolume", volume);
        }
    }
    public static void SetMusicVolume(float volume) { 
        if (volume >= 0.00 && volume <= 1.00)
        {
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
    }

    public static void SetEffectsVolume(float volume)
    {
        if (volume >= 0.00 && volume <= 1.00)
        {
            PlayerPrefs.SetFloat("EffectsVolume", volume);
        } 
        
    }
    
    public static float GetMainSoundVolume() { return PlayerPrefs.GetFloat("MainSoundVolume", 1.0f); }
    public static float GetDialogsVolume() { return PlayerPrefs.GetFloat("DialogsVolume", 1.0f); }
    public static float GetMusicVolume() { return PlayerPrefs.GetFloat("MusicVolume", 1.0f); }
    public static float GetEffectsVolume() { return PlayerPrefs.GetFloat("EffectsVolume", 1.0f); }
    
    /*
     * Gameplay
     */
    /*private static bool ShowTips = true;*/
    
    public static void SetShowTips(bool value) { PlayerPrefs.SetInt("ShowTips", value?1:0); }

    public static bool GetShowTips()
    {
        int doShowTips = PlayerPrefs.GetInt("ShowTips", 1);
        
        if (doShowTips == 1) return true;
        return false;
    }
    
    /*
     * Resoulition
     */
    public static void SetResoulution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResoulutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
        PlayerPrefs.Save();
    }

    public static Vector2 GetResoulution()
    {
        int width = PlayerPrefs.GetInt("ResoulutionWidth", Screen.currentResolution.width);
        int height = PlayerPrefs.GetInt("ResoulutionHeight", Screen.currentResolution.height);
        return new Vector2(width, height);
    }

    public static void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        PlayerPrefs.SetInt("Fullscreen", isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    /*
     * Default
     */
    public static void ResetToDefault()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}