using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using wc3_fate_west_data_access_layer.Data;

namespace wc3_fate_west_web.Models
{
    public class ServantDetailSL
    {
        private readonly StatisticsSL _detailDal;

        public ServantDetailSL(StatisticsSL detailDal)
        {
            _detailDal = detailDal;
        }

        public Dictionary<string, int> BuildVersionPlayedData(Dictionary<string, int> versionPlayed)
        {
            Dictionary<string, int> dataModel = new Dictionary<string, int>();

            foreach (var versionData in versionPlayed)
            {
                string[] versionStrings = versionData.Key.Split(new[] { "Ver " }, StringSplitOptions.None);
                string versionString = versionStrings[1];
                string[] versionNumbers = versionString.Split('.');
                string versionMainNumber = versionNumbers[0];
                string versionSubNumber = Regex.Replace(versionNumbers[1], "[^0-9]+", string.Empty);
                string version = versionMainNumber + "." + versionSubNumber;
                if (dataModel.ContainsKey(version))
                {
                    dataModel[version] += versionData.Value;
                }
                else
                {
                    dataModel[version] = versionData.Value;
                }

            }

            return dataModel;
        }

        public List<ServantDetailStatData> BuildServantStatPointData(List<ServantDetailStatData> servantStatPoints)
        {
            List<ServantDetailStatData> statDataModel = new List<ServantDetailStatData>();
            foreach (var versionStat in servantStatPoints)
            {
                string[] versionStrings = versionStat.MapVersion.Split(new[] { "Ver " }, StringSplitOptions.None);
                string versionString = versionStrings[1];
                string[] versionNumbers = versionString.Split('.');
                string versionMainNumber = versionNumbers[0];
                string versionSubNumber = Regex.Replace(versionNumbers[1], "[^0-9]+", string.Empty);
                string version = versionMainNumber + "." + versionSubNumber;
                int index = statDataModel.FindIndex(c => (c.MapVersion == version) && (c.StatTypeId == versionStat.StatTypeId));
                if (index != -1)
                {
                    statDataModel[index].Points += versionStat.Points;
                }
                else
                {
                    ServantDetailStatData statData = new ServantDetailStatData();
                    statData.MapVersion = version;
                    statData.StatTypeId = versionStat.StatTypeId;
                    statData.StatTypeName = versionStat.StatTypeName;
                    statData.Points = versionStat.Points;
                    statDataModel.Add(statData);
                }
            }
            statDataModel = statDataModel.OrderByDescending(c => Convert.ToInt32(c.MapVersion.Split('.')[0]))
                                           .ThenByDescending(c => Convert.ToInt32(c.MapVersion.Split('.')[1])).ToList();

            return statDataModel;
        }

        public List<ServantDetailData> BuildServantDetailData(List<ServantDetailData> servantDetails)
        {
            List<ServantDetailData> dataModel = new List<ServantDetailData>();

            foreach (var versionData in servantDetails)
            {

                string[] versionStrings = versionData.MapVersion.Split(new[] { "Ver " }, StringSplitOptions.None);
                string versionString = versionStrings[1];
                string[] versionNumbers = versionString.Split('.');
                string versionMainNumber = versionNumbers[0];
                string versionSubNumber = Regex.Replace(versionNumbers[1], "[^0-9]+", string.Empty);
                string version = versionMainNumber + "." + versionSubNumber;
                int index = dataModel.FindIndex(c => c.MapVersion.Contains(version));
                if (index != -1)
                {
                    dataModel[index].PlayCount += versionData.PlayCount;
                    dataModel[index].WinCount += versionData.WinCount;
                    dataModel[index].KillCount += versionData.KillCount;
                    dataModel[index].AssistCount += versionData.AssistCount;
                    dataModel[index].DeathCount += versionData.DeathCount;
                    dataModel[index].DamageTaken += versionData.DamageTaken;
                    dataModel[index].DamageDealt += versionData.DamageDealt;
                    dataModel[index].GoldSpent += versionData.GoldSpent;
                    dataModel[index].WinScoreDifference += versionData.WinScoreDifference;
                    dataModel[index].LossScoreDifference += versionData.LossScoreDifference;
                    dataModel[index].GameDuration += versionData.GameDuration;
                }
                else
                {
                    ServantDetailData servantDetail = new ServantDetailData();
                    servantDetail.PlayCount = versionData.PlayCount;
                    servantDetail.KillCount = versionData.KillCount;
                    servantDetail.AssistCount = versionData.AssistCount;
                    servantDetail.DeathCount = versionData.DeathCount;
                    servantDetail.DamageTaken = versionData.DamageTaken;
                    servantDetail.DamageDealt = versionData.DamageDealt;
                    servantDetail.GoldSpent = versionData.GoldSpent;
                    servantDetail.WinScoreDifference = versionData.WinScoreDifference;
                    servantDetail.LossScoreDifference = versionData.LossScoreDifference;
                    servantDetail.GameDuration = versionData.GameDuration;
                    servantDetail.HeroTitle = versionData.HeroTitle;
                    servantDetail.WinCount = versionData.WinCount;
                    servantDetail.MapVersion = version;
                    dataModel.Add(servantDetail);
                }
            }

            dataModel = dataModel.OrderByDescending(c => Convert.ToInt32(c.MapVersion.Split('.')[0]))
                                       .ThenByDescending(c => Convert.ToInt32(c.MapVersion.Split('.')[1])).ToList();

            return (dataModel.Count > 5) ? dataModel.GetRange(0, 5) : dataModel;
        }

