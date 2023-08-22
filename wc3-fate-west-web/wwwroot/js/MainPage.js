$(document).ready(function () {
    $('body').on('hidden.bs.modal', '.modal', function () {
        $(this).find(".modal-content").html('<img src="images/icons/Loading.gif" height=64 width=64 class="imageCenter">');
        $(this).removeData('bs.modal');
    });
    $(".noExpand").click(function (event) {
        event.stopPropagation();
    });
    $('#collapsibleTable').on('shown.bs.collapse', function (e) {
        var divDom = $(e.target);
        var isDataLoaded = divDom.attr("data-isLoaded");
        console.log(isDataLoaded);
        if (isDataLoaded === "1")
            return;
        var gameId = $(e.target).attr("data-gameId");
        
        $.get("gameDetail/" + gameId, function (data) {
            divDom.attr("data-isLoaded", 1);
            divDom.empty();

            var table = $('<table></table>').addClass('table table-condensed innerTable');
            for (var i = 1; i < 3; i++) {
                var tableBody = $('<tbody></tbody>');
                var tableHeader = $('<thead></thead>');
                var headerRow = $("<tr class=team" + i + "Bar></tr>");

                headerRow.append('<th>Team ' + i + " [Score: " + data["team" + i + "WinCount"] + "]</th>");
                headerRow.append('<th class=centerText>Level</th>');
                headerRow.append('<th class=centerText>Kills</th>');
                headerRow.append('<th class=centerText>Deaths</th>');
                headerRow.append('<th class=centerText>Assists</th>');
                headerRow.append('<th class=centerText>Damage Dealt</th>');
                headerRow.append('<th class=centerText>Gods Help</th>');
                headerRow.append('<th class=centerText>Gold Spent</th>');
                headerRow.append('<th class=centerText>Detail</th>');
                tableHeader.append(headerRow);
                table.append(tableHeader);
                table.append(tableBody);
                divDom.append(table);

                console.log(gameId);

                $.each(data["team" + i + "Data"], function (idx, elem) {
                    var playerRow = $('<tr></tr>');
                    // playerRow.append("<td class=leftText><img class=heroicon height=32 width=32 src='" + elem.heroImageURL + "'><b>" + '<a href="/PlayerStats/test/' + elem.playerName + "/" + gameId + "/NONE" + '">' + elem.playerName + "</b></td>");
                    playerRow.append("<td class=leftText><img class=heroicon height=32 width=32 src='" + elem.heroImageURL + "'><b>" + '<a href="/PlayerStats/test/' + elem.playerName + '">' + elem.playerName + "</b></td>");
                    playerRow.append("<td class=centerText>" + elem.level + "</td>");
                    playerRow.append("<td class=centerText>" + elem.kills + "</td>");
                    playerRow.append("<td class=centerText>" + elem.deaths + "</td>");
                    playerRow.append("<td class=centerText>" + elem.assists + "</td>");
                    playerRow.append("<td class=centerText>" + numberUtil.formatDecimal(elem.damageDealt) + "</td>");
                    var godsHelpCol = $('<td class=centerText></td>');
                    $.each(elem.godsHelpImageURLList, function (i, url) {
                        godsHelpCol.append("<img class=heroicon height=32 width=32 src='" + url + "'>");
                    });
                    playerRow.append(godsHelpCol);
                    playerRow.append("<td class=centerText>" + numberUtil.formatNumber(elem.goldSpent) + "</td>");
                    playerRow.append('<td class=centerText><a href=/PlayerGameBuildDetail/' + gameId + '/' + elem.playerName + ' data-target="#playerGameDetail" data-toggle="modal"><img src="/images/icons/PlayerGameDetail.png" width=24 height=24 alt="View player game detail"></a></td>');
                    tableBody.append(playerRow);
                });
                var teamTotalRow = $("<tr></tr>");
                teamTotalRow.append("<td class='team" + i + "total centerText'><b>Total</b></td>");
                teamTotalRow.append("<td class='team" + i + "total centerText'></td>");
                teamTotalRow.append("<td class='team" + i + "total centerText'><b>" + data["team" + i + "Kills"] + " </b></td>");
                teamTotalRow.append("<td class='team" + i + "total centerText'><b>" + data["team" + i + "Deaths"] + " </b></td>");
                teamTotalRow.append("<td class='team" + i + "total centerText'><b>" + data["team" + i + "Assists"] + " </b></td>");
                teamTotalRow.append("<td class='team" + i + "total centerText'><b>" + numberUtil.formatDecimal(data["team" + i + "DamageDealt"]) + " </b></td>");
                teamTotalRow.append("<td class='team" + i + "total centerText'></td>");
                teamTotalRow.append("<td class='team" + i + "total centerText'><b>" + numberUtil.formatNumber(data["team" + i + "Gold"]) + " </b></td>");
                teamTotalRow.append("<td class='team" + i + "total centerText'></td>");
                tableBody.append(teamTotalRow);
            }
        });
    });
});