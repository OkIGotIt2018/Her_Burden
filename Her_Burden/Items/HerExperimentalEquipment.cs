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
    class HerGambleEquipment : Her_Burden
    {
        public static void Init()
        {
            var Hgcolor = ColorCatalog.ColorIndex.LunarItem;
            if (!Hbdbt.Value)
                Hgcolor = ColorCatalog.ColorIndex.Equipment;
            HerGamble = new EquipmentDef
            {
                name = "HERGAMBLE",
                nameToken = "HERGAMBLE_NAME",
                pickupToken = "HERGAMBLE_PICKUP",
                descriptionToken = "HERGAMBLE_DESC",
                loreToken = "HERGAMBLE_LORE",
                isLunar = Hbdbt.Value,
                colorIndex = Hgcolor,
                pickupIconPath = "@Her_Burden:Assets/Import/herburdenicon/" + Hbiiv.Value + "ItemIcon.png",
                pickupModelPath = "@Her_Burden:Assets/Import/herburden/her_gamble.prefab",
                canDrop = true,
                cooldown = 60f
            };
            ExperimentalBuff = new BuffDef
            {
                name = "ExperimentalBuff",
                iconPath = "@Her_Burden:Assets/Import/herburdenicon/" + Hbiiv.Value + "ItemIcon.png"
            };
            ExperimentalBuff.buffIndex = BuffAPI.Add(new CustomBuff(ExperimentalBuff));
            ExperimentalDeBuff = new BuffDef
            {
                name = "ExperimentalDeBuff",
                iconPath = "@Her_Burden:Assets/Import/herburdenicon/" + Hbiiv.Value + "ItemIcon.png"
            };
            ExperimentalDeBuff.buffIndex = BuffAPI.Add(new CustomBuff(ExperimentalDeBuff));
            AddTokens();
            AddLocation();
        }
        private static void AddTokens()
        {
            //AssetPlus is deprecated, so I switched it to use the current LanguageAPI
            LanguageAPI.Add("HERGAMBLE_NAME", "Her Gamble");
            if (Hbdbt.Value)
            {
                LanguageAPI.Add("HERGAMBLE_PICKUP", "An equipment that gambles your stats");
                LanguageAPI.Add("HERGAMBLE_DESC", "An equipment that gambles your stats that come from Her Burden Variants");
            }
            if (!Hbdbt.Value)
            {
                LanguageAPI.Add("HERGAMBLE_PICKUP", "Has a chance to double your stats");
                LanguageAPI.Add("HERGAMBLE_DESC", "Has a chance to double your stats that come from Her Burden Variants");
            }
            LanguageAPI.Add("HERGAMBLE_LORE", "None");

        }
        public static void AddLocation()
        {
            var rules = new ItemDisplayRuleDict(null);
            HerGamble.equipmentIndex = ItemAPI.Add(new CustomEquipment(HerGamble, rules));
        }
    }
}
