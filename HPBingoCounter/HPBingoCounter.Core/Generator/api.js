function getCards(inVersion, inCardType, inSeed) {
    const opts = {
        seed: inSeed,
        mode: inCardType,
        lang: 'name',
        version: inVersion
    };

    const bingoFunc = bingoGenerator;

    let cards = bingoFunc(bingoList, opts).map(x => {
        return {
            id: x.id,
            name: x.name,
            amount: x.amount,
            uniqueAmount: x.uniqueAmount || x.amount,
            triggers: x.triggers
        };
    });
    return JSON.stringify(cards);
}