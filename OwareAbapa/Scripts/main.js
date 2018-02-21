var globalGameState;

function animateElement(caseElement, direction) {
    var intermediateColor = direction ? "green" : "red";
    caseElement.animate({ backgroundColor: intermediateColor }, 600).animate({ backgroundColor: "grey" }, 600);
}

function displayNumber(element, newValue) {
    let previousValue = element.html();
    element.html(newValue);
    if (newValue != previousValue) {
        animateElement(element, newValue > previousValue);
    }
}

function displayGameState(gameState) {
    $('#currentPlayer').html(gameState.currentPlayer);
    displayNumber($('#scorePlayer1'), gameState.score.player1);
    displayNumber($('#scorePlayer2'), gameState.score.player2);

    for (let i = 0; i <= 12; i++) {
        displayNumber($("#C" + i), gameState.board['c'+i]);
    }

    var colorBoardPlayer1, colorBoardPlayer2;
    [colorBoardPlayer1, colorBoardPlayer2] = (gameState.currentPlayer == "Player1") ? ["yellow", ""] : ["", "yellow"];
    $('#boardPlayer1').css("background-color", colorBoardPlayer1);
    $('#boardPlayer2').css("background-color", colorBoardPlayer2);
}

function refreshGameState(newGameState) {
    globalGameState = newGameState;
    displayGameState(globalGameState);
}

function newGame() {
    $.getJSON("/api/game/newGame").done(refreshGameState);
}

function play(caseId) {
    function handler(data) {
        if (data.status == "OK") {
            refreshGameState(data.gameState);
        } else {
            alert(data.status)
        }
    }

    $.post("/api/game/play", { gameState: globalGameState, caseId: caseId }).done(handler);
}

function clickOnBoardCase() {
    var caseId = $(this).attr("id");
    play(caseId);
}

$(function () {
    newGame();
    //$('.boardCase').click(function () { console.log($(this).attr("id")) });

    $('.boardCase button').click(clickOnBoardCase);
});
