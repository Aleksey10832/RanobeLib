import data from './data.js'
const titles = document.querySelector('#titles')

data.forEach(element => {
    titles.insertAdjacentHTML('beforeend', `<a href="./novels/?name=${element.name}">${element.name.replace('9121', ':').replace('9120', '?')}</a>`) 
});