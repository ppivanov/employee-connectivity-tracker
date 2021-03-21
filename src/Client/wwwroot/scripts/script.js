window.setPageTitle = (titleIn) => {
	document.getElementById('page-title').innerHTML = ' - ' + titleIn
}
window.setUserToNotifyEmail = (name) => {
	let inputField = document.getElementById('userToNotify_email')
	inputField.value = name
	inputField.disabled = true
}
window.setUserToNotifyName = (email) => {
	let inputField = document.getElementById('userToNotify_name')
	inputField.value = email
	inputField.disabled = true
}

window.resetUserToNotifyEmail = () => {
	resetInputField('userToNotify_email')
}
window.resetUserToNotifyName = () => {
	resetInputField('userToNotify_name')
}

window.setCreateTeamLeader = (nameAndEmail) => {
	let inputField = document.getElementById('createTeamLeader')
	inputField.value = nameAndEmail
}

window.resetCreateTeamLeader = () => {
	resetInputField('createTeamLeader')
}

const resetInputField = (elementId) => {
	let inputField = document.getElementById(elementId)
	inputField.value = ''
	inputField.disabled = false
}

const toggleUserDropdown = () => {
	if (isUserDropdownOpen()) {
		closeUserDropdown()
	} else {
		openUserDropdown()
	}
}
const openUserDropdown = () => {
	document.getElementById('dropdown').style.display = 'block'
	window.addEventListener(
		'click',
		(e) => {
			if (e.target.id != 'logout-display-toggle') {
				closeUserDropdown()
			}
		},
		true
	)
}
const closeUserDropdown = () => {
	document.getElementById('dropdown').style.display = 'none'
}
const isUserDropdownOpen = () => {
	const dropdownDisplay = document.getElementById('dropdown').style.display
	const isOpen = dropdownDisplay !== 'none' && dropdownDisplay !== ''
	return isOpen
}

const openNav = () => {
	document.getElementById('mySidenav').style.width = '20em'
	window.addEventListener(
		'click',
		() => {
			closeNav()
		},
		true
	)
}
const closeNav = () => {
	document.getElementById('mySidenav').style.width = '0px'
}
const isNavOpen = () => {
	const isOpen =
		document.getElementById('mySidenav').style.width !== '0px' &&
		document.getElementById('mySidenav').style.width !== ''
	return isOpen
}

const eventsChartDivId = 'events-chart'
const eventsChartHeight = 550

const emailsChartDivId = 'emails-chart'
const emailChartHeight = 550

// Code adapted from https://developers.google.com/chart/interactive/docs/gallery/
// Load the Visualization API and the corechart package.
google.charts.load('current', { packages: ['line', 'bar', 'corechart'] })

// Set a callback to run when the Google Visualization API is loaded.
window.loadDashboardGraph = (mailList, calendarList) => {
	// These can be saved in a global variable to allow for chart resizing (they need to be redrawn)
	google.charts.setOnLoadCallback(drawEmailLineGraph(mailList))
	google.charts.setOnLoadCallback(drawEventsBarChart(calendarList))
}

window.loadMyTeamDashboardGraph = (mailList, calendarList) => {
	google.charts.setOnLoadCallback(drawEmailLineGraphCustomTooltip(mailList))
	google.charts.setOnLoadCallback(drawEventsBarChart(calendarList))
}

const drawEventsBarChart = (calendarList) => {
	let dataTable = new google.visualization.DataTable()
	dataTable.addColumn('string', 'Event subject')
	dataTable.addColumn('number', 'Total duration in minutes')

	dataTable.addRows(calendarList)

	let options = {
		chart: {
			title: 'Meetings in period',
		},
		bars: 'horizontal',
		legend: {
			position: 'none',
		},
		height: eventsChartHeight,
	}

	let chart = new google.charts.Bar(document.getElementById(eventsChartDivId))
	chart.draw(dataTable, google.charts.Bar.convertOptions(options))
}

const drawEmailLineGraph = (mailList) => {
	let dataTable = new google.visualization.DataTable()
	dataTable.addColumn('string', 'Date')
	dataTable.addColumn('number', 'Sent emails')
	dataTable.addColumn('number', 'Received emails')
	dataTable.addRows(mailList)

	let options = {
		chart: {
			title: 'Number of sent and received emails',
		},
		height: emailChartHeight,
	}

	let chart = new google.charts.Line(document.getElementById(emailsChartDivId))
	chart.draw(dataTable, google.charts.Line.convertOptions(options))
}

const drawEmailLineGraphCustomTooltip = (mailList) => {
	let dataTable = new google.visualization.DataTable()
	dataTable.addColumn('string', 'Date')
	dataTable.addColumn('number', 'Sent emails')
	dataTable.addColumn({ type: 'string', role: 'tooltip' })
	dataTable.addColumn('number', 'Received emails')
	dataTable.addColumn({ type: 'string', role: 'tooltip' })
	dataTable.addRows(mailList)

	let options = {
		chart: {
			title: 'Emails received by team members',
		},
		height: emailChartHeight,
	}
	let chart = new google.visualization.LineChart(
		document.getElementById(emailsChartDivId)
	)
	chart.draw(dataTable, options)
}
