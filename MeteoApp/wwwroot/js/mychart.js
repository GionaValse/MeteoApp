window.tempChart = null;

window.createChart = (canvasId, temperatures, labels) => {
    try{
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        if (window.tempChart){
            window.tempChart.destroy();
        }

        window.tempChart = new Chart(canvas, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Temperatura',
                    data: temperatures,
                    borderWidth: 2,
                    tension: 0.35,
                    fill: false
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false
            }
        });
    }catch (error){
        document.getElementById("stupid-error").innerHTML = "Error while creating chart";
    }
};