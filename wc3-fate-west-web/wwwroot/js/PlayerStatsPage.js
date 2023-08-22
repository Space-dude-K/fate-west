function updateGameList() {
    var showMoreDiv = $("#showMoreBox");
    var moreResult = showMoreDiv.attr("data-result");
    if (moreResult === "false") {
        return;
    }
    var userName = showMoreDiv.attr("data-username");
    var server = showMoreDiv.attr("data-server");
    var lastGameId = showMoreDiv.attr("data-lastgameid");
    var servantFilter = $('#collapsibleTable select.selectized,input.selectized').val();
    if (servantFilter === "") {
        servantFilter = "NONE";
    }
    console.log(servantFilter);
    showMoreDiv.html('<img src="/images/icons/Loading.gif" height=48 width=48 class="imageCenter">');
    $.get("/PlayerStatsAjax/" + server + "/" + userName + "/" + lastGameId + "/" + servantFilter, function (data) {
        if (data.length === 0) {
            showMoreDiv.attr("data-result", "false");
            showMoreDiv.html("There are no more results");
            return;
        }
        showMoreDiv.html("Show More");
        $.each(data, function (idx, elem) {
            var statsBox = $('<div class="statsBox gameFullList"></div>');
            var gameStatsBox = $('<div class="gameStatsBox hoverDiv" data-toggle="collapse" data-target="#collapsible' + elem.gameID + '"></div>');
            var gameResultInfo = $('<div class="gameTableCell centerText gameResultInfo">');
            gameResultInfo.append('<div class="' + elem.gameResult + ' GameResult">' + elem.gameResult + '(' + elem.teamOneWinCount + ' - ' + elem.teamTwoWinCount + ')</div>');
            gameResultInfo.append('<div>' + elem.playedDate + '</div>');
            gameStatsBox.append(gameResultInfo);

            var gameHeroPic = $('<div class="gameTableCell gameHeroPic centerText">');
            gameHeroPic.append('<img height="48" width="48" src="' + elem.heroImageURL + '">');
            gameStatsBox.append(gameHeroPic);

            console.log(elem.heroImageURL);

            var gameKDA = $('<div class="gameTableCell gameKDA centerText">');
            gameKDA.append('<div class="' + elem.heroKDAColor + '"><b>' + elem.heroKDA + ' : 1 KDA</b></div>');
            gameKDA.append('<div><b>' + elem.heroKills + ' / <span class="LOSS">' + elem.heroDeaths + '</span> / ' + elem.heroAssists + '</b></div>');
            gameStatsBox.append(gameKDA);

            var gameLevelGold = $('<div class="gameTableCell gameLevelGold centerText">');
            gameLevelGold.append('<div>Level ' + elem.heroLevel + '</div>');
            gameLevelGold.append('<div>' + elem.goldSpent + ' Gold </div>');
            gameLevelGold.append('<div>' + elem.damageDealt + ' Damage</div>');
            gameStatsBox.append(gameLevelGold);

            var gameTeamInfo1 = $('<div class="gameTableCell centerText gameTeamInfo">');
            var gameTeam1 = $('<div class="team">');
            var gamePlayer1 = $('<div class="player">');
            $.each(elem.team1List, function (i, e) {
                var divPlayerInfo = $('<div class="playerInfo">');
                divPlayerInfo.append('<img height="16" width="16" src="' + e.heroImageURL + '" style="margin-right: 3px">' + e.playerName);
                gamePlayer1.append(divPlayerInfo);
            });
            gameTeam1.append(gamePlayer1);
            gameTeamInfo1.append(gameTeam1);
            gameStatsBox.append(gameTeamInfo1);

            var gameTeamInfo2 = $('<div class="gameTableCell centerText gameTeamInfo">');
            var gameTeam2 = $('<div class="team">');
            var gamePlayer2 = $('<div class="player">');
            $.each(elem.team2List, function (i, e) {
                var divPlayerInfo = $('<div class="playerInfo">');
                divPlayerInfo.append('<img height="16" width="16" src="' + e.heroImageURL + '" style="margin-right: 3px">' + e.playerName);
                gamePlayer2.append(divPlayerInfo);
            });
            gameTeam2.append(gamePlayer2);
            gameTeamInfo2.append(gameTeam2);
            gameStatsBox.append(gameTeamInfo2);

            var gameReplayInfo = $('<div class="gameTableCell gameDownloadReplay">');
            var gameReplayInfoHref = $('<a href="/download/replay/' + elem.gameID + '" class="noExpand">');
            var gameReplayInfoImg = $('<img class="gameDownloadReplayImg" src="/images/icons/SaveReplay.png" width="32" height="32" alt="Download Replay">');

            gameReplayInfoHref.append(gameReplayInfoImg);
            gameReplayInfo.append(gameReplayInfoHref);
            gameStatsBox.append(gameReplayInfo);

            statsBox.append(gameStatsBox);

            var collapsibleGame = $('<div class="accordian-body collapse" data-gameId="' + elem.gameID + '" id="collapsible' + elem.gameID + '">');
            collapsibleGame.append('<img src="/images/icons/Loading.gif" height=64 width=64 class="imageCenter">');
            statsBox.append(collapsibleGame);

            showMoreDiv.before(statsBox);
            lastGameId = elem.gameID;
        });
        showMoreDiv.attr("data-lastgameid", lastGameId);
    });
}

$(document).ready(function () {
    $("#showMoreBox").click(function () {
        updateGameList();
    });

    $('#collapsibleTable select.selectized,input.selectized').on('change', function () {
        $(".gameFullList").remove();
        var showMoreDiv = $("#showMoreBox");
        showMoreDiv.attr("data-lastgameid", "9999999");
        showMoreDiv.attr("data-result", "true");
        //showMoreDiv.html('<img src="/images/icons/Loading.gif" height=48 width=48 class="imageCenter">');
        updateGameList();
    });

    $('#select-servant-search').selectize({
        create: true,
        sortField:
        {
            field: 'text',
            direction: 'asc'
        },
        dropdownParent: 'body'
    });
});