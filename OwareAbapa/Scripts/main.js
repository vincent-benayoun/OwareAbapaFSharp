var globalGameState;
var previousGlobalGameStates = [];
var nextGlobalGameStates = [];

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
    
    $('#messageBox').html(gameState.winStatus);
    if (gameState.winStatus != "") {
        $('.boardCase button').attr("disabled", "disabled");
    } else {
        $('.boardCase button').removeAttr("disabled")
    }

    for (let i = 0; i <= 12; i++) {
        displayNumber($("#C" + i), gameState.board['c'+i]);
    }

    var colorBoardPlayer1, colorBoardPlayer2;
    [colorBoardPlayer1, colorBoardPlayer2] = (gameState.currentPlayer == "Player1") ? ["yellow", ""] : ["", "yellow"];
    $('#boardPlayer1').css("background-color", colorBoardPlayer1);
    $('#boardPlayer2').css("background-color", colorBoardPlayer2);
}

function displayWinStatus(winStatus) {
    $('#messageBox').html(winStatus);
}

function refreshGameState(newGameState) {
    if (globalGameState) {
        previousGlobalGameStates.push(globalGameState);
    }
    globalGameState = newGameState;
    nextGlobalGameStates = [];
    displayGameState(newGameState);
    updateNumberOfPreviousAndNextGameState();
}

function newGame() {
    $.getJSON("/api/game/newGame").done(refreshGameState);
}

function handlePlayResult(data) {
    if (data.status == "OK") {
        refreshGameState(data.gameState);
        displayWinStatus(data.winStatus);
    } else {
        alert(data.status);
    }
}

function play(caseId) {
    $.post("/api/game/play", { gameState: globalGameState, caseId: caseId }).done(handlePlayResult);
}

function clickOnBoardCase() {
    var caseId = $(this).attr("id");
    play(caseId);
}

function playAI() {
    $.post("/api/game/playAI", { gameState: globalGameState }).done(handlePlayResult);
}

function updateNumberOfPreviousAndNextGameState() {
    $('#numberPreviousGameState').html(previousGlobalGameStates.length);
    $('#numberNextGameState').html(nextGlobalGameStates.length);
}

function undo() {
    if (previousGlobalGameStates.length > 0) {
        nextGlobalGameStates.push(globalGameState);
        globalGameState = previousGlobalGameStates.pop();
        displayGameState(globalGameState);
        updateNumberOfPreviousAndNextGameState();
    }
}

function redo() {
    if (nextGlobalGameStates.length > 0) {
        previousGlobalGameStates.push(globalGameState);
        globalGameState = nextGlobalGameStates.pop();
        displayGameState(globalGameState);
        updateNumberOfPreviousAndNextGameState();
    }
}

$(function () {
    newGame();
    //$('.boardCase').click(function () { console.log($(this).attr("id")) });

    $('.boardCase button').click(clickOnBoardCase);
    $('#buttonPlayAI').click(playAI);

    $('#buttonUndo').click(undo);
    $('#buttonRedo').click(redo);
});
