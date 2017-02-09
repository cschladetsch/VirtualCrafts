using System;
using UnityEngine;

namespace App.Math
{
    /// <summary>
    /// A standard PID controller implementation.
    /// </summary>
    /// <remarks>
    /// See https://en.wikipedia.org/wiki/PID_controller.
    /// </remarks>
    public class PidController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PidController" /> class.
        /// </summary>
        /// <param name="kp">The proportional gain.</param>
        /// <param name="ki">The integral gain.</param>
        /// <param name="kd">The derivative gain.</param>
        /// <exception cref="ArgumentOutOfRangeException">If one of the parameters is negative.</exception>
        public PidController(float kp, float ki, float kd)
        {
            if (kp < 0.0f)
                throw new ArgumentOutOfRangeException("kp", "kp must be a non-negative number.");

            if (ki < 0.0f)
                throw new ArgumentOutOfRangeException("ki", "ki must be a non-negative number.");

            if (kd < 0.0f)
                throw new ArgumentOutOfRangeException("kd", "kd must be a non-negative number.");

            Kp = kp;
            Ki = ki;
            Kd = kd;

            _integralMax = MaxOutput / Ki;
        }

        /// <summary>
        /// Gets or sets the proportional gain.
        /// </summary>
        /// <value>
        /// The proportional gain.
        /// </value>
        public float Kp
        {
            get { return _kp; } 
            set
            {
                if (value < 0.0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Kp must be a non-negative number.");
                }

                _kp = value;
            }
        }

        /// <summary>
        /// Gets or sets the integral gain.
        /// </summary>
        /// <value>
        /// The integral gain.
        /// </value>
        public float Ki
        {
            get { return _ki; }
            set
            {
                if (value < 0.0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Ki must be a non-negative number.");
                }

                _ki = value;

                _integralMax = MaxOutput / Ki;
                _integral = Mathf.Clamp(_integral, -_integralMax, _integralMax);
            }
        }

        /// <summary>
        /// Gets or sets the derivative gain.
        /// </summary>
        /// <value>
        /// The derivative gain.
        /// </value>
        public float Kd
        {
            get { return _kd; }
            set
            {
                if (value < 0.0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Kd must be a non-negative number.");
                }

                _kd = value;
            }
        }

        /// <summary>
        /// Computes the corrective output.
        /// </summary>
        /// <param name="error">The current error of the signal.</param>
        /// <param name="delta">The delta of the signal since last frame.</param>
        /// <param name="deltaTime">The delta time.</param>
        /// <returnsThe corrective output.</returns>
        public float ComputeOutput(float error, float delta, float deltaTime)
        {
            _integral += (error * deltaTime);
            _integral = Mathf.Clamp(_integral, -_integralMax, _integralMax);

            float derivative = delta / deltaTime;
            float output = (Kp * error) + (Ki * _integral) + (Kd * derivative);

            output = Mathf.Clamp(output, -MaxOutput, MaxOutput);

            return output;
        }

        private const float MaxOutput = 1000.0f;
        private float _integralMax;
        private float _integral;
        private float _kp;
        private float _ki;
        private float _kd;
    }
}
