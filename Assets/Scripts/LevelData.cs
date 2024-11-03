namespace Secrets.Gameplay
{
    using UnityEngine;
    using System.Collections.Generic;
    [CreateAssetMenu(fileName = "New LevelData", menuName = "Secrets/Level Data")]
    public class LevelData : ScriptableObject
    {
        public Sprite        levelImage;
        public List<Hotspot> hotspots;
    }
}