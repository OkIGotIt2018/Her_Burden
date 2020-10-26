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
    class HerRancorItem : Her_Burden
    {
        public static void Init()
        {
            HerRancor = new ItemDef
            {
                name = "HERRANCOR",
                nameToken = "HERRANCOR_NAME",
                pickupToken = "HERRANCOR_PICKUP",
                descriptionToken = "HERRANCOR_DESC",
                loreToken = "HERRANCOR_LORE",
                tier = ItemTier.Lunar,
                pickupIconPath = "@Her_Burden:Assets/Import/herburdenicon/HushorangeItemIcon.png",
                pickupModelPath = "@Her_Burden:Assets/Import/herburden/" + Hbiiv.Value + "orangeher_burden.prefab",
                canRemove = true,
                hidden = false
            };
            AddTokens();
            AddLocation();
        }
        private static void AddTokens()
        {
            //AssetPlus is deprecated, so I switched it to use the current LanguageAPI
            LanguageAPI.Add("HERRANCOR_NAME", "Her Rancor");
            LanguageAPI.Add("HERRANCOR_PICKUP", "Increase damage and decrease armor.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>");
            LanguageAPI.Add("HERRANCOR_DESC", "Increase damage by 5% and decrease armor by 2.5%.\nAll item drops are now variants of: <color=#307FFF>Her Burden</color>");
            LanguageAPI.Add("HERRANCOR_LORE", "None");

        }
        public static void AddLocation()
        {
            GameObject followerPrefab = Resources.Load<GameObject>("@Her_Burden:Assets/Import/herburden/" + Hbiiv.Value + "orangeher_burden.prefab");
            followerPrefab.AddComponent<PrefabSizeScript>();
            var rules = new ItemDisplayRuleDict(null);
            HerRancor.itemIndex = ItemAPI.Add(new CustomItem(HerRancor, rules));
        }
    }
}
