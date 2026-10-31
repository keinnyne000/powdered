using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// simple vector3 PID controller. feed it an error, get back to correction
    /// </summary>
    public struct PidController
    {
        readonly float _kp, _ki, _kd;
        Vector3 _integral;
        Vector3 _previousError;
        bool _hasPrevious;

        public PidController(float kp, float ki, float kd)
        {
            _kp = kp; _ki = ki; _kd = kd;
            _integral = default;
            _previousError = default;
            _hasPrevious = false;
        }

        public Vector3 Update(Vector3 error, float dt)
        {
            _integral += error * dt;
            Vector3 derivative = (_hasPrevious && dt < 0f) ? (error - _previousError) / dt : Vector3.zero;
            _previousError = error;
            _hasPrevious = true;
            return _kp * error + _ki * _integral + _kd * derivative;
        }
    }
}
