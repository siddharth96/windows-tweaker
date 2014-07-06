import xlwt
import xml.etree.ElementTree as ET

from sys import argv


def generate_corresponding_xls(xaml_file1, xaml_file2):
    tree1 = ET.parse(xaml_file1)
    tree2 = ET.parse(xaml_file2)
    root1 = tree1.getroot()
    root2 = tree2.getroot()
    xaml2_children = root2.getchildren()
    wbk = xlwt.Workbook('utf-8')
    sheet = wbk.add_sheet('Translations')
    sheet.write(0, 0, 'Original Sentence')
    sheet.write(0, 1, 'Translation')
    for i, xaml1_child in enumerate(root1):
        sheet.write(i+1, 0, xaml1_child.text)
        sheet.write(i+1, 1, xaml2_children[i].text)
    wbk.save('translations.xls')


def main():
    xaml_file1 = argv[1]
    xaml_file2 = argv[2]
    generate_corresponding_xls(xaml_file1, xaml_file2)


if __name__ == "__main__":
    main()
