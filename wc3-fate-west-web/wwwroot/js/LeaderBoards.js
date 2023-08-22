$(document).ready(function () {
    $("#leaderBoardsTable").tablesorter();

    $('input[name=radioCriteriaType]').change(function () {
        refreshStatistics();
    });

    $('input[name=radioCriteriaTime]').change(function () {
        refreshStatistics();
    });

    refreshStatistics();
});

function refreshStatistics() {
    var filterType = $('input[name=radioCriteriaType]:checked').val();
    var timeType = $('input[name=radioCriteriaTime]:checked').val();
    var tableBody = $("#leaderBoardsTable tbody");
    tableBody.empty();
    $.get("/PlayerStatsAjax/Statistics/" + filterType + "/" + timeType, function (data)
    {
        for (var key in data)
        {
            var x = data[key];

            var tableRow = $('<tr></tr>');
            var playerTd = $('<td><b><a href="/PlayerStats/test/' + x.playerName + '">' + x.playerName + '</a></b></td>');
            var winRatioTd = $('<td class="leftText"><div class="'
                + x.winRatioPBColor + '" style="width: ' + x.winRatio + '"></div><span class="winrateval">' + x.winRatio + '</span></td>');
            var kdaTd = $('<td class="' + x.kdaColor + '"><b>' + x.avgKDA + '</b></td>');
            var damageDealtTd = $('<td>' + x.avgDamageDealt + '</td>');
            var damageTakenTd = $('<td>' + x.avgDamageTaken + '</td>');
            var avgGoldSpentTd = $('<td>' + x.avgGoldSpent + '</td>');

            tableRow.append(playerTd);
            tableRow.append(winRatioTd);
            tableRow.append(kdaTd);
            tableRow.append(damageDealtTd);
            tableRow.append(damageTakenTd);
            tableRow.append(avgGoldSpentTd);
            tableBody.append(tableRow);
        }

        $("#leaderBoardsTable").trigger("update");
    });
}