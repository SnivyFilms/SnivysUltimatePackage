using UnityEngine;

namespace VVUP.Base
{
    public static class GetRandomNumber
    {
        public static int GetRandomInt()
        {
            return Random.Range(0, int.MaxValue);
        }
        public static int GetRandomInt(int max)
        {
            return Random.Range(0, max);
        }
        public static int GetRandomInt(int min, int max)
        {
            return Random.Range(min, max);
        }
        public static float GetRandomFloat()
        {
            return Random.Range(0, float.MaxValue);
        }
        public static float GetRandomFloat(float max)
        {
            return Random.Range(0, max);
        }
        public static float GetRandomFloat(float min, float max)
        {
            return Random.Range(min, max);
        }
    }
}