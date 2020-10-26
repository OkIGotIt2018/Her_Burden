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
            HerRecluse = new ItemDef
            {
                name = "HERRECLUSE",
                nameToken = "HERRECLUSE_NAME",
                pickupToken = "HERRECLUSE_PICKUP",
                descriptionToken = "HERRECLUSE_DESC",
                loreToken = "HERRECLUSE_LORE",
                tier = ItemTier.Lunar,
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
            GameObject followerPrefab = Resources.Load<GameObject>("@Her_Burden:Assets/Import/herburden/" + Hbiiv.Value + "lightishblueher_burden.prefab");
            followerPrefab.AddComponent<PrefabSizeScript>();
            var rules = new ItemDisplayRuleDict(null);
            HerRecluse.itemIndex = ItemAPI.Add(new CustomItem(HerRecluse, rules));
        }
    }
}
