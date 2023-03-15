
if (localStorage.getItem('hp') === 'list') {
    showList();
}
else {
    gridList();
}
function showList() {
    localStorage.setItem('hp', 'list');
    var $gridCont = $(".grid-container");
    $gridCont.addClass("list-view");
}
function gridList() {
    localStorage.setItem('hp', 'grid');
    var $gridCont = $(".grid-container");
    $gridCont.removeClass("list-view");
}

$(document).on("click", ".btn-grid", gridList);
$(document).on("click", ".btn-list", showList);


//---------------------------------------------------------------------//
/*var checkboxes = document.querySelectorAll(".checkbox");

let filtersSection = document.querySelector(".filters-section");

var filterList = document.querySelector(".filter-list");

for (var checkbox of checkboxes) {
    checkbox.addEventListener("click", function () {
        if (this.checked == true) {
            addElement(this, this.value);
            loadMissions(pg = 1);
        }
        else {
            removeElement(this.value);
            console.log("unchecked");
            loadMissions(pg = 1);
        }
    })
}


function addElement(current, value) {
    let filtersSection = document.querySelector(".filters-section");

    let createdTag = document.createElement('span');
    createdTag.classList.add('filter-list');
    createdTag.classList.add('ps-3');
    createdTag.classList.add('pe-1');
    createdTag.classList.add('me-2');
    createdTag.innerHTML = value;

    createdTag.setAttribute('id', value);
    let crossButton = document.createElement('button');
    crossButton.classList.add("filter-close-button");
    let cross = '&times;'
    crossButton.innerHTML = cross;


    crossButton.addEventListener('click', function () {
        let elementToBeRemoved = document.getElementById(value);

        console.log(elementToBeRemoved);
        console.log(current);
        elementToBeRemoved.remove();
        loadMissions(pg = 1);

        current.checked = false; 
    })

    // let crossButton = '&times;'

    createdTag.appendChild(crossButton);
    filtersSection.appendChild(createdTag);
    loadMissions(pg = 1);
}

*//*function ClearAllElement() {

    var filtersSection = document.querySelector(".filters-section");
    var filtersSectioncity = document.querySelector("#filterlistcity");
    filtersSectioncity.innerHTML = "";
    filtersSection.innerHTML = "";

    $(".citycheck").prop('checked', false);
    $(".otherthencity").prop('checked', false);
    loadMissions(pg = 1);
}
*//*
function removeElement(value) {

    let filtersSection = document.querySelector(".filters-section");

    let elementToBeRemoved = document.getElementById(value);
    filtersSection.removeChild(elementToBeRemoved);
    loadMissions(pg = 1);
}*/


//--------------------------------------------------------------------------//



var cbs = document.querySelectorAll('.checkbox');
for (var i = 0; i < cbs.length; i++) {
    cbs[i].addEventListener('change', function () {
        if (this.checked) {
            $("input[type=checkbox][value='" + this.value + "']").prop('checked', true);
            addElement(this, this.value);
        }
        else {
            $("input[type=checkbox][value='" + this.value + "']").prop('checked', false);
            removeElement(this.value);
            loadMissions(pg = 1);
        }
    });
}


function addElement(current, value) {
    let filtersSection = document.querySelector(".filters-section");

    let createdTag = document.createElement('span');
    createdTag.classList.add('filter-list');
    createdTag.classList.add('fs-7');
    createdTag.classList.add('border');
    createdTag.classList.add('px-2');
    createdTag.classList.add('me-2');
    createdTag.classList.add('mb-2');
    createdTag.classList.add('rounded-pill');
    createdTag.classList.add('text-secondary');

    createdTag.innerHTML = value;

    createdTag.setAttribute('id', value);
    let crossButton = document.createElement('button');
    crossButton.classList.add("filter-close-button");
    crossButton.style.textDecoration = 'none';
    crossButton.classList.add('btn-light');
    crossButton.classList.add('ms-1');
    let cross = '&times;'


    crossButton.addEventListener('click', function () {
        let elementToBeRemoved = document.getElementById(value);

        $("input[type=checkbox][value='" + value + "']").prop('checked', false);

        elementToBeRemoved.remove();
        loadMissions(pg = 1);
    })

    crossButton.innerHTML = cross;


    createdTag.appendChild(crossButton);
    filtersSection.appendChild(createdTag);
    loadMissions(pg = 1);
}

/*function ClearAllElement() {

    var filtersSection = document.querySelector(".filters-section");
    var filtersSectioncity = document.querySelector("#filterlistcity");
    filtersSectioncity.innerHTML = "";
    filtersSection.innerHTML = "";

    $(".citycheck").prop('checked', false);
    $(".otherthencity").prop('checked', false);
    loadMissions(pg = 1);
}*/


function removeElement(value) {

    let filtersSection = document.querySelector(".filters-section");
    let elementToBeRemoved = document.getElementById(value);
    filtersSection.removeChild(elementToBeRemoved);
    loadMissions(pg = 1);
}
