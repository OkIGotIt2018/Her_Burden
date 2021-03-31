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
    class HerBurdenItem : Her_Burden
    {
        private static String HERBURDEN_PICKUP;
        private static String HERBURDEN_DESC;
        private static String HERBURDEN_LORE;
        public static void Init()
        {
            ItemTier Hbtier = ItemTier.Lunar;
            if (Hbdbt.Value == false)
                Hbtier = ItemTier.Tier3;
            AddTokens();
            HerBurden = ScriptableObject.CreateInstance<ItemDef>();
            HerBurden.name = "HERBURDEN";
            HerBurden.nameToken = "Her Burden";
            HerBurden.pickupToken = HERBURDEN_PICKUP;
            HerBurden.descriptionToken = HERBURDEN_DESC;
            HerBurden.loreToken = HERBURDEN_LORE;
            HerBurden.tier = Hbtier;
            HerBurden.pickupIconSprite = Her_Burden.bundle.LoadAsset<Sprite>(Hbiiv.Value + "ItemIcon");
            HerBurden.pickupModelPrefab = Her_Burden.bundle.LoadAsset<GameObject>(Hbiiv.Value + "her_burden");
            HerBurden.canRemove = true;
            HerBurden.hidden = false;
            AddLocation();
        }
        private static void AddTokens()
        {
            //AssetPlus is deprecated, so I switched it to use the current LanguageAPI
            if (Hbdbt.Value)
            {
                HERBURDEN_PICKUP = "Increase HP and decrease move speed.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>";
                HERBURDEN_DESC = "Increase HP by 5% and decrease move speed by 2.5%.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>";
            }
            if (!Hbdbt.Value)
            {
                HERBURDEN_PICKUP = "Increase HP.\nMonsters now have a chance to drop variants of: <color=#e7553b>Her Burden</color>";
                HERBURDEN_DESC = "Increase HP by 5%.\nMonsters now have a chance to drop variants of: <color=#e7553b>Her Burden</color>";
            }
            HERBURDEN_LORE = "None";

        }
        public static void AddLocation()
        {
            /*GameObject followerPrefab = Her_Burden.bundle.LoadAsset<GameObject>(Hbiiv.Value + "her_burden");
            followerPrefab.AddComponent<PrefabSizeScript>();
            Vector3 generalScale = new Vector3(.0125f, .0125f, .0125f);
            ItemDisplayRule what2 = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = followerPrefab,
                childName = "Pelvis",
                localPos = new Vector3(0f, 0.1f, 0.1f),
                localAngles = new Vector3(0f, 0.1f, 0.1f),
                localScale = generalScale
            };
            DisplayRuleGroup what = new DisplayRuleGroup();
            what.AddDisplayRule(what2);*/

            /*if (Hbisos.Value == true)
            {
                GameObject followerPrefab = Resources.Load<GameObject>("@Her_Burden:Assets/Import/herburden/" + Hbiiv.Value + "her_burden.prefab");
                followerPrefab.AddComponent<PrefabSizeScript>();
                if (Hbvos.Value == "Burden")
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
                    Pickups.RegisterItem(HerBurden);
                    HerBurden.itemIndex = ItemAPI.Add(new CustomItem(HerBurden, rules));
                }
                else
                {
                    var rules = new ItemDisplayRuleDict(null);
                    HerBurden.itemIndex = ItemAPI.Add(new CustomItem(HerBurden, rules));
                }

            }
            else
            {
                var rules = new ItemDisplayRuleDict(null);
                HerBurden.itemIndex = ItemAPI.Add(new CustomItem(HerBurden, rules));
            }*/
            Pickups.RegisterItem(HerBurden);
        }
    }
}
