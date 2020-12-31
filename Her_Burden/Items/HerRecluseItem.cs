using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Her_Burden
{
    class HerRecluseItem : Her_Burden
    {
        public static void Init()
        {
            ItemTier Hbtier = ItemTier.Lunar;
            if (Hbdbt.Value == false)
                Hbtier = ItemTier.Tier3;
            HerRecluse = new ItemDef
            {
                name = "HERRECLUSE",
                nameToken = "HERRECLUSE_NAME",
                pickupToken = "HERRECLUSE_PICKUP",
                descriptionToken = "HERRECLUSE_DESC",
                loreToken = "HERRECLUSE_LORE",
                tier = Hbtier,
                pickupIconPath = "@Her_Burden:Assets/Import/herburdenicon/HushlightishblueItemIcon.png",
                pickupModelPath = "@Her_Burden:Assets/Import/herburden/" + Hbiiv.Value + "lightishblueher_burden.prefab",
                canRemove = true,
                hidden = false
            };
            AddTokens();
            AddLocation();
        }
        private static void AddTokens()
        {
            //AssetPlus is deprecated, so I switched it to use the current LanguageAPI
            LanguageAPI.Add("HERRECLUSE_NAME", "Her Recluse");
            LanguageAPI.Add("HERRECLUSE_PICKUP", "Increase armor and decrease regen.\nAll item drops are now: <color=#307FFF>Her Burden</color>");
            LanguageAPI.Add("HERRECLUSE_DESC", "Increase armor by 5% and decrease regen by 2.5%.\nAll item drops are now: <color=#307FFF>Her Burden</color>");
            LanguageAPI.Add("HERRECLUSE_LORE", "None");

        }
        public static void AddLocation()
        {
            if (Hbisos.Value == true)
            {
                GameObject followerPrefab = Resources.Load<GameObject>("@Her_Burden:Assets/Import/herburden/" + Hbiiv.Value + "lightishblueher_burden.prefab");
                followerPrefab.AddComponent<PrefabSizeScript>();
                if (Hbvos.Value == "Recluse")
                {
                    Vector3 generalScale = new Vector3(.0125f, .0125f, .0125f);
                    ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
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
                    });
                    rules.Add("mdlHuntress", new ItemDisplayRule[]
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
                    });
                    rules.Add("mdlToolbot", new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = followerPrefab,
                            childName = "LowerArmR",
                            localPos = new Vector3(0f, 5.5f, 0f),
                            localAngles = new Vector3(45f, -90f, 0f),
                            localScale = generalScale * 10
                        }
                    });
                    rules.Add("mdlEngi", new ItemDisplayRule[]
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
                    });
                    rules.Add("mdlMage", new ItemDisplayRule[]
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
                    });
                    rules.Add("mdlMerc", new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = followerPrefab,
                            childName = "Pelvis",
                            localPos = new Vector3(0f, 0.25f, 0.05f),
                            localAngles = new Vector3(180f, -0.05f, 0f),
                            localScale = generalScale
                        }
                    });
                    rules.Add("mdlTreebot", new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = followerPrefab,
                            childName = "WeaponPlatform",
                            localPos = new Vector3(0.2f, 0.05f, 0.2f),
                            localAngles = new Vector3(-45f, 0f, 0f),
                            localScale = generalScale * 2
                        }
                    });
                    rules.Add("mdlLoader", new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = followerPrefab,
                            childName = "Pelvis",
                            localPos = new Vector3(0f, 0.2f, 0.2f),
                            localAngles = new Vector3(180f, -0.05f, 0f),
                            localScale = generalScale
                        }
                    });
                    rules.Add("mdlCroco", new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = followerPrefab,
                            childName = "Hip",
                            localPos = new Vector3(0f, 3.5f, 0f),
                            localAngles = new Vector3(135f, -0.05f, 0f),
                            localScale = generalScale * 10
                    }
                    });
                    rules.Add("mdlCaptain", new ItemDisplayRule[]
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
                    });
                    HerRecluse.itemIndex = ItemAPI.Add(new CustomItem(HerRecluse, rules));
                }
                else
                {
                    var rules = new ItemDisplayRuleDict(null);
                    HerRecluse.itemIndex = ItemAPI.Add(new CustomItem(HerRecluse, rules));
                }
            }
            else
            {
                var rules = new ItemDisplayRuleDict(null);
                HerRecluse.itemIndex = ItemAPI.Add(new CustomItem(HerRecluse, rules));
            }
        }
    }
}
