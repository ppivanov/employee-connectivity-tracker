window.setPageTitle = (titleIn) => {
    document.getElementById("page-title").innerHTML = " - " + titleIn;
}

const toggleUserDropdown = () => {
    let element = document.getElementById('dropdown')
    element.classList.toggle('display-none')
}
