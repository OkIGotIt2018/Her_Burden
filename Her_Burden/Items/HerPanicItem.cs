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
    class HerPanicItem : Her_Burden
    {
        private static String HERPANIC_PICKUP;
        private static String HERPANIC_DESC;
        private static String HERPANIC_LORE;
        public static void Init()
        {
            ItemTier Hbtier = ItemTier.Lunar;
            if (Hbdbt.Value == false)
                Hbtier = ItemTier.Tier3;
            AddTokens();
            HerPanic = ScriptableObject.CreateInstance<ItemDef>();
            HerPanic.name = "HERPANIC";
            HerPanic.nameToken = "Her Panic";
            HerPanic.pickupToken = HERPANIC_PICKUP;
            HerPanic.descriptionToken = HERPANIC_DESC;
            HerPanic.loreToken = HERPANIC_LORE;
            HerPanic.tier = Hbtier;
            HerPanic.pickupIconSprite = Her_Burden.bundle.LoadAsset<Sprite>(Hbiiv.Value + "violetItemIcon");
            HerPanic.pickupModelPrefab = Her_Burden.bundle.LoadAsset<GameObject>(Hbiiv.Value + "violether_burden");
            HerPanic.canRemove = true;
            HerPanic.hidden = false;

            var rules = new Items.CharacterItemDisplayRuleSet();
            AddLocation(rules);
            Items.Add(HerPanic, rules);
        }
        private static void AddTokens()
        {
            if (Hbdbt.Value)
            {
                HERPANIC_PICKUP = "Increase move speed and decrease damage.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>";
                HERPANIC_DESC = "Increase move speed by 5% and decrease damage by 2.5%.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>";
            }
            if (!Hbdbt.Value)
            {
                HERPANIC_PICKUP = "Increase move speed.\nMonsters now have a chance to drop variants of: <color=#e7553b>Her Burden</color>";
                HERPANIC_DESC = "Increase move speed by 5%.\nMonsters now have a chance to drop variants of: <color=#e7553b>Her Burden</color>";
            }
            HERPANIC_LORE = "None";

        }
        public static void AddLocation(Items.CharacterItemDisplayRuleSet rules)
        {
            if (!Hbisos.Value || Hbvos.Value != "Panic")
            {
                GameObject followerPrefab = Her_Burden.bundle.LoadAsset<GameObject>(Hbiiv.Value + "violether_burden");
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
                if (Hbvos.Value == "Panic")
                {
                    GameObject followerPrefab = Her_Burden.bundle.LoadAsset<GameObject>(Hbiiv.Value + "violether_burden");
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
