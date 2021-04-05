using BepInEx;
using BepInEx.Configuration;
using BetterAPI;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Her_Burden
{
    class HerTorporItem : Her_Burden
    {
        private static String HERTORPOR_PICKUP;
        private static String HERTORPOR_DESC;
        private static String HERTORPOR_LORE;
        public static void Init()
        {
            ItemTier Hbtier = ItemTier.Lunar;
            if (Hbdbt.Value == false)
                Hbtier = ItemTier.Tier3;
            AddTokens();
            HerTorpor = ScriptableObject.CreateInstance<ItemDef>();
            HerTorpor.name = "HERTORPOR";
            HerTorpor.nameToken = "Her Torpor";
            HerTorpor.pickupToken = HERTORPOR_PICKUP;
            HerTorpor.descriptionToken = HERTORPOR_DESC;
            HerTorpor.loreToken = HERTORPOR_LORE;
            HerTorpor.tier = Hbtier;
            HerTorpor.pickupIconSprite = Her_Burden.bundle.LoadAsset<Sprite>(Hbiiv.Value + "royalblueItemIcon");
            HerTorpor.pickupModelPrefab = Her_Burden.bundle.LoadAsset<GameObject>(Hbiiv.Value + "royalblueher_burden");
            HerTorpor.canRemove = true;
            HerTorpor.hidden = false;

            var rules = new Items.CharacterItemDisplayRuleSet();
            AddLocation(rules);
            Items.Add(HerTorpor, rules);
        }
        private static void AddTokens()
        {
            if (Hbdbt.Value)
            {
                HERTORPOR_PICKUP = "Increase regen and decrease attack speed.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>";
                HERTORPOR_DESC = "Increase regen by 5% and decrease attack speed by 2.5%.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>";
            }
            if (!Hbdbt.Value)
            {
                HERTORPOR_PICKUP = "Increase regen.\nMonsters now have a chance to drop variants of: <color=#e7553b>Her Burden</color>";
                HERTORPOR_DESC = "Increase regen by 5%.\nMonsters now have a chance to drop variants of: <color=#e7553b>Her Burden</color>";
            }
            HERTORPOR_LORE = "None";

        }
        public static void AddLocation(Items.CharacterItemDisplayRuleSet rules)
        {
            if (!Hbisos.Value || Hbvos.Value != "Torpor")
            {
                GameObject followerPrefab = Her_Burden.bundle.LoadAsset<GameObject>(Hbiiv.Value + "royalblueher_burden");
                followerPrefab.AddComponent<PrefabSizeScript>();
                Vector3 generalScale = new Vector3(.0125f, .0125f, .0125f);
                _ = new ItemDisplayRule[]
                {
                new ItemDisplayRule
                {
                     ruleType = ItemDisplayRuleType.ParentedPrefab,
                     followerPrefab = followerPrefab,
                     childName = "Pelvis",
                     localPos = new Vector3(0f, 0.1f, 0.1f),
                     localAngles = new Vector3(180f, -0.05f, 0f),
                     localScale = generalScale
                }
                };
            }
            if (Hbisos.Value)
            {
                if (Hbvos.Value == "Torpor")
                {
                    GameObject followerPrefab = Her_Burden.bundle.LoadAsset<GameObject>(Hbiiv.Value + "royalblueher_burden");
                    followerPrefab.AddComponent<PrefabSizeScript>();
                    Vector3 generalScale = new Vector3(.0125f, .0125f, .0125f);
                    rules.AddCharacterModelRule(new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = followerPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0f, 0.1f, 0.1f),
                        localAngles = new Vector3(180f, -0.05f, 0f),
                        localScale = generalScale
                    }, "mdlCommandoDualies"
                    );
                    rules.AddCharacterModelRule(new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = followerPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0f, 0.1f, 0.1f),
                        localAngles = new Vector3(180f, -0.05f, 0f),
                        localScale = generalScale
                    }, "mdlHuntress"
                    );
                    rules.AddCharacterModelRule(new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = followerPrefab,
                        childName = "LowerArmR",
                        localPos = new Vector3(0f, 5.5f, 0f),
                        localAngles = new Vector3(45f, -90f, 0f),
                        localScale = generalScale * 10
                    }, "mdlToolbot"
                    );
                    rules.AddCharacterModelRule(new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = followerPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0f, 0.1f, 0.1f),
                        localAngles = new Vector3(180f, -0.05f, 0f),
                        localScale = generalScale
                    }, "mdlEngi"
                    );
                    rules.AddCharacterModelRule(new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = followerPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0f, 0.1f, 0.1f),
                        localAngles = new Vector3(180f, -0.05f, 0f),
                        localScale = generalScale
                    }, "mdlMage"
                    );
                    rules.AddCharacterModelRule(new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = followerPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0f, 0.25f, 0.05f),
                        localAngles = new Vector3(180f, -0.05f, 0f),
                        localScale = generalScale
                    }, "mdlMerc"
                    );
                    rules.AddCharacterModelRule(new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = followerPrefab,
                        childName = "WeaponPlatform",
                        localPos = new Vector3(0.2f, 0.05f, 0.2f),
                        localAngles = new Vector3(-45f, 0f, 0f),
                        localScale = generalScale * 2
                    }, "mdlTreebot"
                    );
                    rules.AddCharacterModelRule(new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = followerPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0f, 0.2f, 0.2f),
                        localAngles = new Vector3(180f, -0.05f, 0f),
                        localScale = generalScale
                    }, "mdlLoader"
                    );
                    rules.AddCharacterModelRule(new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = followerPrefab,
                        childName = "Hip",
                        localPos = new Vector3(0f, 3.5f, 0f),
                        localAngles = new Vector3(135f, -0.05f, 0f),
                        localScale = generalScale * 10
                    }, "mdlCroco"
                    );
                    rules.AddCharacterModelRule(new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = followerPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0f, 0.1f, 0.1f),
                        localAngles = new Vector3(180f, -0.05f, 0f),
                        localScale = generalScale
                    }, "mdlCaptain"
                    );
                    rules.AddCharacterModelRule(new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = followerPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(0f, 0.1f, 0.1f),
                        localAngles = new Vector3(180f, -0.05f, 0f),
                        localScale = generalScale
                    }, "mdlBandit2"
                    );
                    rules.AddCharacterModelRule(new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = followerPrefab,
                        childName = "Pelvis",
                        localPos = new Vector3(.3f, -.15f, 0f),
                        localAngles = new Vector3(20f, -120f, -36f),
                        localScale = generalScale
                    }, "mdlHeretic"
                    );
                }
            }
        }
    }
}
