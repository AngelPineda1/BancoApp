
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://BancoMexicoAPI.websitos256.com/turnosHub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
    })
    .build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.on("TurnoAtendido", function (turno, cajas, idCajaExiste) {

    if (idCaja != idCajaExiste)
        return;

    const $turno = document.getElementById("turnoActual");

    if (turno.numero == 0)
        $turno.textContent = "No hay turnos";
    else
        $turno.textContent = turno.numero;
});




connection.on("TurnoGenerado", function (turno, cajas) {
    alert("Turno generado: " + turno.numero);
    const $turno = document.getElementById("turnoActual");
    $turno.textContent = turno.numero;
});


document.getElementById("atendiendo").addEventListener("click", function (event) {
    connection.invoke("AtenderCliente", idCaja);
});


connection.onclose(async () => {
    await start();
});

// Start the connection.
start();