using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "Match3/Theme")]
public class Theme : ScriptableObject
{
    public string ThemeName;

    [Header("Normal Item Sprites (7 types)")]
    public Sprite[] ItemSprites = new Sprite[7];

    public Sprite GetSpriteForType(NormalItem.eNormalType type)
    {
        int index = (int)type;
        if (index >= 0 && index < ItemSprites.Length)
        {
            return ItemSprites[index];
        }
        return null;
    }
}
