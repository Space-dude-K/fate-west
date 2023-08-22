using System.Collections.Generic;
using System.IO;

namespace wc3_fate_west_web.Models
{
    public static class ContentURL
    {
        private static readonly Dictionary<string, string> _heroIconDic = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _ghIconDic = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _attributeDic = new Dictionary<string, string>();

        static ContentURL()
        {
            _heroIconDic.Add("H028", @"/images/icons/heroes/BTNAvenger.jpg");
            _heroIconDic.Add("H000", @"/images/icons/heroes/BTNSaber.jpg");
            _heroIconDic.Add("H002", @"/images/icons/heroes/BTNChulainn.jpg");
            _heroIconDic.Add("H004", @"/images/icons/heroes/BTNCaster.jpg");
            _heroIconDic.Add("H001", @"/images/icons/heroes/BTNArcherIcon.jpg");
            _heroIconDic.Add("H005", @"/images/icons/heroes/BTNAssassinIcon.jpg");
            _heroIconDic.Add("H006", @"/images/icons/heroes/BTNBerserker.jpg");
            _heroIconDic.Add("H007", @"/images/icons/heroes/BTNDarkSaber.jpg");
            _heroIconDic.Add("H00Y", @"/images/icons/heroes/BTNDrake.jpg");
            _heroIconDic.Add("H00I", @"/images/icons/heroes/BTNGawain.jpg");
            _heroIconDic.Add("H009", @"/images/icons/heroes/BTNGilgamesh.jpg");
            _heroIconDic.Add("E002", @"/images/icons/heroes/BTNGilles.jpg");
            _heroIconDic.Add("H00A", @"/images/icons/heroes/BTNIskander.jpg");
            _heroIconDic.Add("H021", @"/images/icons/heroes/BTNKarna.jpg");
            _heroIconDic.Add("H03M", @"/images/icons/heroes/BTNLancelot.jpg");
            _heroIconDic.Add("H01X", @"/images/icons/heroes/BTNNero.jpg");
            _heroIconDic.Add("H024", @"/images/icons/heroes/BTNNursery.jpg");
            _heroIconDic.Add("H003", @"/images/icons/heroes/BTNRider.jpg");
            _heroIconDic.Add("H01T", @"/images/icons/heroes/BTNRobin.jpg");
            _heroIconDic.Add("H00E", @"/images/icons/heroes/BTNTamamo.jpg");
            _heroIconDic.Add("H008", @"/images/icons/heroes/BTNTrueAssassin.jpg");
            _heroIconDic.Add("H01F", @"/images/icons/heroes/BTNVlad.jpg");
            _heroIconDic.Add("H01Q", @"/images/icons/heroes/BTNYeopo.jpg");
            _heroIconDic.Add("H04D", @"/images/icons/heroes/BTNZeroLancer.jpg");
            _heroIconDic.Add("H01A", @"/images/icons/heroes/BTNLi.jpg");
            _heroIconDic.Add("H02W", @"/images/icons/heroes/BTNElizabeth.png");
            _ghIconDic.Add("A04B", @"/images/icons/GHGold.jpg");
            _ghIconDic.Add("A04F", @"/images/icons/GHLevelUp.jpg");
            _ghIconDic.Add("A0DD", @"/images/icons/GHStats.jpg");
            _ghIconDic.Add("A04C", @"/images/icons/GHAMP.jpg");
            _ghIconDic.Add("A04D", @"/images/icons/GHFullHealPot.jpg");
            _ghIconDic.Add("A00U", @"/images/icons/GHInvulBird.jpg");
            _attributeDic.Add("A0GO", @"/images/icons/attributes/SphereBoundary.png");
            _attributeDic.Add("A0GP", @"/images/icons/attributes/FierceTigerForciblyClimbsaMountain.png");
            _attributeDic.Add("A0GF", @"/images/icons/attributes/CirculatoryShock.png");
            _attributeDic.Add("A0GG", @"/images/icons/attributes/DoubleClass.png");
            _attributeDic.Add("A077", @"/images/icons/attributes/ImprovedTerritory.png");
            _attributeDic.Add("A082", @"/images/icons/attributes/DivineLanguage.png");
            _attributeDic.Add("A07R", @"/images/icons/attributes/ImprovedHecaticGraea.png");
            _attributeDic.Add("A07H", @"/images/icons/attributes/ImprovedAegis.png");
            _attributeDic.Add("A07B", @"/images/icons/attributes/ImprovedGaeBolg.png");
            _attributeDic.Add("A07L", @"/images/icons/attributes/FlyingSpearofBarbedDeath.png");
            _attributeDic.Add("A07V", @"/images/icons/attributes/SpearofDeath.png");
            _attributeDic.Add("A0GL", @"/images/icons/attributes/ImprovedRunes.png");
            _attributeDic.Add("A071", @"/images/icons/attributes/ImprovedBattleContinuation.png");
            _attributeDic.Add("A08K", @"/images/icons/attributes/ProtectionofFairies.png");
            _attributeDic.Add("A08M", @"/images/icons/attributes/HonoroftheShiningLake.png");
            _attributeDic.Add("A08J", @"/images/icons/attributes/EternalArmsMastership.png");
            _attributeDic.Add("A08L", @"/images/icons/attributes/ImprovedArondight.png");
            _attributeDic.Add("A081", @"/images/icons/attributes/PhantomAttack.png");
            _attributeDic.Add("A0H0", @"/images/icons/attributes/DelusionalIllusion.png");
            _attributeDic.Add("A07Q", @"/images/icons/attributes/Zabaniya.png");
            _attributeDic.Add("A07G", @"/images/icons/attributes/ProtectionfromWind.png");
            _attributeDic.Add("A07A", @"/images/icons/attributes/Riding.png");
            _attributeDic.Add("A07K", @"/images/icons/attributes/Seal.png");
            _attributeDic.Add("A07U", @"/images/icons/attributes/MonstrousStrength.png");
            _attributeDic.Add("A070", @"/images/icons/attributes/ImprovedMysticEyes.png");
            _attributeDic.Add("A074", @"/images/icons/attributes/ImprovedClairvoyance.png");
            _attributeDic.Add("A07E", @"/images/icons/attributes/Hrunting.png");
            _attributeDic.Add("A07Y", @"/images/icons/attributes/ImprovedTracing.png");
            _attributeDic.Add("A07I", @"/images/icons/attributes/ManaBlast.png");
            _attributeDic.Add("A078", @"/images/icons/attributes/ImprovedManaShroud.png");
            _attributeDic.Add("A083", @"/images/icons/attributes/BlackLight-DarkExcalibur.png");
            _attributeDic.Add("A0BV", @"/images/icons/attributes/ImprovedFerocity.png");
            _attributeDic.Add("A0AC", @"/images/icons/attributes/BloomingofYellow-RedRose.png");
            _attributeDic.Add("A0AE", @"/images/icons/attributes/DoubleSpear.png");
            _attributeDic.Add("A08P", @"/images/icons/attributes/LoveSpotOfSeduction.png");
            _attributeDic.Add("A073", @"/images/icons/attributes/ImprovedExcalibur.png");
            _attributeDic.Add("A0FZ", @"/images/icons/attributes/StrikeAir.png");
            _attributeDic.Add("A07D", @"/images/icons/attributes/ImprovedInstincts.png");
            _attributeDic.Add("A0K9", @"/images/icons/attributes/DragonSkillet.png");
            _attributeDic.Add("A0KC", @"/images/icons/attributes/RainbowPlainsandtheMazeofMirrors.png");
            _attributeDic.Add("A0KD", @"/images/icons/attributes/TheKingofBlackandWhiteCheckerboard.png");
            _attributeDic.Add("A0D3", @"/images/icons/attributes/ImprovedBlackMagic.png");
            _attributeDic.Add("A0CY", @"/images/icons/attributes/ImprovedDemonicCreatureoftheOceanDepths.png");
            _attributeDic.Add("A0CV", @"/images/icons/attributes/Contagion.png");
            _attributeDic.Add("A0J2", @"/images/icons/attributes/KeytotheTheater.png");
            _attributeDic.Add("A0IW", @"/images/icons/attributes/EmbryonicFlame.png");
            _attributeDic.Add("A0IX", @"/images/icons/attributes/ImperialPrivilege.png");
            _attributeDic.Add("A0J0", @"/images/icons/attributes/ThriceThoughIWelcometheSettingSun.png");
            _attributeDic.Add("A0JS", @"/images/icons/attributes/ImprovedVasaviShakti.png");
            _attributeDic.Add("A0JQ", @"/images/icons/attributes/ImprovedDivinity(Karna).png");
            _attributeDic.Add("A0JR", @"/images/icons/attributes/ImprovedBrahmastra.png");
            _attributeDic.Add("A0I9", @"/images/icons/attributes/SacramentumofDruid.png");
            _attributeDic.Add("A0I8", @"/images/icons/attributes/SherwoodForest.png");
            _attributeDic.Add("A0I6", @"/images/icons/attributes/OneDropofaViper.png");
            _attributeDic.Add("A0I7", @"/images/icons/attributes/PoisonedArrow.png");
            _attributeDic.Add("A07N", @"/images/icons/attributes/Knighthood.png");
            _attributeDic.Add("A07X", @"/images/icons/attributes/ImprovedCharisma.png");
            _attributeDic.Add("A06F", @"/images/icons/attributes/ImprovedIonionHetairoi.png");
            _attributeDic.Add("A053", @"/images/icons/attributes/ImprovedCharisma(ZR).png");
            _attributeDic.Add("A046", @"/images/icons/attributes/ImprovedMilitaryTactics.png");
            _attributeDic.Add("A0BP", @"/images/icons/attributes/EyeforArt.png");
            _attributeDic.Add("A07Z", @"/images/icons/attributes/QuickDraw.png");
            _attributeDic.Add("A07P", @"/images/icons/attributes/Vitrification.png");
            _attributeDic.Add("A09O", @"/images/icons/attributes/MonohoshiZao.png");
            _attributeDic.Add("A075", @"/images/icons/attributes/EyeoftheMind.png");
            _attributeDic.Add("A06C", @"/images/icons/attributes/Nightmare.png");
            _attributeDic.Add("A069", @"/images/icons/attributes/CurseofBlood.png");
            _attributeDic.Add("A060", @"/images/icons/attributes/CurseofBlood2.png");
            _attributeDic.Add("A0BQ", @"/images/icons/attributes/AvengerofBlood.png");
            _attributeDic.Add("A066", @"/images/icons/attributes/TawrichandZarich.png");
            _attributeDic.Add("A0BM", @"/images/icons/attributes/SpiritOrb.png");
            _attributeDic.Add("A0C8", @"/images/icons/attributes/Witchcraft.png");
            _attributeDic.Add("A0BZ", @"/images/icons/attributes/CursedCharms.png");
            _attributeDic.Add("A0C1", @"/images/icons/attributes/ImprovedTerritory(Tamamo).png");
            _attributeDic.Add("A0KA", @"/images/icons/attributes/AckroydinCelluloid.png");
            _attributeDic.Add("A0JT", @"/images/icons/attributes/PranaBurst(Karna).png");
            _attributeDic.Add("A00N", @"/images/icons/attributes/KanshoBakuyaOveredge.png");
            _attributeDic.Add("A0I5", @"/images/icons/attributes/TheFacelessKing.png");
            _attributeDic.Add("A05N", @"/images/icons/attributes/WheelofGordias.png");
            _attributeDic.Add("A05F", @"/images/icons/attributes/ImprovedSpatha.png");
            _attributeDic.Add("A000", @"/images/icons/attributes/ImprovedDivinity.png");
            _attributeDic.Add("A07J", @"/images/icons/attributes/ImprovedSwordRain.png");
            _attributeDic.Add("A07T", @"/images/icons/attributes/TheStarofCreationthatSplitHeavenandEarth.png");
            _attributeDic.Add("A0ED", @"/images/icons/attributes/SwordofVictory.png");
            _attributeDic.Add("A0EC", @"/images/icons/attributes/NumeraloftheSaint.png");
            _attributeDic.Add("A072", @"/images/icons/attributes/SacredMirror.png");
            _attributeDic.Add("A07F", @"/images/icons/attributes/Ganryu-ShoreWillow.png");
            _attributeDic.Add("A06S", @"/images/icons/attributes/ImprovedGoldenRule.png");
            _attributeDic.Add("A079", @"/images/icons/attributes/PowerofSumer.png");
            _attributeDic.Add("A07W", @"/images/icons/attributes/Reincarnation.png");
            _attributeDic.Add("A07M", @"/images/icons/attributes/GodHand.png");
            _attributeDic.Add("A0J1", @"/images/icons/attributes/ImprovedFountainoftheSaint.png");
            _attributeDic.Add("A0HF", @"/images/icons/attributes/Houtengageki.png");
            _attributeDic.Add("A0HL", @"/images/icons/attributes/Bravery.png");
            _attributeDic.Add("A0HK", @"/images/icons/attributes/GateHalberdShot.png");
            _attributeDic.Add("A0FE", @"/images/icons/attributes/CombustionShot.png");
            _attributeDic.Add("A0FG", @"/images/icons/attributes/ImprovedGoldenHind.png");
            _attributeDic.Add("A0FH", @"/images/icons/attributes/GoldenRule_Pillage.png");
            _attributeDic.Add("A0EB", @"/images/icons/attributes/Knighthood(Gawain).png");
            _attributeDic.Add("A0EA", @"/images/icons/attributes/ProtectionoftheFairies(Gawain).png");
            _attributeDic.Add("A07C", @"/images/icons/attributes/MadEnhancement.png");
            _attributeDic.Add("A0E9", @"/images/icons/attributes/Gwalhmai.png");
            _attributeDic.Add("A07O", @"/images/icons/attributes/ShroudofMartin.png");
            _attributeDic.Add("A0FD", @"/images/icons/attributes/Logbook.png");
            _attributeDic.Add("A0AT", @"/images/icons/attributes/LordofExecution.png");
            _attributeDic.Add("A0B2", @"/images/icons/attributes/RebelliousIntent.png");
            _attributeDic.Add("A0AK", @"/images/icons/attributes/InnocentMonster.png");
            _attributeDic.Add("A0AS", @"/images/icons/attributes/ProtectionoftheFaith.png");
            _attributeDic.Add("A0HE", @"/images/icons/attributes/ImmortalRedHare.png");
            _attributeDic.Add("A0HD", @"/images/icons/attributes/RedHare.png");
            _attributeDic.Add("A0KB", @"/images/icons/attributes/VorpalBlade.png");
            _attributeDic.Add("A0JP", @"/images/icons/attributes/UncrownedArmsMastership.png");
            _attributeDic.Add("A0AD", @"/images/icons/attributes/Mind'sEye(True).png");
            _attributeDic.Add("A076", @"/images/icons/attributes/ImprovedPresenceConcealment.png");
            _attributeDic.Add("A0LD", @"/images/icons/attributes/BTNAspectOfDragon.png");
            _attributeDic.Add("A0LF", @"/images/icons/attributes/BTNDoubleClassEliz.png");
            _attributeDic.Add("A0LE", @"/images/icons/attributes/BTNInnocentMonsterEliz.png");
            _attributeDic.Add("A0LC", @"/images/icons/attributes/BTNSadisticCharisma.png");
            _attributeDic.Add("A0LK", @"/images/icons/attributes/BTNTortureTechniques.png");
        }

        public static string GetAttributeIconURL(string attributeAbilId)
        {
            string url;
            _attributeDic.TryGetValue(attributeAbilId, out url);
            return url;
        }

        public static string GetHeroIconURL(string heroUnitTypeId)
        {
            string url;
            _heroIconDic.TryGetValue(heroUnitTypeId, out url);
            return url;
        }

        public static string GetGodsHelpIconURL(string godsHelpAbilId)
        {
            string url;
            _ghIconDic.TryGetValue(godsHelpAbilId, out url);
            return url;
        }

        public static string GetLocalReplayPath(string replayPath)
        {
            return "Content\\replays\\" + Path.GetFileName(replayPath);
        }
    }
}