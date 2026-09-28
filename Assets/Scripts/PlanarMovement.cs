using UnityEngine;

namespace ValorantPractice
{
    /// <summary>Linear prototype movement, independent of input and scene objects.</summary>
    public static class PlanarMovement
    {
        // Returns displacement as well as the end velocity. Integrating the ramp
        // avoids frame-rate-dependent stopping distances and handles mid-frame stops.
        public static Vector3 Step(ref Vector3 velocity, Vector3 targetVelocity,
            float acceleration, float deceleration, float deltaTime)
        {
            if (deltaTime <= 0f)
                return Vector3.zero;

            acceleration = Mathf.Max(acceleration, 0.001f);
            deceleration = Mathf.Max(deceleration, 0.001f);

            if (Vector3.Dot(velocity, targetVelocity) < 0f)
            {
                float brakeTime = Mathf.Min(deltaTime, velocity.magnitude / deceleration);
                Vector3 displacement = Integrate(ref velocity, Vector3.zero, deceleration, brakeTime);
                return displacement + Integrate(ref velocity, targetVelocity,
                    acceleration, deltaTime - brakeTime);
            }

            float rate = targetVelocity == Vector3.zero || targetVelocity.sqrMagnitude + 0.0001f < velocity.sqrMagnitude
                ? deceleration : acceleration;
            return Integrate(ref velocity, targetVelocity, rate, deltaTime);
        }

        private static Vector3 Integrate(ref Vector3 velocity, Vector3 target, float rate, float deltaTime)
        {
            if (deltaTime <= 0f)
                return Vector3.zero;

            Vector3 difference = target - velocity;
            float distance = difference.magnitude;
            if (distance <= 0.000001f)
            {
                velocity = target;
                return target * deltaTime;
            }

            float timeToTarget = distance / rate;
            float rampTime = Mathf.Min(deltaTime, timeToTarget);
            Vector3 endVelocity = deltaTime >= timeToTarget
                ? target : velocity + difference * (rampTime / timeToTarget);
            Vector3 displacement = (velocity + endVelocity) * (0.5f * rampTime)
                + target * (deltaTime - rampTime);
            velocity = endVelocity;
            return displacement;
        }
    }
}
