using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public static class Helpers
    {
        public const float kEpsilonNormalSqrt = 1e-15F;

        // Returns the angle in degrees between /from/ and /to/. This is always the smallest
        public static float Angle(this float3 from, float3 to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            float denominator = (float)math.sqrt(math.lengthsq(from) * math.lengthsq(to));
            if (denominator < kEpsilonNormalSqrt)
            {
                return 0F;
            }

            float dot = math.clamp(math.dot(from, to) / denominator, -1F, 1F);
            return math.acos(dot);
        }

        // The smaller of the two possible angles between the two vectors is returned, therefore the result will never be greater than 180 degrees or smaller than -180 degrees.
        // If you imagine the from and to vectors as lines on a piece of paper, both originating from the same point, then the /axis/ vector would point up out of the paper.
        // The measured angle between the two vectors would be positive in a clockwise direction and negative in an anti-clockwise direction.
        public static float SignedAngle(this float3 from, float3 to, float3 axis)
        {
            float unsignedAngle = Angle(from, to);

            float cross_x = from.y * to.z - from.z * to.y;
            float cross_y = from.z * to.x - from.x * to.z;
            float cross_z = from.x * to.y - from.y * to.x;
            float sign = math.sign(axis.x * cross_x + axis.y * cross_y + axis.z * cross_z);
            return unsignedAngle * sign;
        }
    }
}