        public Dictionary<string, int> ParseServantStatPointId(List<ServantDetailStatData> versionStatList)
        {
            Dictionary<string, int> parsedStat = new Dictionary<string, int>();
            parsedStat["str"] = 0;
            parsedStat["agi"] = 0;
            parsedStat["int"] = 0;
            parsedStat["atk"] = 0;
            parsedStat["armor"] = 0;
            parsedStat["healthRegen"] = 0;
            parsedStat["manaRegen"] = 0;
            parsedStat["movespeed"] = 0;
            parsedStat["goldRegen"] = 0;
            parsedStat["prelati"] = 0;

            Dictionary<string, string> statPointDic = new Dictionary<string, string>();
            statPointDic.Add("A02W", @"str");
            statPointDic.Add("A03D", @"agi");
            statPointDic.Add("A03E", @"int");
            statPointDic.Add("A03W", @"atk");
            statPointDic.Add("A03X", @"armor");
            statPointDic.Add("A03Y", @"healthRegen");
            statPointDic.Add("A03Z", @"manaRegen");
            statPointDic.Add("A04Y", @"movespeed");
            statPointDic.Add("A0A9", @"goldRegen");
            statPointDic.Add("A0CJ", @"prelati");
            foreach (var versionStat in versionStatList)
            {
                string statType;
                statPointDic.TryGetValue(versionStat.StatTypeId, out statType);
                parsedStat[statType] = versionStat.Points;
            }
            return parsedStat;
        }

