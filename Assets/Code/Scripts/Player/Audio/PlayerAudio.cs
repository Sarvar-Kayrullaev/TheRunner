using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public new AudioSource audio;
    [Range(0,1)]public float Volume = 1;

    [Space]
    public AudioClip MUSIC_DYING_THEME;
    public AudioClip MUSIC_START_THEME;
    public AudioClip MUSIC_ACTION_THEME;
    public AudioClip MUSIC_SURVIVAL_THEME;
    public AudioClip MUSIC_CALM_THEME;
    public AudioClip MUSIC_ALIEN_THEME;
    public AudioClip MUSIC_COMPLETED_THEME;
    public AudioClip MUSIC_FAILED_THEME;
    public AudioClip MUSIC_TRAVELING_THEME;
    public AudioClip MUSIC_ESCAPE_THEME;
    public AudioClip CLIP_PICK_WEAPON;
    public AudioClip CLIP_PICK_AMMO;
    public AudioClip CLIP_THROW_WEAPON;
    public AudioClip CLIP_NO_SPACE_LEFT;
}
