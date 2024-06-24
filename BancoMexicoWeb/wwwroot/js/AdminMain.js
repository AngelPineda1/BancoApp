const $turnosAtendidos = document.getElementById("turnosAtendidos");
const $turnosCancelados = document.getElementById("turnosCancelados");
const $turnosEspera = document.getElementById("turnosEspera");
const $promedioEspera = document.getElementById("promedioEspera");
const $tabla = document.querySelector("#tabla tbody");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://BancoMexicoAPI.websitos256.com/estadisticasHub", {
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




connection.on("OnConnected", function () {
    connection.invoke("Actualizar");
});




connection.on("ActualizarEstadisticas", function (estadisticas, cajas) {

    $promedioEspera.textContent = `${estadisticas.tiempoPromedio} minutos`;
    $turnosAtendidos.textContent = estadisticas.turnosAtendidos;
    $turnosCancelados.textContent = estadisticas.turnosCancelados;
    $turnosEspera.textContent = estadisticas.turnosPendientes;

    $tabla.innerHTML = "";
    for (let i = 0; i < cajas.length; i++) {

        var row = $tabla.insertRow();
        row.insertCell().textContent = cajas[i].nombre;
        row.insertCell().textContent = cajas[i].estado == 0 ? "Cerrada" : cajas[i].estado == 1? "Activa": "Ocupada";
        row.insertCell().textContent = cajas[i].numeroActual;
        row.insertCell().textContent = "";

    }


});


























connection.onclose(async () => {
    await start();
});

// Start the connection.
start();