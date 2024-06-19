const $turno = document.getElementById("turnoActual");
const $esperando = document.getElementById("esperando");
const $botonCancelar = document.getElementById("cancelar");

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

connection.on("TurnoAtendido", function (turno, cajas, idCajaExiste, turnoFuturo) {

    if (idCaja != idCajaExiste)
        return;

    const $turno = document.getElementById("turnoActual");
    $turno.style.color = "#295D99";

    if (turno.numero == 0) {
        $turno.textContent = "No hay turnos";
        $esperando.style.opacity = "1";
        $botonCancelar.style.display = "none";
    }
    else {
        $turno.textContent = turno.numero
        $botonCancelar.style.display = "inline-block";

    }
});




connection.on("TurnoGenerado", function (turno, cajas) {
    alert("Se conecto el cliente: " + turno.numero);
    $esperando.style.opacity = "0";
    $turno.textContent = turno.numero;
    $botonCancelar.style.display = "inline-block";


});
connection.on("TurnoCancelado", function (turnoCancelado, idcajaE) {

    if (idCaja != idcajaE)
        return;

    $turno.style.color = "#FF0000";
    $turno.textContent = `${turnoCancelado.numero} Cancelado`;
    $botonCancelar.style.display = "none";


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