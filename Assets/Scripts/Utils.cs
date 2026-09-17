using UnityEngine;

namespace Utils
{
    public static class Helper
    {
        public static Vector3 GetRandomDir()
        {
            return new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
            ).normalized;
        }
    }
}