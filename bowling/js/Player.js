function Player(n) {

    var name = n;
    var score = 0;
    var throws = []; // PlayerTurn()

    this.addScore = function (playerTurn) {
        addThrows(playerTurn.getThrows());
        score = this.recalculateScore();
        return score;
    };

    var addThrows = function (thisTurnThrows) {
        //Add the latest throws to the array
        throws[throws.length] = thisTurnThrows.FirstThrow;
        if (thisTurnThrows.SecondThrow >= 0)throws[throws.length] = thisTurnThrows.SecondThrow;
        if (thisTurnThrows.ThirdThrow >= 0)throws[throws.length] = thisTurnThrows.ThirdThrow;
    };

    this.recalculateScore = function () {
        var calculatedScore = 0;
        for (var throwIndex = 0; throwIndex < throws.length; throwIndex++) {
            var score = throws[throwIndex];
            var secondScore = throwIndex + 1 < throws.length ? throws[throwIndex + 1] : 0;
            var thirdScore = throwIndex + 2 < throws.length ? throws[throwIndex + 2] : 0;
            if (score == 10) { //If this throw is 10, we have a strike, and we'll add the next two balls to the score
                calculatedScore += score + secondScore + thirdScore;
            } else if (score + secondScore == 10) { // if the next two balls are 10 together, we have a spare. Add the third ball as bonus
                calculatedScore += score + secondScore + thirdScore;
                throwIndex++; // We have 2 balls in one turn, we will skip the next ball in the loop.
            } else { // If we don't have a strike or spare
                if (throwIndex + 1 >= throws.length && secondScore == 0) break; //If this is a bonus ball, skip.
                calculatedScore += score + secondScore; // Add the balls of this turn, to the score
                throwIndex++; // We have used 2 balls for 1 turn, skip the next one in the loop
            }
        }
        return calculatedScore;
    };

    this.getPlayerName = function () {
        return name;
    };

    this.getScore = function () {
        return score;
    };

}