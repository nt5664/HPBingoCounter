bingoGenerator = function(list, opts) {
    let goals = [];

    for (var i = 0; i < 5; ++i) {
        goals[i] = {
            name: "test" + i,
            amount: i + 1,
            id: "t" + i
        };
    }

    return goals;
}

module.exports = bingoGenerator;