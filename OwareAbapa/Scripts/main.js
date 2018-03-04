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
        $('#buttonPlayAI').attr("disabled", "disabled");
    } else {
        $('.boardCase button').removeAttr("disabled");
        $('#buttonPlayAI').removeAttr("disabled");
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

function playNextMoveIfAI() {
    if (globalGameState.winStatus) {
        return;
    }
    let selectedStrategy;
    switch (globalGameState.currentPlayer) {
        case "Player1":
            selectedStrategy = $("#selectAIPlayer1").val();
            break;
        case "Player2":
            selectedStrategy = $("#selectAIPlayer2").val();
            break;
        default:
            selectedStrategy = "none";
    }
    if (selectedStrategy != "none") {
        playAI(selectedStrategy);
    }
}

function handlePlayResult(data) {
    if (data.status == "OK") {
        refreshGameState(data.gameState);
        displayWinStatus(data.winStatus);
        setTimeout(() => { playNextMoveIfAI() }, 1000);
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

function playAI(strategy) {
    if (!strategy) {
        return;
    }
    $.post("/api/game/playAI", { gameState: globalGameState, strategy }).done(handlePlayResult);
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

function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}

function displayAIStrategies(aiStrategies) {
    let selectAIPlayer1 = $('#selectAIPlayer1');
    let selectAIPlayer2 = $('#selectAIPlayer2');

    let makeOption = function(name) {
        return "<option value=\"" + name + "\">" + capitalizeFirstLetter(name) + "</option>";
    }

    selectAIPlayer1.append(makeOption("none"));
    selectAIPlayer2.append(makeOption("none"));
    for (let strategyName in aiStrategies) {
        let strategyOption = makeOption(strategyName);
        selectAIPlayer1.append(strategyOption);
        selectAIPlayer2.append(strategyOption);
    }
}

function loadAIStrategies() {
    $.getJSON("/api/game/getAIStrategies").done(displayAIStrategies);
}

$(function () {
    newGame();
    //$('.boardCase').click(function () { console.log($(this).attr("id")) });

    $('.boardCase button').click(clickOnBoardCase);
    $('#selectAIPlayer1').change(playNextMoveIfAI);
    $('#selectAIPlayer2').change(playNextMoveIfAI);

    $('#buttonUndo').click(undo);
    $('#buttonRedo').click(redo);

    loadAIStrategies();
});
