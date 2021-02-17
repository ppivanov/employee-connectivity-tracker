﻿window.setPageTitle = (titleIn) => {
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

const eventsChartDivId = "events-chart";
const emailsChartDivId = "emails-chart";


// Code adapted from https://developers.google.com/chart/interactive/docs/gallery/
// Load the Visualization API and the corechart package.
google.charts.load('current', { 'packages': ['line', 'bar', 'corechart'] });

// Set a callback to run when the Google Visualization API is loaded.
window.loadDashboardGraph = (mailList, calendarList) => {                                // TODO - save these in a global letiable to allow for chart resizing (they need to be redrawn)
    google.charts.setOnLoadCallback(drawLineGraph(mailList));
    if (calendarList.length > 1) {
        google.charts.setOnLoadCallback(drawBarChart(calendarList));
    } else {
        document.getElementById(eventsChartDivId).innerHTML = "There are no events in the selected range";
    }
}

window.loadMyTeamDashboardGraph = (mailList, calendarList) => {
    google.charts.setOnLoadCallback(drawLineGraphCustomTooltip(mailList));
    if (calendarList.length > 1) {
        google.charts.setOnLoadCallback(drawBarChart(calendarList));
    } else {
        document.getElementById(eventsChartDivId).innerHTML = "There are no events in the selected range";
    }

}

// Callback that creates and populates a data table,
// instantiates the pie chart, passes in the data and
// draws it.
const drawBarChart = (calendarList) => {
    let dataTable = new google.visualization.DataTable();
    dataTable.addColumn('string', 'Event subject');
    dataTable.addColumn('number', 'Number of events');

    dataTable.addRows(calendarList)

    let options = {
        chart: {
            title: 'Meetings in period'
        },
        bars: 'horizontal',
        legend: {
            position: 'none'
        },
        height: 750
    };

    let chart = new google.charts.Bar(document.getElementById(eventsChartDivId));
    chart.draw(dataTable, google.charts.Bar.convertOptions(options));
}

const drawLineGraph = (mailList) => {

    let data = google.visualization.arrayToDataTable(mailList);
    let options = {
        chart: {
            title: 'Number of sent and received emails'
        },
        height: 550
    };

    let chart = new google.charts.Line(document.getElementById(emailsChartDivId));
    chart.draw(data, google.charts.Line.convertOptions(options));
}

const drawLineGraphCustomTooltip = (mailList) => {
    let dataTable = new google.visualization.DataTable();
    dataTable.addColumn('string', 'Date');
    dataTable.addColumn('number', 'Sent emails');
    dataTable.addColumn({ type: 'string', role: 'tooltip' });
    dataTable.addColumn('number', 'Received emails');
    // A column for custom tooltip content
    dataTable.addColumn({ type: 'string', role: 'tooltip' });
    dataTable.addRows(mailList);

    let options = {
        chart: {
            title: 'Emails received by team members'
        },
        height: 550 };
    let chart = new google.visualization.LineChart(document.getElementById(emailsChartDivId));
    chart.draw(dataTable, options);
}