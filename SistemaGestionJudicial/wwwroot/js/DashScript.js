const ctx = document.getElementById('crimeChart').getContext('2d');
const crimeChart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: [], // se llenan dinámicamente
        datasets: [{
            label: 'Crimes',
            data: [],
            backgroundColor: 'rgba(59, 130, 246, 0.5)',
            borderColor: 'rgba(59, 130, 246, 1)',
            borderWidth: 1
        }]
    },
    options: {
        responsive: true,
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});

const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

async function loadAndUpdateDashboard(days = 365) {
    try {
        const response = await fetch(`/Dashboard/GetData?days=${days}`);
        const data = await response.json();

        // Actualiza el gráfico
        const labels = data.crimeStatsFilter.map(item => months[item.month - 1]);
        const values = data.crimeStatsFilter.map(item => item.count);

        crimeChart.data.labels = labels;
        crimeChart.data.datasets[0].data = values;
        crimeChart.update();
    } catch (error) {
        console.error("Error loading dashboard data:", error);
    }
}

// Al cargar la página
document.addEventListener("DOMContentLoaded", () => loadAndUpdateDashboard());

// Al cambiar el filtro
document.getElementById('filterRange').addEventListener('change', function () {
    const days = this.value;
    loadAndUpdateDashboard(days);
});