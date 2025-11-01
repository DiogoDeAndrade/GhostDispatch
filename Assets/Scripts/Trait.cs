using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "Trait", menuName = "Ghost Dispatch/Trait")]
public class Trait : ScriptableObject
{
    public enum TargetDisplay { MainColor, Hat, Eyes, Mouth };

    [Serializable]
    public class VisualDisplay
    {
        public TargetDisplay    type = TargetDisplay.MainColor;
        [ShowIf(nameof(hasColor))]
        public Color            color = Color.white;
        [ShowIf(nameof(hasSprite))]
        public Sprite           sprite;

        public bool hasColor => (type == TargetDisplay.MainColor) || (type == TargetDisplay.Hat);
        public bool hasSprite => type == TargetDisplay.Hat;
    }

    [SerializeField] VisualDisplay[] visualDisplays;

    public void ApplyTraitVisuals(Ghost ghost)
    {
        foreach (var display in visualDisplays)
        {
            switch (display.type)
            {
                case TargetDisplay.MainColor:
                    {
                        var ghostMain = ghost.mainBody;
                        ghostMain.color = display.color;
                    }
                    break;
                case TargetDisplay.Hat:
                    {
                        var hatSprite = ghost.accessoryHat;
                        hatSprite.sprite = display.sprite;
                        hatSprite.color = display.color;
                        hatSprite.enabled = true;
                    }
                    break;
                case TargetDisplay.Eyes:
                    {
                        var eyes = ghost.eyes;
                        eyes.sprite = display.sprite;
                    }
                    break;
                case TargetDisplay.Mouth:
                    {
                        var mouth = ghost.mouth;
                        mouth.sprite = display.sprite;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
