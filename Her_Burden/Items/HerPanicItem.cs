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
    class HerPanicItem : Her_Burden
    {
        public static void Init()
        {
            HerPanic = new ItemDef
            {
                name = "HERPANIC",
                nameToken = "HERPANIC_NAME",
                pickupToken = "HERPANIC_PICKUP",
                descriptionToken = "HERPANIC_DESC",
                loreToken = "HERPANIC_LORE",
                tier = ItemTier.Lunar,
                pickupIconPath = "@Her_Burden:Assets/Import/herburdenicon/HushvioletItemIcon.png",
                pickupModelPath = "@Her_Burden:Assets/Import/herburden/" + Hbiiv.Value + "violether_burden.prefab",
                canRemove = true,
                hidden = false
            };
            AddTokens();
            AddLocation();
        }
        private static void AddTokens()
        {
            //AssetPlus is deprecated, so I switched it to use the current LanguageAPI
            LanguageAPI.Add("HERPANIC_NAME", "Her Panic");
            LanguageAPI.Add("HERPANIC_PICKUP", "Increase move speed and decrease damage.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>");
            LanguageAPI.Add("HERPANIC_DESC", "Increase move speed by 5% and decrease damage by 2.5%.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>");
            LanguageAPI.Add("HERPANIC_LORE", "None");

        }
        public static void AddLocation()
        {
            GameObject followerPrefab = Resources.Load<GameObject>("@Her_Burden:Assets/Import/herburden/" + Hbiiv.Value + "violether_burden.prefab");
            followerPrefab.AddComponent<PrefabSizeScript>();
            var rules = new ItemDisplayRuleDict(null);
            HerPanic.itemIndex = ItemAPI.Add(new CustomItem(HerPanic, rules));
        }
    }
}
