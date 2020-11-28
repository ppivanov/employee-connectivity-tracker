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