import data from './data.js'
const chapters_html = document.querySelector("#chapters")
const title_name = (new URLSearchParams(document.location.search)).get('name')
const chapters = data.find(el => title_name == el.name)
document.querySelector("#title").innerHTML = title_name.replace('9121', ':').replace('9120', '?')
chapters.chapters.forEach(element => {
    chapters_html.insertAdjacentHTML('beforeend', `<li><a href="./${element}.html">${element.split('/')[2].replace('9121', ':').replace('9120', '?')}</a></li>`)
});