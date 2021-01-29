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

const personalEventDivId = "events-chart";
const personalEmailsDivId = "emails-chart";

const myTeamEventDivId = "my-team-events-chart";
const myTeamEmailsDivId = "my-team-emails-chart";

// Code adapted from https://developers.google.com/chart/interactive/docs/gallery/
// Load the Visualization API and the corechart package.
google.charts.load('current', { 'packages': ['line', 'bar', 'corechart'] });

// Set a callback to run when the Google Visualization API is loaded.
window.loadDashboardGraph = (mailList, calendarList) => {                                // TODO - save these in a global variable to allow for chart resizing (they need to be redrawn)
    google.charts.setOnLoadCallback(drawLineGraph(mailList));
    if (calendarList.length > 1) {
        google.charts.setOnLoadCallback(drawBarChart(calendarList));
    } else {
        document.getElementById(personalEventDivId).innerHTML = "There are no events in the selected range";
    }
}

window.loadMyTeamDashboardGraph = (mailList, calendarList) => {
    google.charts.setOnLoadCallback(drawLineGraphManually(mailList));
}

// Callback that creates and populates a data table,
// instantiates the pie chart, passes in the data and
// draws it.
const drawBarChart = (calendarList) => {

    var data = google.visualization.arrayToDataTable(calendarList);
    var options = {
        chart: {
            title: 'Meetings in period'
        },
        bars: 'horizontal',
        legend: {
            position: 'none'
        },
        height: 750
    };

    var chart = new google.charts.Bar(document.getElementById(personalEventDivId));
    chart.draw(data, google.charts.Bar.convertOptions(options));
}

const drawLineGraph = (mailList) => {

    var data = google.visualization.arrayToDataTable(mailList);
    var options = {
        chart: {
            title: 'Number of sent and received emails'
        },
        height: 550
    };

    var chart = new google.charts.Line(document.getElementById(personalEmailsDivId));
    chart.draw(data, google.charts.Line.convertOptions(options));
}

const drawLineGraphManually = (mailList) => {
    var dataTable = new google.visualization.DataTable();
    dataTable.addColumn('string', 'Year');
    dataTable.addColumn('number', 'Sales');
    // A column for custom tooltip content
    dataTable.addColumn({ type: 'string', role: 'tooltip' });
    dataTable.addRows([
        ['2010', 600, 'John: 14\nSam: 2\nPatricia: 35'],
        ['2011', 1500, 'Sunspot activity made this our best year ever!'],
        ['2012', 800, '$800K in 2012.'],
        ['2013', 1000, '$1M in sales last year.']
    ]);

    var options = {
        chart: {
            title: 'Emails received by team members'
        },
        height: 550 };
    var chart = new google.visualization.LineChart(document.getElementById(myTeamEmailsDivId));
    chart.draw(dataTable, options);
}