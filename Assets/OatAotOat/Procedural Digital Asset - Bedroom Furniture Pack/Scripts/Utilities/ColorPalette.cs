using UnityEngine;

namespace OatAotOat.ProceduralDigitalAsset
{
    [CreateAssetMenu(fileName = "New Color Palette", menuName = "Procedural Tools/Color Palette")]
    public class ColorPalette : ScriptableObject
    {
        [Tooltip("Add as many colors as you want to this palette!")]
        public Color[] colors = new Color[] {
            new(0.6f, 0.4f, 0.2f),
            new(0.3f, 0.2f, 0.1f),
            Color.white,
            Color.black
        };
    }
}
