import subprocess


def generate_russian_json():
    subprocess.call(['python', 'stemmer.py', '--input',  'searchInputRussian.txt',
                               '--output', 'russianTermAndUiElementMap.json', '--lang=ru'])


def generate_german_json():
    subprocess.call(['python', 'stemmer.py', '--input', 'searchInputGerman.txt',
                               '--output', 'germanTermAndUiElementMap.json', '--lang=de'])


def generate_english_json():
    subprocess.call(['python', 'stemmer.py', '--input', 'searchInputEnglish.txt',
                               '--output', 'englishTermAndUiElementMap.json', '--lang=en'])

if __name__ == '__main__':
    generate_english_json()
    generate_german_json()
    generate_russian_json()