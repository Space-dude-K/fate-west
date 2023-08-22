var selectedServant = 0;

$(document).ready(function () {
    $("#myTable").tablesorter();
    $(".detailInfo").hide();

});

$(".servantData").click(function () {
    var servantTypeId = $(this).attr('data-hero');
    if (selectedServant !== servantTypeId) {
        $('tbody > tr').removeClass('detail-active');
        $("#servant_" + servantTypeId).addClass('detail-active');
        loadServantDetail(servantTypeId);
        $(".detailInfo").show();
        selectedServant = servantTypeId;
    } else {
        $('tbody > tr').removeClass('detail-active');
        $(".detailInfo").hide();
    }

});

function loadServantDetail(typeId) {
    $("#servantInfo").html("");
    $("#servantInfoDetail").html("<img src='/images/icons/Loading.gif' height=64 width=64 class='imageCenter'>");
    var image = $("#servant_" + typeId).attr("data-image");
    var title = $("#servant_" + typeId).attr("data-title");
    var name = $("#servant_" + typeId).attr("data-name");

    var infoDiv = $('<div></div>');

    var heroInfoLeft = $('<span></span>').addClass('media-left');
    heroInfoLeft.append("<img src='" + image + "' height=64 width=64>");
    var heroInfoBody = $('<div></div>').addClass('media-body');
    heroInfoBody.append("<h5 class='media-heading'><b>" + name + "</b></h5>");
    heroInfoBody.append("<h5 class='media-heading'>" + title + "</h5>");

    infoDiv.append(heroInfoLeft);
    infoDiv.append(heroInfoBody);
    $("#servantInfo").html(infoDiv);

    $.get("/ServantDetails/" + typeId,
        function (data) {
            $("#servantInfoDetail").html("");

            var panel = $("<ul id='tabMenu' data-tabs='tabs'></ul>").addClass('nav nav-tabs margin-top');
            $('#servantInfoDetail').append(panel);

            var tabContents = $("<div></div>").addClass('tab-content');

            data.forEach(function (instance) {
                var versionId = instance['mapVersion'].split('.').join('_');
                var colRow = $('<div></div>').addClass('row');
                $("#tabMenu").append("<li><a href='#tab_" + versionId + "' data-toggle='tab'>Version " + instance['mapVersion'] + "</a></li>");

                var tabBody = $("<div id='tab_" + versionId + "'></div>").addClass('tab-pane margin-top');

                var col1 = $('<div></div>').addClass('col-sm-4');

                col1.append("<label>Popularity</label>");
                var popularityTable = $('<table></table>').addClass('table table-bordered table-striped');
                var popularityTableBody = $('<tbody></tbody>');
                popularityTableBody.append("<tr><td>Games Played</td><td>" + instance['playCount'] + "</td></tr>");
                popularityTableBody.append("<tr><td>Pick Rate</td><td>" + instance['pickRate'] + "%</td></tr>");
                popularityTable.append(popularityTableBody);
                col1.append(popularityTable);

                var col2 = $('<div></div>').addClass('col-sm-4');

                col2.append("<label>Combat Average</label>");
                var combatTable = $('<table></table>').addClass('table table-bordered table-striped');
                var combatTableBody = $('<tbody></tbody>');
                combatTableBody.append("<tr><td>Damage Dealt</td><td>" + instance['avgDamageDealt'] + "</td></tr>");
                combatTableBody.append("<tr><td>Damage Taken</td><td>" + instance['avgDamageTaken'] + "</td></tr>");
                combatTableBody.append("<tr><td>Kills</td><td>" + instance['avgKills'] + "</td></tr>");
                combatTableBody.append("<tr><td>Deaths</td><td>" + instance['avgDeaths'] + "</td></tr>");
                combatTableBody.append("<tr><td>Assists</td><td>" + instance['avgAssists'] + "</td></tr>");
                combatTableBody.append("<tr><td>KDA</td><td>" + instance['avgKDA'] + "</td></tr>");
                combatTable.append(combatTableBody);
                col2.append(combatTable);

                var col3 = $('<div></div>').addClass('col-sm-4');
                col3.append("<label>Match Average</label>");
                var matchTable = $('<table></table>').addClass('table table-bordered table-striped');
                var matchTableBody = $('<tbody></tbody>');
                matchTableBody.append("<tr><td>Gold Spent</td><td>" + instance['avgGoldSpent'] + "</td></tr>");
                matchTableBody.append("<tr><td>Game Duration</td><td>" + instance['avgGameDuration'] + "</td></tr>");
                matchTableBody.append("<tr><td>Win Score Difference</td><td>" + instance['avgWinScoreDifference'] + "</td></tr>");
                matchTableBody.append("<tr><td>Loss Score Difference</td><td>" + instance['avgLossScoreDifference'] + "</td></tr>");
                matchTableBody.append("<tr><td>Win Rate</td><td>" + instance['winRate'] + "%</td></tr>");
                matchTable.append(matchTableBody);
                col3.append(matchTable);

                var tableRow = $('<div></div>').addClass('row');
                var table12Col = $('<div></div>').addClass('col-sm-12');
                table12Col.append("<label>Build Average</label>")
                var statTable = $("<table></table>").addClass("table table-bordered table-stats");
                var tableHeader1 = $("<thead></thead>");
                var tableHeaderRow1 = $('<tr></tr>');
                tableHeaderRow1
                    .append("<th class='centerText'><img height='32' width='32' src='/images/icons/stats/StatsStr.jpg'><div>Strength</div></th>");
                tableHeaderRow1
                    .append("<th class='centerText'><img height='32' width='32' src='/images/icons/stats/StatsAgi.jpg'><div>Agility</div></th>");
                tableHeaderRow1
                    .append("<th class='centerText'><img height='32' width='32' src='/images/icons/stats/StatsInt.jpg'><div>Intelligence</div></th>");
                tableHeaderRow1
                    .append("<th class='centerText'><img height='32' width='32' src='/images/icons/stats/StatsAtk.jpg'><div>Attack</div></th>");
                tableHeaderRow1
                    .append("<th class='centerText'><img height='32' width='32' src='/images/icons/stats/StatsArmor.jpg'><div>Armor</div></th>");

                tableHeader1.append(tableHeaderRow1);
                statTable.append(tableHeader1);

                var tableBody1 = $("<tbody></tbody>");
                var tableBodyRow1 = $("<tr></tr>");
                tableBodyRow1.append("<td class='centerText'>" + instance['avgStr'] + "</td>");
                tableBodyRow1.append("<td class='centerText'>" + instance['avgAgi'] + "</td>");
                tableBodyRow1.append("<td class='centerText'>" + instance['avgInt'] + "</td>");
                tableBodyRow1.append("<td class='centerText'>" + instance['avgAtk'] + "</td>");
                tableBodyRow1.append("<td class='centerText'>" + instance['avgArmor'] + "</td>");

                tableBody1.append(tableBodyRow1);
                statTable.append(tableBody1);

                var tableHeader2 = $('<thead></thead>');
                var tableHeaderRow2 = $('<tr></tr>');
                tableHeaderRow2
                    .append("<th class='centerText'><img height='32' width='32' src='/images/icons/stats/StatsHealthRegen.jpg'><div>Health Regen</div></th>");
                tableHeaderRow2
                    .append("<th class='centerText'><img height='32' width='32' src='/images/icons/stats/StatsManaRegen.jpg'><div>Mana Regen</div></th>");
                tableHeaderRow2
                    .append("<th class='centerText'><img height='32' width='32' src='/images/icons/stats/StatsMS.jpg'><div>Movespeed</div></th>");
                tableHeaderRow2
                    .append("<th class='centerText'><img height='32' width='32' src='/images/icons/GHGold.jpg'><div>Gold Regen</div></th>");
                tableHeaderRow2
                    .append("<th class='centerText'><img height='32' width='32' src='/images/icons/stats/StatsPrelati.jpg'><div>Prelati</div></th>");

                tableHeader2.append(tableHeaderRow2);
                statTable.append(tableHeader2);

                var tableBody2 = $("<tbody></tbody>");
                var tableBodyRow2 = $("<tr></tr>");
                tableBodyRow2.append("<td class='centerText'>" + instance['avgHealthRegen'] + "</td>");
                tableBodyRow2.append("<td class='centerText'>" + instance['avgManaRegen'] + "</td>");
                tableBodyRow2.append("<td class='centerText'>" + instance['avgMovespeed'] + "</td>");
                tableBodyRow2.append("<td class='centerText'>" + instance['avgGoldRegen'] + "</td>");
                tableBodyRow2.append("<td class='centerText'>" + instance['avgPrelati'] + "</td>");

                tableBody2.append(tableBodyRow2);
                statTable.append(tableBody2);

                colRow.append(col1);
                colRow.append(col2);
                colRow.append(col3);

                //statTable.append(tableHeader1);
                //statTable.append(tableBody1);
                //statTable.append(tableHeader2);
                //statTable.append(tableBody2);

                table12Col.append(statTable);
                tableRow.append(table12Col);

                tabBody.append(colRow);
                tabBody.append(tableRow);

                tabContents.append(tabBody);
            });
            $('#servantInfoDetail').append(tabContents);
            $("#tabMenu a").click(function (e) {
                e.preventDefault();
                $(this).tab('show');
            });
            $("#tabMenu >li:first-child").addClass("active");
            $(".tab-content > div:first-child").addClass("active");
        });
}