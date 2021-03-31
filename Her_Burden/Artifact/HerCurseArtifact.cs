using EnigmaticThunder.Modules;
using RoR2;
using UnityEngine;

namespace Her_Burden
{
    class HerCurseArtifact : Her_Burden
    {
        public static void Init()
        {
            HerCurse = ScriptableObject.CreateInstance<ArtifactDef>();

            HerCurse.nameToken = "Artifact of Her Curse";
            HerCurse.descriptionToken = "All item drops will be turned into Her Burden Variants";
            HerCurse.smallIconSelectedSprite = Resources.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");
            HerCurse.smallIconDeselectedSprite = Resources.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");
            Artifacts.RegisterArtifact(HerCurse);
        }
    }
}
