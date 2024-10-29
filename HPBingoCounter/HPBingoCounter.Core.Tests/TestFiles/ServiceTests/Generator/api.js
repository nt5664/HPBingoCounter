function getCards(inVersion, inCardType, inSeed) {
    const opts = {
        seed: inSeed,
        mode: inCardType,
        lang: 'name',
        version: inVersion
    };

    const bingoFunc = bingoGenerator;

    let cards = bingoFunc(bingoList, opts).map(x => { return { name: x.name, amount: x.amount } });
    return JSON.stringify(cards);
}