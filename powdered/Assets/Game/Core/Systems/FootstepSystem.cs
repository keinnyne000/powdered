using UnityEngine;

namespace Game.Core.Systems
{
    /// <summary>
    /// Plays footstep sound everytime the player walks one stride
    /// </summary>
    public class FootstepSystem : ISimulationSystem
    {
    readonly GameState _state;
    readonly GameConfig _config;
    readonly AudioSource _audio;
    float _distance;

    public FootstepSystem(GameState state, GameConfig config)
    {
        _state = state;
        _config = config;

        _audio = new GameObject("FootstepAudio").AddComponent<AudioSource>();
        _audio.playOnAwake = false;

        _distance = _config.StepDistance;
    }
    public void Tick(float deltaTime)
        {
            AudioClip[] clips = _config.FootstepClips;
            if(clips == null || clips.Length == 0)
                return;

            Vector3 flat = new Vector3(_state.moveDirection.x, 0f, _state.moveDirection.z);

            if(!_state.isGrounded || flat.magnitude < 0.01f)
            {
                 _distance = _config.StepDistance;
                return;
            }
            _distance += flat.magnitude * deltaTime;
            if(_distance < _config.StepDistance)
                return;

            _distance = 0f;
            _audio.pitch = Random.Range(0.9f, 1.05f);
            _audio.PlayOneShot(clips[Random.Range(0, clips.Length)], _config.FootstepVolume);
        }
    }
}
