import requests
import os
from bs4 import BeautifulSoup

def js_save(name, chapters):
    try:
        json = ''
        with open('./novels/data.js', 'r', encoding='utf8') as file:
            json = file.read()
            file.close()
        json = json.replace('/**/', ', {' + f"name: '{name}', chapters: {chapters}" + '}/**/')
        with open('./novels/data.js', 'w', encoding='utf8') as file:
            file.write(json)
            file.close()
    except Exception as e:
        print(e)
        with open('./novels/data.js', 'w', encoding='utf8') as file:
            file.write("export default [/**/]")
            file.close()
        json = 'export default [/**/]'.replace('/**/', '{' + f"name: '{name}', chapters: {chapters}" + '}/**/')
        with open('./novels/data.js', 'w', encoding='utf8') as file:
            file.write(json)
            file.close()

list_chapters = []
url = input('Введите название тайтла содержащегося в url страницы: ')
req = requests.get(f"https://ranobe.me/{url}")
html = BeautifulSoup(req.text, "html.parser")
title = html.find('h1').text.split(sep=' | ')[0].replace(':', '9121').replace('•', '').replace('?', '9120')
try:
    count_pages = html.findAll('div', class_='FicContentsChapterName')
except:
    count_pages = []
print('Начало загрузки тайтла:', title)

try:
    os.makedirs(f'./novels/{title}')
    count_pages2 = 0
    if count_pages == []:
        count_pages2 = 1
    # else:
        # print(0)
        # count_pages2 = int(count_pages[-1].text)
        # print(1)
    print(count_pages)
    
    main_page = html

    chapters = count_pages
    try:
        for chapter in chapters:
            https = requests.get('https://ranobe.me/' + chapter.find('a').get('href'))
            
            page = BeautifulSoup(https.text, "html.parser")
            name = page.find('h1').text
            
            name = name.replace(':', '9121').replace('•', '').replace('?', '9120')
            texts = page.find_all('p', class_='fict')
            text = ''
            if(len(texts) > 0):
                for paragraph in texts:
                    text += f"<p class='paragraph'>{paragraph.text}</p>\n"                

            imgs = page.findAll('img', class_="ranobe_image")
            imgs_path = []
            if imgs:
                imgs_url = []
                for o in imgs:
                    imgs_url.append(f"https://ranobe.me/{o.get('src')}")
                count = 0
                for img_url in imgs_url:
                    image = requests.get(img_url, stream=True)
                    with open(f"./novels/imgs/{imgs[count].get('src').split('/')[-1]}", "wb") as file:
                        for chunk in image.iter_content(chunk_size=8192):
                            file.write(chunk)
                        file.close()
                    imgs_path.append(f"../imgs/{imgs[count].get('src').split('/')[-1]}")
                    count += 1
            imgs_html = ""
            if imgs_path != []:
                for img in imgs_path:
                    imgs_html += f"<img src='{img}' alt='404'>"
            with open(f'./novels/{title}/{name}.html', 'w', encoding='utf-8') as file:
                file.write(f'''<!DOCTYPE html>
<meta charset="UTF-8">
<link rel="stylesheet" href="../style.css">
<html lang="en">
<head>
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>{name.replace('9121', ':').replace('9120', '?')}</title>
</head>
<body>
<h1 class='title'>{name.replace('9121', ':').replace('9120', '?')}</h1>
{text}
{imgs_html}
</body>
</html>
''')
                file.close()
            list_chapters.append(f'./{title}/{name}')
            print(f"Загружена глава {name.replace('9121', ':')}")
    except Exception as e:
        print('ошибка парсинга главы' + e)
    js_save(title, list_chapters)
except Exception as e:
    print(e)
