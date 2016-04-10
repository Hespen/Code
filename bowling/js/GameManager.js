function GameManager(playerNames) {
    var players = new Array(playerNames.length);
    var currentPlayer = 0;
    var currentTurn = 0;
    var currentThrow = new PlayerTurn();

    var initialize = function () {
        //Setup the score table and create the Player objects
        for (var player = 0; player < players.length; player++) {
            players[player] = new Player(playerNames[player]);
            $('#scoreTable').append("<tr class=\"player" + player + "\"><td>" + playerNames[player] + "</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td class=\"total\"></td></tr>");
        }
    };

    //Add the score to the Total score, and display
    var addScore = function (currentThrow) {
        $('.total')[currentPlayer].innerHTML = players[currentPlayer].addScore(currentThrow);
    };

    var setScoreText = function (points) {
        var scoreString = "";
        var textContainer = $('.player' + currentPlayer).children()[currentTurn + 1];
        if (points.isStrike()) {
            scoreString = "X";
        } else if (points.isSpare()) {
            scoreString = points.getThrows().FirstThrow + " /";
        } else {
            scoreString = points.getThrows().FirstThrow + " ";
            scoreString += points.getThrows().SecondThrow == -1 ? "" : points.getThrows().SecondThrow;
        }
        if (points.getThrows().ThirdThrow >= 0) {
            var thirdPoints = points.getThrows().ThirdThrow;
            if (thirdPoints == 10) {
                scoreString += " X"
            } else {
                scoreString += " " + thirdPoints;
            }
        }
        textContainer.innerHTML = scoreString + "<br>" + players[currentPlayer].recalculateScore();
    };

    this.nextThrow = function (score) {
        if (currentThrow.setPoints(score)) {  //If we have a strike or 2 thrown balls
            //Unlock all buttons
            this.lockButtons(0);

            //If we are in the last turn, and we have a strike or spare. End the method, and throw again.
            if (((currentTurn == 9 && currentThrow.getThrows().ThirdThrow == -1) && (currentThrow.isStrike() || currentThrow.isSpare())) == true) {
                setScoreText(currentThrow);
                return;
            }

            //Update scoreboard values and go to the next turn.
            addScore(currentThrow);
            setScoreText(currentThrow);
            nextTurn();
        } else { // If one ball is thrown, and no strike
            setScoreText(currentThrow);
            this.lockButtons(score);
        }
    };

    var showWinner = function (bestPlayer) {
        //With more than one player in the array, we have a tie.
        if (bestPlayer.length > 1) {
            var text = "We have a tie between ";
            for (var player = 0; player < bestPlayer.length; player++) {
                text += bestPlayer[player].getPlayerName();
                text += player + 1 < bestPlayer.length ? " and " : "";
            }
            alert(text);
        } else {
            alert("The winner is " + bestPlayer[0].getPlayerName());
        }
        location.reload();
    };

    var endGame = function () {
        var bestPlayer = [players[0]];
        for (var player = 1; player < players.length; player++) {
            if (players[player].getScore() > bestPlayer[0].getScore()) {
                //Reset to a single item in the array
                bestPlayer = [players[player]];
            } else if (players[player].getScore() == bestPlayer[0].getScore()) {
                //Add player to the array if there is a tie
                bestPlayer[bestPlayer.length] = players[player];
            }
        }
        showWinner(bestPlayer);
    };

    var nextTurn = function () {
        currentThrow = new PlayerTurn;
        if (currentPlayer < (players.length - 1)) { //If there is another player
            currentPlayer++;
        } else { //reset to player 1
            if (currentTurn >= 9) {
                endGame(); //end the game if we are above turn 10 == turn 9 in [0-9]
                return;
            }
            currentPlayer = 0;
            currentTurn++;
        }
    };

    //Lock the buttons, to prevent more than 10 points for a single round.
    this.lockButtons = function (number) {
        $('.throw').each(function (index) {
            $(this).prop("disabled", index > (10 - number));
        });
    };

    return initialize();
}