function PlayerTurn() {
    var firstThrow = -1;
    var secondThrow = -1;
    var thirdThrow = -1;
    var strike;
    var spare;

    this.getThrows = function () {
        return {FirstThrow: firstThrow, SecondThrow: secondThrow, ThirdThrow: thirdThrow};
    };

    this.setPoints = function (points) {
        if (firstThrow >= 0 && !strike && !spare) {// if we have no strike or spare, this is a second throw
            secondThrow = points;
            spare = ((firstThrow + secondThrow) == 10);
            return true;
        } else if (strike || spare) { //if we have a strike or spare, this is turn 10 and this is a bonus ball
            thirdThrow = points;
            return true;
        } else { // if we have no scores whatsoever, add them to the first throw
            firstThrow = points;
            strike = firstThrow == 10;
            return strike;
        }
    };

    this.isStrike = function () {
        return strike;
    };
    this.isSpare = function () {
        return spare;
    };
    this.getScore = function () {
        return this.isStrike() ? 10 : firstThrow + secondThrow;
    };
}