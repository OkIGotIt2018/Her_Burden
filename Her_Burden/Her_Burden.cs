using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;
using Object = System.Object;
using UnityObject = UnityEngine.Object;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using RoR2.ContentManagement;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Her_Burden
{
    [BepInDependency("com.xoxfaby.BetterAPI")]
    [BepInPlugin("com.OkIgotIt.Her_Burden", "Her_Burden", "1.4.6")]

    public class Her_Burden : BaseUnityPlugin
    {
        public static AssetBundle bundle;
        public static ItemDef HerBurden;
        public static ItemDef HerRecluse;
        public static ItemDef HerFury;
        public static ItemDef HerTorpor;
        public static ItemDef HerRancor;
        public static ItemDef HerPanic;
        public static EquipmentDef HerGamble;
        public static BuffDef ExperimentalBuff;
        public static BuffDef ExperimentalDeBuff;
        public static ArtifactDef HerCurse;
        public static ItemDef VariantOnSurvivor;
        public static ConfigEntry<bool> Hbisos { get; set; }
        public static ConfigEntry<bool> Hbpul { get; set; }
        public static ConfigEntry<bool> Hbgoi { get; set; }
        public static ConfigEntry<bool> Hbdbt { get; set; }
        public static ConfigEntry<bool> Hbvst { get; set; }
        public static ConfigEntry<bool> Hbvsm { get; set; }
        public static ConfigEntry<bool> Hbnsfw { get; set; }
        public static ConfigEntry<string> Hbiiv { get; set; }
        public static ConfigEntry<string> Hbvos { get; set; }
        public static ConfigEntry<float> Hbims { get; set; }
        public static ConfigEntry<float> Hbimssm { get; set; }
        public static ConfigEntry<int> Hbcpu { get; set; }
        public static ConfigEntry<int> Hbedc { get; set; }
        public static ConfigEntry<float> Hbbuff { get; set; }
        public static ConfigEntry<float> Hbdebuff { get; set; }
        public void Awake()
        {
            Hbisos = Config.Bind<bool>("1. Her Burden Toggle", "Toggle Item Visibility", true, "Changes if Her Burden Variants shows on the Survivor");
            Hbpul = Config.Bind<bool>("1. Her Burden Toggle", "Toggle Luck Effect", false, "Changes if luck effects chance to pickup Her Burden Variants once you have one");
            Hbgoi = Config.Bind<bool>("1. Her Burden Toggle", "Toggle Give Original Item", false, "Changes if you also get the original item along with a Her Burden Variant");
            Hbdbt = Config.Bind<bool>("1. Her Burden Toggle", "Toggle Debuffs", true, "Changes if debuffs are applied or not, if disabled, Her Burden Variants changes to legendary and makes it have a chance to drop on kill");
            Hbvst = Config.Bind<bool>("1. Her Burden Toggle", "Toggle Variant Drop Count", true, "Changes if all Her Burden Variants are in the drop list or just Her Burden, if disabled, only Her Burden is in the drop list");
            Hbvsm = Config.Bind<bool>("2. Her Burden Toggle", "Toggle Variants Affect Size", false, "Changes if other Her Burden Variants increase item display size");
            Hbnsfw = Config.Bind<bool>("0. Her Burden NSFW Toggle", "Toggles Her Burden NSFW", true, "Changes if Her Burden is NSFW or SFW, if disabled, Her Burden will have SFW models when released");
            Hbiiv = Config.Bind<string>("2. Her Burden General", "Artist", "Hush", "Decides what artist to use, \"Hush\" or \"aka6\".");
            Hbvos = Config.Bind<string>("2. Her Burden General", "Variant Size Increase", "Burden", "Changes what Variant gets its size increased, \"Burden\" \"Recluse\" \"Fury\" \"Torpor\" \"Rancor\" \"Panic\".");
            Hbims = Config.Bind<float>("1. Her Burden Size", "Max size of the item", 2, "Changes the max size of the item on the Survivor");
            Hbimssm = Config.Bind<float>("1. Her Burden Size", "Size Multiplier for the item", 0.049375f, "Changes the rate that the item size increases by");
            Hbcpu = Config.Bind<int>("1. Her Burden Mechanics", "Chance to change pickup to Her Burden Variants", 100, "Chance to change other items to Her Burden Variants on pickup once you have one");
            Hbedc = Config.Bind<int>("1. Her Burden Mechanics", "Chance for enemies to drop Her Burden Variants", 5, "Chance for enemies top drop Her Burden Variants once you have one");
            Hbbuff = Config.Bind<float>("1. Her Burden Mechanics", "Buff", 1.05f, "Changes the increase of buff of the item per item exponentially");
            Hbdebuff = Config.Bind<float>("1. Her Burden Mechanics", "Debuff", 0.975f, "Changes the decrease of debuff of the item per item exponentially");
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Her_Burden.Resources.herburden"))
            {
                bundle = AssetBundle.LoadFromStream(stream);
            }

            if (Hbvos.Value != "Burden" && Hbvos.Value != "Recluse" && Hbvos.Value != "Fury" && Hbvos.Value != "Torpor" && Hbvos.Value != "Rancor" && Hbvos.Value != "Panic")
                Hbvos.Value = "Burden";
            if (Hbiiv.Value != "Hush" && Hbiiv.Value != "aka6")
                Hbiiv.Value = "Hush";

            HerBurdenItem.Init();
            HerRecluseItem.Init();
            HerFuryItem.Init();
            HerTorporItem.Init();
            HerRancorItem.Init();
            HerPanicItem.Init();
            HerGambleEquipment.Init();
            HerCurseArtifact.Init();

            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;

            //maxHealth-moveSpeed   Her Burden
            //armor-regen           Her Recluse
            //attackSpeed-maxHealth Her Fury
            //regen-attackSpeed     Her Torpor
            //damage-armor          Her Rancor
            //moveSpeed-damage      Her Panic

            On.RoR2.GenericPickupController.GrantItem += (orig, self, body, inventory) =>
            {
                if (RunArtifactManager.instance.IsArtifactEnabled(HerCurse))
                {
                    orig(self, body, inventory);


                    //Handle the size change with scripts
                    Size(1, body, false);
                    Size(2, body, false);
                    return;
                }
                if (/*Hbvst.Value && */Hbdbt.Value)
                {
                    bool changepickup = false;
                    if (body.inventory.GetItemCount(HerBurden.itemIndex) > 0 || body.inventory.GetItemCount(HerRecluse.itemIndex) > 0 || body.inventory.GetItemCount(HerFury.itemIndex) > 0 || body.inventory.GetItemCount(HerTorpor.itemIndex) > 0 || body.inventory.GetItemCount(HerRancor.itemIndex) > 0 || body.inventory.GetItemCount(HerPanic.itemIndex) > 0)
                        changepickup = true;
                    bool blacklist = false;
                    if (Hbdbt.Value == false)
                        blacklist = true;
                    if (self.pickupIndex == PickupCatalog.FindPickupIndex(HerBurden.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(HerRecluse.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(HerFury.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(HerTorpor.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(HerRancor.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(HerPanic.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapWhite.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapGreen.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapRed.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapYellow.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.ArtifactKey.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarTrinket.itemIndex))
                        blacklist = true;
                    if (blacklist == false && Hbgoi.Value == true)
                        if (self.pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex))
                            blacklist = true;

                    bool CheckRollTrue;
                    if (Hbpul.Value == true)
                        CheckRollTrue = Util.CheckRoll(Hbcpu.Value, body.master);
                    else
                        CheckRollTrue = Util.CheckRoll(Hbcpu.Value);

                    if (changepickup == true && CheckRollTrue == true && blacklist == false && Hbgoi.Value == true)
                        orig(self, body, inventory);
                    if (changepickup == true && CheckRollTrue == true && blacklist == false)
                    {
                        switch (Mathf.FloorToInt(UnityRandom.Range(0, 6)))
                        {
                            case 0:
                                self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden.itemIndex);
                                break;
                            case 1:
                                self.pickupIndex = PickupCatalog.FindPickupIndex(HerRecluse.itemIndex);
                                break;
                            case 2:
                                self.pickupIndex = PickupCatalog.FindPickupIndex(HerFury.itemIndex);
                                break;
                            case 3:
                                self.pickupIndex = PickupCatalog.FindPickupIndex(HerTorpor.itemIndex);
                                break;
                            case 4:
                                self.pickupIndex = PickupCatalog.FindPickupIndex(HerRancor.itemIndex);
                                break;
                            case 5:
                                self.pickupIndex = PickupCatalog.FindPickupIndex(HerPanic.itemIndex);
                                break;
                        }
                    }
                }
                /*if (!Hbvst.Value && Hbdbt.Value)
                {
                    bool changepickup = false;
                    if (body.inventory.GetItemCount(HerBurden.itemIndex) > 0 || body.inventory.GetItemCount(HerRecluse.itemIndex) > 0 || body.inventory.GetItemCount(HerFury.itemIndex) > 0 || body.inventory.GetItemCount(HerTorpor.itemIndex) > 0 || body.inventory.GetItemCount(HerRancor.itemIndex) > 0 || body.inventory.GetItemCount(HerPanic.itemIndex) > 0)
                        changepickup = true;
                    bool blacklist = false;
                    if (Hbdbt.Value == false)
                        blacklist = true;
                    if (self.pickupIndex == PickupCatalog.FindPickupIndex(HerRecluse.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(HerFury.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(HerTorpor.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(HerRancor.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex(HerPanic.itemIndex) || self.pickupIndex == PickupCatalog.FindPickupIndex("ScrapWhite") || self.pickupIndex == PickupCatalog.FindPickupIndex("ScrapGreen") || self.pickupIndex == PickupCatalog.FindPickupIndex("ScrapRed") || self.pickupIndex == PickupCatalog.FindPickupIndex("ScrapYellow") || self.pickupIndex == PickupCatalog.FindPickupIndex("ArtifactKey") || self.pickupIndex == PickupCatalog.FindPickupIndex("LunarTrinket"))
                        blacklist = true;
                    if (blacklist == false && Hbgoi.Value == true)
                        if (self.pickupIndex == PickupCatalog.FindPickupIndex("Pearl") || self.pickupIndex == PickupCatalog.FindPickupIndex("ShinyPearl"))
                            blacklist = true;

                    bool CheckRollTrue;
                    if (Hbpul.Value == true)
                        CheckRollTrue = Util.CheckRoll(Hbcpu.Value, body.master);
                    else
                        CheckRollTrue = Util.CheckRoll(Hbcpu.Value);

                    if (changepickup == true && CheckRollTrue == true && blacklist == false && Hbgoi.Value == true)
                        orig(self, body, inventory);
                    if (changepickup == true && CheckRollTrue == true && blacklist == false)
                    {
                        self.pickupIndex = PickupCatalog.FindPickupIndex(HerBurden.itemIndex);
                    }
                }*/
                orig(self, body, inventory);


                //Handle the size change with scripts
                Size(1, body, false);
                Size(2, body, false);
            };
            WhoKnows();
            /*if (!Hbvst.Value)
            {
                On.RoR2.Run.BuildDropTable += (orig, self) =>
                {
                    if (Hbdbt.Value)
                    {
                        ItemDropAPI.RemoveFromDefaultByTier(ItemTier.Lunar, HerFury.itemIndex);
                        ItemDropAPI.RemoveFromDefaultByTier(ItemTier.Lunar, HerPanic.itemIndex);
                        ItemDropAPI.RemoveFromDefaultByTier(ItemTier.Lunar, HerRancor.itemIndex);
                        ItemDropAPI.RemoveFromDefaultByTier(ItemTier.Lunar, HerRecluse.itemIndex);
                        ItemDropAPI.RemoveFromDefaultByTier(ItemTier.Lunar, HerTorpor.itemIndex);
                    }
                    if (!Hbdbt.Value)
                    {
                        ItemDropAPI.RemoveFromDefaultByTier(ItemTier.Tier3, HerFury.itemIndex);
                        ItemDropAPI.RemoveFromDefaultByTier(ItemTier.Tier3, HerPanic.itemIndex);
                        ItemDropAPI.RemoveFromDefaultByTier(ItemTier.Tier3, HerRancor.itemIndex);
                        ItemDropAPI.RemoveFromDefaultByTier(ItemTier.Tier3, HerRecluse.itemIndex);
                        ItemDropAPI.RemoveFromDefaultByTier(ItemTier.Tier3, HerTorpor.itemIndex);
                    }
                    orig(self);
                };
            }*/
            On.RoR2.CharacterBody.Update += CharacterBody_Update;
            On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
            On.EntityStates.Duplicator.Duplicating.OnEnter += Duplicating_OnEnter;
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            On.RoR2.PickupDropletController.CreatePickupDroplet += PickupDropletController_CreatePickupDroplet;
            On.RoR2.CharacterMaster.RespawnExtraLife += CharacterMaster_RespawnExtraLife;
        }

        public void Start()
        {
            if (Hbvos.Value == "Burden")
                VariantOnSurvivor = HerBurden;
            else if (Hbvos.Value == "Recluse")
                VariantOnSurvivor = HerRecluse;
            else if (Hbvos.Value == "Fury")
                VariantOnSurvivor = HerFury;
            else if (Hbvos.Value == "Torpor")
                VariantOnSurvivor = HerTorpor;
            else if (Hbvos.Value == "Rancor")
                VariantOnSurvivor = HerRancor;
            else if (Hbvos.Value == "Panic")
                VariantOnSurvivor = HerPanic;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new Content());
        }

        private void CharacterMaster_RespawnExtraLife(On.RoR2.CharacterMaster.orig_RespawnExtraLife orig, CharacterMaster self)
        {
            CharacterBody body = self.GetBody();
            Size(3, body, true);
            Size(4, body, true);
            orig(self);

            if (self.playerCharacterMasterController)
            {
                Size(5, body, false);
                Size(6, body, false);
            }
        }

        private void PickupDropletController_CreatePickupDroplet(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            bool burdenvariant = false;
            if (pickupIndex == PickupCatalog.FindPickupIndex(HerBurden.itemIndex) || pickupIndex == PickupCatalog.FindPickupIndex(HerRecluse.itemIndex) || pickupIndex == PickupCatalog.FindPickupIndex(HerFury.itemIndex) || pickupIndex == PickupCatalog.FindPickupIndex(HerTorpor.itemIndex) || pickupIndex == PickupCatalog.FindPickupIndex(HerRancor.itemIndex) || pickupIndex == PickupCatalog.FindPickupIndex(HerPanic.itemIndex))
                burdenvariant = true;
            bool blacklist = false;
            if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.ArtifactKey.itemIndex) || pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarTrinket.itemIndex))
                blacklist = true;
            if (RunArtifactManager.instance.IsArtifactEnabled(HerCurse) && PickupCatalog.GetPickupDef(pickupIndex).itemIndex != ItemIndex.None && !burdenvariant/* && Hbvst.Value*/ && !blacklist)
            {
                switch (Mathf.FloorToInt(UnityRandom.Range(0, 6)))
                {
                    case 0:
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerBurden.itemIndex), position, velocity);
                        break;
                    case 1:
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerRecluse.itemIndex), position, velocity);
                        break;
                    case 2:
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerFury.itemIndex), position, velocity);
                        break;
                    case 3:
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerTorpor.itemIndex), position, velocity);
                        break;
                    case 4:
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerRancor.itemIndex), position, velocity);
                        break;
                    case 5:
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerPanic.itemIndex), position, velocity);
                        break;
                }
                return;
            }
            /*if (RunArtifactManager.instance.IsArtifactEnabled(HerCurse) && PickupCatalog.GetPickupDef(pickupIndex).itemIndex != ItemIndex.None && !burdenvariant && !Hbvst.Value && !blacklist)
            {
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerBurden.itemIndex), position, velocity);
                return;
            }
            if (pickupIndex == PickupCatalog.FindPickupIndex(HerBurden.itemIndex) && !Hbvst.Value)
            {
                switch (Mathf.FloorToInt(UnityRandom.Range(0, 6)))
                {
                    case 0:
                        orig(pickupIndex, position, velocity);
                        break;
                    case 1:
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerRecluse.itemIndex), position, velocity);
                        break;
                    case 2:
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerFury.itemIndex), position, velocity);
                        break;
                    case 3:
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerTorpor.itemIndex), position, velocity);
                        break;
                    case 4:
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerRancor.itemIndex), position, velocity);
                        break;
                    case 5:
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerPanic.itemIndex), position, velocity);
                        break;
                }
                return;
            }*/
            orig(pickupIndex, position, velocity);
        }

        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport report)
        {
            /*if (report.attackerTeamIndex == report.victimTeamIndex && report.victimMaster.minionOwnership.ownerMaster)
            {
                return;
            }*/
            if (Hbdbt.Value == true)
                return;
            if (!report.attacker || !report.attackerBody)
                return;
            if (report.victimMaster == null)
                return;
            if (report.victimMaster.minionOwnership.ownerMaster)
                return;
            if (report.attackerBody.inventory.GetItemCount(HerBurden.itemIndex) == 0 && report.attackerBody.inventory.GetItemCount(HerRecluse.itemIndex) == 0 && report.attackerBody.inventory.GetItemCount(HerFury.itemIndex) == 0 && report.attackerBody.inventory.GetItemCount(HerTorpor.itemIndex) == 0 && report.attackerBody.inventory.GetItemCount(HerRancor.itemIndex) == 0 && report.attackerBody.inventory.GetItemCount(HerPanic.itemIndex) == 0)
                return;
            bool CheckRollTrue;
            if (Hbpul.Value == true)
                CheckRollTrue = Util.CheckRoll(Hbedc.Value, report.attackerBody.master);
            else
                CheckRollTrue = Util.CheckRoll(Hbedc.Value);
            /*if (CheckRollTrue && !Hbvst.Value)
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerBurden.itemIndex), report.victimBody.corePosition, Vector3.up * 20f);*/
            if (CheckRollTrue/* && Hbvst.Value*/)
            {
                {
                    switch (Mathf.FloorToInt(UnityRandom.Range(0, 6)))
                    {
                        case 0:
                            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerBurden.itemIndex), report.victimBody.corePosition, Vector3.up * 20f);
                            break;
                        case 1:
                            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerRecluse.itemIndex), report.victimBody.corePosition, Vector3.up * 20f);
                            break;
                        case 2:
                            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerFury.itemIndex), report.victimBody.corePosition, Vector3.up * 20f);
                            break;
                        case 3:
                            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerTorpor.itemIndex), report.victimBody.corePosition, Vector3.up * 20f);
                            break;
                        case 4:
                            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerRancor.itemIndex), report.victimBody.corePosition, Vector3.up * 20f);
                            break;
                        case 5:
                            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(HerPanic.itemIndex), report.victimBody.corePosition, Vector3.up * 20f);
                            break;
                    }
                }
            }
        }

        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            if (self && self.characterBody)
            {
                CharacterBody body = self.characterBody;
                if (body && equipmentDef == HerGamble)
                {
                    if (Util.CheckRoll(90, body.master))
                        body.AddTimedBuff(ExperimentalBuff.buffIndex, 8f);
                    else if (Hbdbt.Value)
                        body.AddTimedBuff(ExperimentalDeBuff.buffIndex, 8f);
                    return true;
                }
            }
            return orig(self, equipmentDef);
        }

        private Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            if (Hbdbt.Value == false)
                return orig(self, activator);
            CharacterBody buddy = activator.GetComponent<CharacterBody>();
            bool disablecleanse = false;
            if (buddy.inventory.GetItemCount(HerBurden.itemIndex) > 0)
                disablecleanse = true;
            if (buddy.inventory.GetItemCount(HerRecluse.itemIndex) > 0)
                disablecleanse = true;
            if (buddy.inventory.GetItemCount(HerFury.itemIndex) > 0)
                disablecleanse = true;
            if (buddy.inventory.GetItemCount(HerTorpor.itemIndex) > 0)
                disablecleanse = true;
            if (buddy.inventory.GetItemCount(HerRancor.itemIndex) > 0)
                disablecleanse = true;
            if (buddy.inventory.GetItemCount(HerPanic.itemIndex) > 0)
                disablecleanse = true;
            if (self.costType == CostTypeIndex.LunarItemOrEquipment)
            {
                if (self.displayNameToken.ToLower() == "shrine_cleanse_name")
                {
                    if (buddy && buddy.inventory && disablecleanse == true)
                    {
                        return Interactability.Disabled;
                    }
                }
            }
            return orig(self, activator);
        }

        private void Duplicating_OnEnter(On.EntityStates.Duplicator.Duplicating.orig_OnEnter orig, EntityStates.Duplicator.Duplicating self)
        {
            if (Hbdbt.Value == false)
            {
                orig(self);
                return;
            }
            var lastActivator = self.GetComponent<PurchaseInteraction>().lastActivator;
            if (!lastActivator)
                return;
            var body = lastActivator.GetComponent<CharacterBody>();
            int itemcount = 0;
            List<ItemIndex> items = new List<ItemIndex>();
            if (body.inventory.GetItemCount(HerBurden.itemIndex) > 0)
                items.Add(HerBurden.itemIndex);
            if (body.inventory.GetItemCount(HerRecluse.itemIndex) > 0)
                items.Add(HerRecluse.itemIndex);
            if (body.inventory.GetItemCount(HerFury.itemIndex) > 0)
                items.Add(HerFury.itemIndex);
            if (body.inventory.GetItemCount(HerTorpor.itemIndex) > 0)
                items.Add(HerTorpor.itemIndex);
            if (body.inventory.GetItemCount(HerRancor.itemIndex) > 0)
                items.Add(HerRancor.itemIndex);
            if (body.inventory.GetItemCount(HerPanic.itemIndex) > 0)
                items.Add(HerPanic.itemIndex);
            for (int i = 0; i < items.Count; i++)
            {
                itemcount += body.inventory.GetItemCount(items[i]);
            }
            if (itemcount > 1 && Hbgoi.Value == true)
            {
                switch (Mathf.FloorToInt(UnityRandom.Range(0, items.Count)))
                {
                    case 0:
                        body.inventory.RemoveItem(items[0], 1);
                        break;
                    case 1:
                        body.inventory.RemoveItem(items[1], 1);
                        break;
                    case 2:
                        body.inventory.RemoveItem(items[2], 1);
                        break;
                    case 3:
                        body.inventory.RemoveItem(items[3], 1);
                        break;
                    case 4:
                        body.inventory.RemoveItem(items[4], 1);
                        break;
                    case 5:
                        body.inventory.RemoveItem(items[5], 1);
                        break;
                }
            }
            orig(self);
        }

        private void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);
            if (self.playerCharacterMasterController)
            {
                Size(1, body, false);
                Size(2, body, false);
            }
        }

        //This hook just updates the stack count
        private void CharacterBody_Update(On.RoR2.CharacterBody.orig_Update orig, CharacterBody self)
        {
            orig(self);
            if (Hbvsm.Value == true)
                Size(9, self, true);
            else if (Hbvsm.Value == false)
                Size(7, self, true);
            Size(8, self, true);
        }
        public void Size(int operation, CharacterBody body, bool truefalse)
        {
            if (Hbisos.Value == false)
                return;
            if (!LocalUserManager.GetFirstLocalUser().cachedMasterController.master.bodyPrefab.GetComponentInChildren<CharacterModel>().itemDisplayRuleSet)
                return;
            if (LocalUserManager.GetFirstLocalUser().cachedMasterController.master.bodyPrefab.GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.GetItemDisplayRuleGroup(VariantOnSurvivor.itemIndex).isEmpty)
                return;

            if (operation % 2 == 1)
            {
                if (truefalse)
                    if (!body.gameObject.GetComponent<BodySizeScript>())
                        return;
                if (!truefalse)
                    if (body.gameObject.GetComponent<BodySizeScript>())
                        return;
            }
            if (operation % 2 == 0)
            {
                if (truefalse)
                    if (!body.gameObject.GetComponent<FakeBodySizeScript>())
                        return;
                if (!truefalse)
                    if (body.gameObject.GetComponent<FakeBodySizeScript>())
                        return;
            }

            int burdenCount = 0;
            if (VariantOnSurvivor != HerBurden)
                burdenCount = body.inventory.GetItemCount(HerBurden.itemIndex);
            int recluseCount = 0;
            if (VariantOnSurvivor != HerRecluse)
                recluseCount = body.inventory.GetItemCount(HerRecluse.itemIndex);
            int furyCount = 0;
            if (VariantOnSurvivor != HerFury)
                furyCount = body.inventory.GetItemCount(HerFury.itemIndex);
            int torporCount = 0;
            if (VariantOnSurvivor != HerTorpor)
                torporCount = body.inventory.GetItemCount(HerTorpor.itemIndex);
            int rancorCount = 0;
            if (VariantOnSurvivor != HerRancor)
                rancorCount = body.inventory.GetItemCount(HerRancor.itemIndex);
            int panicCount = 0;
            if (VariantOnSurvivor != HerPanic)
                panicCount = body.inventory.GetItemCount(HerPanic.itemIndex);

            if (operation % 2 == 1)
                if (body.inventory.GetItemCount(VariantOnSurvivor) == 0)
                    return;
            if (operation % 2 == 0)
                if (burdenCount == 0 && recluseCount == 0 && furyCount == 0 && torporCount == 0 && rancorCount == 0 && panicCount == 0)
                    return;

            switch (operation)
            {
                case 1:
                    body.gameObject.AddComponent<BodySizeScript>();
                    body.gameObject.GetComponent<BodySizeScript>().SetBodyMultiplier(body.baseNameToken);
                    break;
                case 2:
                    body.gameObject.AddComponent<FakeBodySizeScript>();
                    body.gameObject.GetComponent<FakeBodySizeScript>().SetBodyMultiplier(body.baseNameToken, body);
                    break;
                case 3:
                    DestroyImmediate(body.gameObject.GetComponent<BodySizeScript>());
                    break;
                case 4:
                    DestroyImmediate(body.gameObject.GetComponent<FakeBodySizeScript>());
                    break;
                case 5:
                    body.gameObject.AddComponent<BodySizeScript>();
                    body.gameObject.GetComponent<BodySizeScript>().SetBodyMultiplier(body.baseNameToken);
                    body.gameObject.GetComponent<BodySizeScript>().UpdateStacks(body.inventory.GetItemCount(VariantOnSurvivor));
                    break;
                case 6:
                    if (burdenCount > 0)
                    {
                        body.gameObject.AddComponent<FakeBodySizeScript>();
                        body.gameObject.GetComponent<FakeBodySizeScript>().SetBodyMultiplier(body.baseNameToken, body);
                        body.gameObject.GetComponent<FakeBodySizeScript>().UpdateStacks(body.inventory.GetItemCount(HerBurden.itemIndex), body);
                    }
                    if (recluseCount > 0)
                    {
                        body.gameObject.AddComponent<FakeBodySizeScript>();
                        body.gameObject.GetComponent<FakeBodySizeScript>().SetBodyMultiplier(body.baseNameToken, body);
                        body.gameObject.GetComponent<FakeBodySizeScript>().UpdateStacks(body.inventory.GetItemCount(HerRecluse.itemIndex), body);
                    }
                    if (furyCount > 0)
                    {
                        body.gameObject.AddComponent<FakeBodySizeScript>();
                        body.gameObject.GetComponent<FakeBodySizeScript>().SetBodyMultiplier(body.baseNameToken, body);
                        body.gameObject.GetComponent<FakeBodySizeScript>().UpdateStacks(body.inventory.GetItemCount(HerFury.itemIndex), body);
                    }
                    if (torporCount > 0)
                    {
                        body.gameObject.AddComponent<FakeBodySizeScript>();
                        body.gameObject.GetComponent<FakeBodySizeScript>().SetBodyMultiplier(body.baseNameToken, body);
                        body.gameObject.GetComponent<FakeBodySizeScript>().UpdateStacks(body.inventory.GetItemCount(HerTorpor.itemIndex), body);
                    }
                    if (rancorCount > 0)
                    {
                        body.gameObject.AddComponent<FakeBodySizeScript>();
                        body.gameObject.GetComponent<FakeBodySizeScript>().SetBodyMultiplier(body.baseNameToken, body);
                        body.gameObject.GetComponent<FakeBodySizeScript>().UpdateStacks(body.inventory.GetItemCount(HerRancor.itemIndex), body);
                    }
                    if (panicCount > 0)
                    {
                        body.gameObject.AddComponent<FakeBodySizeScript>();
                        body.gameObject.GetComponent<FakeBodySizeScript>().SetBodyMultiplier(body.baseNameToken, body);
                        body.gameObject.GetComponent<FakeBodySizeScript>().UpdateStacks(body.inventory.GetItemCount(HerPanic.itemIndex), body);
                    }
                    break;
                case 7:
                    body.gameObject.GetComponent<BodySizeScript>().SetBodyMultiplier(body.baseNameToken);
                    body.gameObject.GetComponent<BodySizeScript>().UpdateStacks(body.inventory.GetItemCount(VariantOnSurvivor));
                    break;
                case 8:
                    if (burdenCount > 0)
                    {
                        body.gameObject.GetComponent<FakeBodySizeScript>().SetBodyMultiplier(body.baseNameToken, body);
                        body.gameObject.GetComponent<FakeBodySizeScript>().UpdateStacks(body.inventory.GetItemCount(HerBurden.itemIndex), body);
                    }
                    if (recluseCount > 0)
                    {
                        body.gameObject.GetComponent<FakeBodySizeScript>().SetBodyMultiplier(body.baseNameToken, body);
                        body.gameObject.GetComponent<FakeBodySizeScript>().UpdateStacks(body.inventory.GetItemCount(HerRecluse.itemIndex), body);
                    }
                    if (furyCount > 0)
                    {
                        body.gameObject.GetComponent<FakeBodySizeScript>().SetBodyMultiplier(body.baseNameToken, body);
                        body.gameObject.GetComponent<FakeBodySizeScript>().UpdateStacks(body.inventory.GetItemCount(HerFury.itemIndex), body);
                    }
                    if (torporCount > 0)
                    {
                        body.gameObject.GetComponent<FakeBodySizeScript>().SetBodyMultiplier(body.baseNameToken, body);
                        body.gameObject.GetComponent<FakeBodySizeScript>().UpdateStacks(body.inventory.GetItemCount(HerTorpor.itemIndex), body);
                    }
                    if (rancorCount > 0)
                    {
                        body.gameObject.GetComponent<FakeBodySizeScript>().SetBodyMultiplier(body.baseNameToken, body);
                        body.gameObject.GetComponent<FakeBodySizeScript>().UpdateStacks(body.inventory.GetItemCount(HerRancor.itemIndex), body);
                    }
                    if (panicCount > 0)
                    {
                        body.gameObject.GetComponent<FakeBodySizeScript>().SetBodyMultiplier(body.baseNameToken, body);
                        body.gameObject.GetComponent<FakeBodySizeScript>().UpdateStacks(body.inventory.GetItemCount(HerPanic.itemIndex), body);
                    }
                    break;
                case 9:
                    int temp = body.inventory.GetItemCount(VariantOnSurvivor);
                    int total = burdenCount + recluseCount + furyCount + torporCount + rancorCount + panicCount + temp;
                    body.gameObject.GetComponent<BodySizeScript>().SetBodyMultiplier(body.baseNameToken);
                    body.gameObject.GetComponent<BodySizeScript>().UpdateStacks(total);
                    break;
            }
        }
        public void WhoKnows()
        {
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                ILLabel dioend = il.DefineLabel(); //end label

                try
                {
                    c.Index = 0;

                    // find the section that the code will be injected					
                    c.GotoNext(
                        MoveType.Before,
                        x => x.MatchLdarg(0), // 1388	0D5A	ldarg.0
                        x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("Weak")), // 1389	0D5B	ldc.i4.s	0x21
                        x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff") // 1390	0D5D	call	instance bool RoR2.CharacterBody::HasBuff(valuetype RoR2.BuffIndex)
                    );

                    if (c.Index != 0)
                    {
                        c.Index++;

                        // this block is just "If artifact isn't enabled, jump to dioend label". In an item case, this should be the part where you check if you have the items or not.
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, bool>>((cb) =>
                        {
                            if (cb.master && cb.master.inventory)
                            {
                                int items = cb.master.inventory.GetItemCount(HerBurden.itemIndex);
                                int items2 = cb.master.inventory.GetItemCount(HerRecluse.itemIndex);
                                int items3 = cb.master.inventory.GetItemCount(HerFury.itemIndex);
                                int items4 = cb.master.inventory.GetItemCount(HerTorpor.itemIndex);
                                int items5 = cb.master.inventory.GetItemCount(HerRancor.itemIndex);
                                int items6 = cb.master.inventory.GetItemCount(HerPanic.itemIndex);
                                if (items > 0 || items2 > 0 || items3 > 0 || items4 > 0 || items5 > 0 || items6 > 0) return true;
                                return false;
                            }
                            return false;
                        });
                        c.Emit(OpCodes.Brfalse, dioend);

                        // this.maxHealth
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_maxHealth")); // 1414 0DA3	call	instance float32 RoR2.CharacterBody::get_maxHealth()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.maxHealth *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                        {
                            float healthmultiplier = 1;
                            if (cb.master && cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(HerBurden.itemIndex);
                                if (itemCount > 0)
                                {
                                    if (cb.GetBuffCount(ExperimentalBuff.buffIndex) > 0)
                                        healthmultiplier *= Mathf.Pow(Hbbuff.Value, itemCount) * 2;
                                    else
                                        healthmultiplier *= Mathf.Pow(Hbbuff.Value, itemCount);
                                }
                                int itemCount2 = cb.master.inventory.GetItemCount(HerFury.itemIndex);
                                if (itemCount2 > 0 && Hbdbt.Value == true)
                                {
                                    if (cb.GetBuffCount(ExperimentalDeBuff.buffIndex) > 0)
                                        healthmultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2) / 2;
                                    else
                                        healthmultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2);
                                }
                            }
                            return healthmultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_maxHealth", BindingFlags.Instance | BindingFlags.NonPublic));

                        // this.attackSpeed
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_attackSpeed")); // 1426 0DC7	call	instance float32 RoR2.CharacterBody::get_attackSpeed()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.attackSpeed *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                        {
                            float attackSpeedmultiplier = 1;
                            if (cb.master && cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(HerFury.itemIndex);
                                if (itemCount > 0)
                                {
                                    if (cb.GetBuffCount(ExperimentalBuff.buffIndex) > 0)
                                        attackSpeedmultiplier *= Mathf.Pow(Hbbuff.Value, itemCount) * 2;
                                    else
                                        attackSpeedmultiplier *= Mathf.Pow(Hbbuff.Value, itemCount);
                                }
                                int itemCount2 = cb.master.inventory.GetItemCount(HerTorpor.itemIndex);
                                if (itemCount2 > 0 && Hbdbt.Value == true)
                                {
                                    if (cb.GetBuffCount(ExperimentalDeBuff.buffIndex) > 0)
                                        attackSpeedmultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2) / 2;
                                    else
                                        attackSpeedmultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2);
                                }
                            }
                            return attackSpeedmultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_attackSpeed", BindingFlags.Instance | BindingFlags.NonPublic));

                        // this.moveSpeed
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_moveSpeed")); // 1406	0D8A	call	instance float32 RoR2.CharacterBody::get_moveSpeed()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.moveSpeed *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                        {
                            float moveSpeedmultiplier = 1;
                            if (cb.master && cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(HerPanic.itemIndex);
                                if (itemCount > 0)
                                {
                                    if (cb.GetBuffCount(ExperimentalBuff.buffIndex) > 0)
                                        moveSpeedmultiplier *= Mathf.Pow(Hbbuff.Value, itemCount) * 2;
                                    else
                                        moveSpeedmultiplier *= Mathf.Pow(Hbbuff.Value, itemCount);
                                }
                                int itemCount2 = cb.master.inventory.GetItemCount(HerBurden.itemIndex);
                                if (itemCount2 > 0 && Hbdbt.Value == true)
                                {
                                    if (cb.GetBuffCount(ExperimentalDeBuff.buffIndex) > 0)
                                        moveSpeedmultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2) / 2;
                                    else
                                        moveSpeedmultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2);
                                }
                            }
                            return moveSpeedmultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_moveSpeed", BindingFlags.Instance | BindingFlags.NonPublic));

                        // this.armor2
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_armor")); // 1438 0DE8	call	instance float32 RoR2.CharacterBody::get_armor()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.armor *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                        {
                            float armormultiplier = 0;
                            if (cb.master && cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(HerRecluse.itemIndex);
                                if (itemCount > 0)
                                {
                                    armormultiplier += 5;
                                }
                            }
                            return armormultiplier;
                        });
                        c.Emit(OpCodes.Add);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_armor", BindingFlags.Instance | BindingFlags.NonPublic));

                        // this.armor
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_armor")); // 1438 0DE8	call	instance float32 RoR2.CharacterBody::get_armor()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.armor *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                        {
                            float armormultiplier = 1;
                            if (cb.master && cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(HerRecluse.itemIndex);
                                if (itemCount > 1)
                                {
                                    if (cb.GetBuffCount(ExperimentalBuff.buffIndex) > 0)
                                        armormultiplier *= Mathf.Pow(Hbbuff.Value, itemCount) * 2;
                                    else
                                        armormultiplier *= Mathf.Pow(Hbbuff.Value, itemCount - 1);
                                }
                                int itemCount2 = cb.master.inventory.GetItemCount(HerRancor.itemIndex);
                                if (itemCount2 > 0 && Hbdbt.Value == true)
                                {
                                    if (cb.GetBuffCount(ExperimentalDeBuff.buffIndex) > 0)
                                        armormultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2) / 2;
                                    else
                                        armormultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2);
                                }
                            }
                            return armormultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_armor", BindingFlags.Instance | BindingFlags.NonPublic));

                        // this.damage
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_damage")); // 1444 0DFD	call	instance float32 RoR2.CharacterBody::get_damage()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.damage *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                        {
                            float damagemultiplier = 1;
                            if (cb.master && cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(HerRancor.itemIndex);
                                if (itemCount > 0)
                                {
                                    if (cb.GetBuffCount(ExperimentalBuff.buffIndex) > 0)
                                        damagemultiplier *= Mathf.Pow(Hbbuff.Value, itemCount) * 2;
                                    else
                                        damagemultiplier *= Mathf.Pow(Hbbuff.Value, itemCount);
                                }
                                int itemCount2 = cb.master.inventory.GetItemCount(HerPanic.itemIndex);
                                if (itemCount2 > 0 && Hbdbt.Value == true)
                                {
                                    if (cb.GetBuffCount(ExperimentalDeBuff.buffIndex) > 0)
                                        damagemultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2) / 2;
                                    else
                                        damagemultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2);
                                }
                            }
                            return damagemultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_damage", BindingFlags.Instance | BindingFlags.NonPublic));

                        // this.regen
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_regen")); // 1450 0E0F	call	instance float32 RoR2.CharacterBody::get_regen()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.regen *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                        {
                            float regenmultiplier = 1;
                            if (cb.master && cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(HerTorpor.itemIndex);
                                if (itemCount > 0)
                                {
                                    if (cb.GetBuffCount(ExperimentalBuff.buffIndex) > 0)
                                        regenmultiplier *= Mathf.Pow(Hbbuff.Value, itemCount) * 2;
                                    else
                                        regenmultiplier *= Mathf.Pow(Hbbuff.Value, itemCount);
                                }
                                int itemCount2 = cb.master.inventory.GetItemCount(HerRecluse.itemIndex);
                                if (itemCount2 > 0 && Hbdbt.Value == true)
                                {
                                    if (cb.GetBuffCount(ExperimentalDeBuff.buffIndex) > 0)
                                        regenmultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2) / 2;
                                    else
                                        regenmultiplier *= Mathf.Pow(Hbdebuff.Value, itemCount2);
                                }
                            }
                            return regenmultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_regen", BindingFlags.Instance | BindingFlags.NonPublic));

                        c.MarkLabel(dioend); // end label
                    }

                }
                catch (Exception ex) { base.Logger.LogError(ex); }
            };
        }
    }
    public class Content : IContentPackProvider
    {
        public ContentPack contentPack = new ContentPack();

        public string identifier
        {
            get { return "Content"; }
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            EquipmentDef[] equipmentDefs = new EquipmentDef[] { Her_Burden.HerGamble };
            ArtifactDef[] artifactDefs = new ArtifactDef[] { Her_Burden.HerCurse };
            contentPack.identifier = identifier;
            contentPack.equipmentDefs.Add(equipmentDefs);
            contentPack.artifactDefs.Add(artifactDefs);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}