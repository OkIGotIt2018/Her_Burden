using BepInEx;
using BepInEx.Configuration;
using EnigmaticThunder.Modules;
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
            AddLocation();
        }
        private static void AddTokens()
        {
            //AssetPlus is deprecated, so I switched it to use the current LanguageAPI
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
        public static void AddLocation()
        {
            /*if(Hbisos.Value == true)
            {
                GameObject followerPrefab = Resources.Load<GameObject>("@Her_Burden:Assets/Import/herburden/" + Hbiiv.Value + "violether_burden.prefab");
                followerPrefab.AddComponent<PrefabSizeScript>();
                if (Hbvos.Value == "Panic")
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
                    HerPanic.itemIndex = ItemAPI.Add(new CustomItem(HerPanic, rules));
                }
                else
                {
                    var rules = new ItemDisplayRuleDict(null);
                    HerPanic.itemIndex = ItemAPI.Add(new CustomItem(HerPanic, rules));
                }
            }
            else
            {
                var rules = new ItemDisplayRuleDict(null);
                HerPanic.itemIndex = ItemAPI.Add(new CustomItem(HerPanic, rules));
            }*/
            Pickups.RegisterItem(HerPanic);
        }
    }
}