        public List<ServantStatisticsDetailViewModel> GetServantDetail(int heroId)
        {
            List<ServantStatisticsDetailViewModel> vm = new List<ServantStatisticsDetailViewModel>();
            List<ServantDetailData> servantDetailVersionData = this.BuildServantDetailData(_detailDal.GetServantDetailStatistics(heroId));
            List<ServantDetailStatData> servantDetailStatData = this.BuildServantStatPointData(_detailDal.GetServantDetailStatPoints(heroId));
            Dictionary<string, int> totalGamesPlayedVersion = this.BuildVersionPlayedData(_detailDal.GetTotalGamesPlayedVersion());

            foreach (var versionData in servantDetailVersionData)
            {
                ServantStatisticsDetailViewModel vmu = new ServantStatisticsDetailViewModel();

                int totalPlayed = totalGamesPlayedVersion[versionData.MapVersion];
                decimal winRate = Convert.ToDecimal(versionData.WinCount * 100 / versionData.PlayCount);
                vmu.WinRate = $"{Math.Round(winRate, 1)}";
                decimal pickRate = Convert.ToDecimal(versionData.PlayCount * 100 / totalPlayed);
                vmu.PickRate = $"{Math.Round(pickRate, 1)}";
                int avgKills = versionData.KillCount / versionData.PlayCount;
                vmu.AvgKills = $"{avgKills}";
                int avgDeaths = versionData.DeathCount / versionData.PlayCount;
                vmu.AvgDeaths = $"{avgDeaths}";
                int avgAssists = versionData.AssistCount / versionData.PlayCount;
                vmu.AvgAssists = $"{avgAssists}";
                decimal avgKDA = Convert.ToDecimal(versionData.AssistCount + versionData.KillCount) / versionData.DeathCount;
                vmu.AvgKDA = $"{Math.Round(avgKDA, 2)}";
                vmu.PlayCount = versionData.PlayCount;
                vmu.MapVersion = versionData.MapVersion;
                int avgGoldSpent = Convert.ToInt32(versionData.GoldSpent / versionData.PlayCount);
                vmu.AvgGoldSpent = $"{avgGoldSpent}";
                int avgDamageDealt = Convert.ToInt32(versionData.DamageDealt / versionData.PlayCount);
                vmu.AvgDamageDealt = $"{avgDamageDealt}";
                int avgDamageTaken = Convert.ToInt32(versionData.DamageTaken / versionData.PlayCount);
                vmu.AvgDamageTaken = $"{avgDamageTaken}";
                TimeSpan time = TimeSpan.FromSeconds(Convert.ToInt32(versionData.GameDuration / versionData.PlayCount));
                string timeString = time.ToString(@"hh\:mm\:ss");
                vmu.AvgGameDuration = timeString;
                decimal avgWinScoreDifference = versionData.WinCount > 0 ? Convert.ToDecimal(versionData.WinScoreDifference / versionData.WinCount) : decimal.Zero;
                vmu.AvgWinScoreDifference = $"{Math.Round(avgWinScoreDifference, 1)}";
                decimal avgLossScoreDifference = (versionData.PlayCount - versionData.WinCount) > 0 ? Convert.ToDecimal(versionData.LossScoreDifference / (versionData.PlayCount - versionData.WinCount)) : decimal.Zero;
                vmu.AvgLossScoreDifference = $"{Math.Round(avgLossScoreDifference, 1)}";

                List<ServantDetailStatData> versionStatList = servantDetailStatData.FindAll(c => (c.MapVersion == versionData.MapVersion));
                Dictionary<string, int> parsedStat = ParseServantStatPointId(versionStatList);

                decimal avgStr = Convert.ToDecimal(parsedStat["str"] / versionData.PlayCount);
                vmu.AvgStr = $"{Math.Round(avgStr, 1)}";
                decimal avgAgi = Convert.ToDecimal(parsedStat["agi"] / versionData.PlayCount);
                vmu.AvgAgi = $"{Math.Round(avgAgi, 1)}";
                decimal avgInt = Convert.ToDecimal(parsedStat["int"] / versionData.PlayCount);
                vmu.AvgInt = $"{Math.Round(avgInt, 1)}";
                decimal avgAtk = Convert.ToDecimal(parsedStat["atk"] / versionData.PlayCount);
                vmu.AvgAtk = $"{Math.Round(avgAtk, 1)}";
                decimal avgArmor = Convert.ToDecimal(parsedStat["armor"] / versionData.PlayCount);
                vmu.AvgArmor = $"{Math.Round(avgArmor, 1)}";
                decimal avgHealthRegen = Convert.ToDecimal(parsedStat["healthRegen"] / versionData.PlayCount);
                vmu.AvgHealthRegen = $"{Math.Round(avgHealthRegen, 1)}";
                decimal avgManaRegen = Convert.ToDecimal(parsedStat["manaRegen"] / versionData.PlayCount);
                vmu.AvgManaRegen = $"{Math.Round(avgManaRegen, 1)}";
                decimal avgGoldRegen = Convert.ToDecimal(parsedStat["goldRegen"] / versionData.PlayCount);
                vmu.AvgGoldRegen = $"{Math.Round(avgGoldRegen, 1)}";
                decimal avgPrelati = Convert.ToDecimal(parsedStat["prelati"] / versionData.PlayCount);
                vmu.AvgPrelati = $"{Math.Round(avgPrelati, 1)}";
                decimal avgMovespeed = Convert.ToDecimal(parsedStat["movespeed"] / versionData.PlayCount);
                vmu.AvgMovespeed = $"{Math.Round(avgMovespeed, 1)}";
                vm.Add(vmu);
            }
            return vm;
        }
    }
}
