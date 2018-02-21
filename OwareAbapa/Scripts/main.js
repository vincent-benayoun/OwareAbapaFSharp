var globalGameState;

function displayGameState(gameState) {
    $('#currentPlayer').html(gameState.currentPlayer);
    $('#scorePlayer1').html(gameState.score.player1);
    $('#scorePlayer2').html(gameState.score.player2);

    $('#C1').html(gameState.board.c1);
    $('#C2').html(gameState.board.c2);
    $('#C3').html(gameState.board.c3);
    $('#C4').html(gameState.board.c4);
    $('#C5').html(gameState.board.c5);
    $('#C6').html(gameState.board.c6);
    $('#C7').html(gameState.board.c7);
    $('#C8').html(gameState.board.c8);
    $('#C9').html(gameState.board.c9);
    $('#C10').html(gameState.board.c10);
    $('#C11').html(gameState.board.c11);
    $('#C12').html(gameState.board.c12);
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

    $('.boardCase').click(clickOnBoardCase);
});
