
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

connection.on("TurnoAtendido", function (turno, cajas, idCajaExiste,turnoFuturo) {

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
connection.on("TurnoCancelado", function (turnoCancelado, idcaja) {



    //await Clients.All.SendAsync("TurnoCancelado", turnoCancelar, idcaja, turnoDto);
});


document.getElementById("atendiendo").addEventListener("click", function (event) {
    connection.invoke("AtenderCliente", idCaja);
});

document.getElementById("cancelar").addEventListener("click", function (event) {
    const $turno = document.getElementById("turnoActual");
    connection.invoke("CancelarTurno", parseInt($turno.textContent), idCaja);
});


connection.onclose(async () => {
    await start();
});

// Start the connection.
start();