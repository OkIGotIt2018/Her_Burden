using R2API;
using RoR2;
using UnityEngine;

namespace Her_Burden
{
    class HerCurseArtifact : Her_Burden
    {
        public static void Init()
        {
            HerCurse = ScriptableObject.CreateInstance<ArtifactDef>();

            LanguageAPI.Add("HERCURSE_NAME_TOKEN", "Artifact of Her Curse");
            LanguageAPI.Add("HERCURSE_DESC_TOKEN", "All item drops will be turned into Her Burden Variants");

            HerCurse.nameToken = "HERCURSE_NAME_TOKEN";
            HerCurse.descriptionToken = "HERCURSE_DESC_TOKEN";
            HerCurse.smallIconSelectedSprite = Resources.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");
            HerCurse.smallIconDeselectedSprite = Resources.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");

            ArtifactCatalog.getAdditionalEntries += ArtifactCatalog_getAdditionalEntries;
        }

        private static void ArtifactCatalog_getAdditionalEntries(System.Collections.Generic.List<ArtifactDef> obj)
        {
            obj.Add(HerCurse);
        }
    }
}
