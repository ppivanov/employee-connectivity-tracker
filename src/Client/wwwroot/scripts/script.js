window.setPageTitle = (titleIn) => {
    document.getElementById("page-title").innerHTML = " - " + titleIn;
}

const toggleUserDropdown = () => {
    let element = document.getElementById('dropdown')
    element.classList.toggle('display-none')
}

const openNav = () => {
    document.getElementById('mySidenav').style.width = '20em'
}

const closeNav = () => {
    document.getElementById('mySidenav').style.width = '0'
}


// Code adapted from https://developers.google.com/chart/interactive/docs/gallery/linechart
// Load the Visualization API and the corechart package.
google.charts.load('current', { 'packages': ['line'] });

// Set a callback to run when the Google Visualization API is loaded.
window.loadGraph = (mailList) => {
    google.charts.setOnLoadCallback(drawLineGraph(mailList));
}

// Callback that creates and populates a data table,
// instantiates the pie chart, passes in the data and
// draws it.
const drawLineGraph = (mailList) => {

    console.log(mailList);
    var data = google.visualization.arrayToDataTable(mailList);
    var options = {
        chart: {
            title: 'Number of sent and received emails'
        },
        height: 550
    };

    var chart = new google.charts.Line(document.getElementById('emails-chart'));

    chart.draw(data, google.charts.Line.convertOptions(options));
}

window.onresize = updateCharts;

function doALoadOfStuff() {
    //do a load of stuff
}