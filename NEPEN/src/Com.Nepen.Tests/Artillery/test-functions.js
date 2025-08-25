const medidorIds = [
    "MED001","MED002","MED003","MED004","MED005",
    "MED006","MED007","MED008","MED009","MED010"
];

function generateMedidorId(context, events, done) {
    const id = medidorIds[Math.floor(Math.random() * medidorIds.length)];
    context.vars.medidorId = id;
    return done();
}

function generateDataInicio(context, events, done) {
    const now = new Date();
    const past = new Date();
    past.setDate(now.getDate() - 30);
    const randomDate = new Date(past.getTime() + Math.random() * (now.getTime() - past.getTime()));
    context.vars.dataInicio = randomDate.toISOString();
    return done();
}

function generateDataFim(context, events, done) {
    context.vars.dataFim = new Date().toISOString();
    return done();
}

function generateTimestamp(context, events, done) {
    context.vars.timestamp = new Date().toISOString();
    return done();
}

function generateFutureTimestamp(context, events, done) {
    const future = new Date();
    future.setDate(future.getDate() + 30);
    context.vars.futureTimestamp = future.toISOString();
    return done();
}

module.exports = {
    generateMedidorId,
    generateDataInicio,
    generateDataFim,
    generateTimestamp,
    generateFutureTimestamp
};
