#!/usr/bin/env python
"""
Script Usage:
Write to a file with indentation: 
python %(scriptName)s --input <input-file> --output englishTermAndUiElementMap.json --indent=4

Write to a file without any indentation:
python %(scriptName)s --input <input-file> --output englishTermAndUiElementMap.json

Write to stdout with indentation:
python %(scriptName)s --input <input-file> --indent=4

Write to stdout without any indentation:
python %(scriptName)s --input <input-file-path>
"""
import argparse
import json
import Stemmer

from collections import defaultdict
from stemming import porter2

EN = "en"
DE = "de"
RU = "ru"
UI_ELEMENT_NAME = 0
MAIN_TAB = 1
SUB_TAB_CONTROL = 2
SUB_TAB = 3


def stemmer_meth(lang):
    if lang == EN:
        print 'Returning eng stem'
        return porter2.stem
    if lang == DE:
        print 'Returning german stem'
        return Stemmer.Stemmer('german').stemWord
    if lang == RU:
        print 'Returning russian stem'
        return Stemmer.Stemmer('russian').stemWord
    return lambda x: x


def stem_file(file_path, lang, output_file_path, indent, separator="=>"):
    term_and_ui_element_map = defaultdict(list)
    ui_element_map_for_term = lambda ui_element_row: \
        {"UiElement": ui_element_row[UI_ELEMENT_NAME].strip(),
         "MainTab": ui_element_row[MAIN_TAB].strip(),
         "SubTabControl": ui_element_row[SUB_TAB_CONTROL].strip(),
         "SubTab": ui_element_row[SUB_TAB].strip()}
    stemmer_func = stemmer_meth(lang)
    with open(file_path, 'r') as input_file:
        for line in input_file:
            if not line or separator not in line:
                continue
            row = line.splitlines()[0].strip()
            row = row.split(separator)
            line_to_stem = row[0]
            stemmed_line = [stemmer_func(_term.lower())
                            for _term in line_to_stem.split(' ')
                            if _term and len(_term) >= 3 and 
                            (_term.isalnum() or lang != EN)]
            if not stemmed_line:
                continue
            for _term in stemmed_line:
                ui_element_lst = row[1].split(',')
                if len(ui_element_lst) != 4:
                    continue
                term_and_ui_element_map[_term].append(
                    ui_element_map_for_term(ui_element_lst))
    if output_file_path:
        with open(output_file_path, 'w') as output_file:
            json.dump(term_and_ui_element_map, output_file,
                      indent=indent if indent else None)
    else:
        print json.dumps(term_and_ui_element_map,
                         indent=indent if indent else None)


def main():
    parser = argparse.ArgumentParser("Stemmer")
    parser.add_argument('--input', dest='input', default=None,
                        type=str, help='Input text file to be stemmed')
    parser.add_argument('--lang', dest='lang',
                        type=str, default=EN)
    parser.add_argument('--indent', dest='indent', type=int,
                        default=0)
    parser.add_argument('--output', dest='output', default=None,
                        type=str, help='Output file to write to')
    args = parser.parse_args()
    if not args.input:
        print __doc__ % {'scriptName': __file__}
        parser.error("No input text file provided")
    stem_file(args.input, args.lang, args.output, args.indent)


if __name__ == "__main__":
    main()
