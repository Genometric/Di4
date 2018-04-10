"""
Requirements: BEDTools and BEDOPS are installed and PATH variable is set accordingly.

Syntax:

python run.py TOOL_NAME DATASET [--on-the-fly]

TOOL_NAME:  --bedtools or --bedops
DATASET:    a1, a2, a3, a4, b1, b2, c1, c2, c3

Usage examples:
- python run.py --bedtools c1
- python run.py --bedtools c1 --on-the-fly

Note: there must NOT be any space character in the path to this file.
"""

from __future__ import print_function
from subprocess import call
import os
import shutil, sys
import time

REPEATS = 10
BEDTOOLS = "bedtools"
BEDOPS = "bedops"
REF = "ref.narrowPeak"
SORTEDREF = "ref_sorted.narrowPeak"
FILES_FOLDER = "files"
DATASET_FILES_LIST_FOLDER = "list"

def list_files(scd):
    f = []
    for root, sub, files in os.walk(scd):
        for x in files:
            if x.endswith(".narrowPeak"):
                f += [os.path.join(root, x)]
    return f

def copy(dataset):
    with open(os.path.join(DATASET_FILES_LIST_FOLDER, dataset + ".txt"), "r") as l:
        if os.path.exists(dataset):
            shutil.rmtree(dataset)
        os.makedirs(dataset)
        for f in l:
            f = f.replace('\n', '')
            shutil.copyfile(
                os.path.join(FILES_FOLDER, f),
                os.path.join(dataset, f))

def pre_process(tool, dataset):
    path = os.path.dirname(os.path.realpath(__file__))

    results_path = os.path.join(path, dataset + "_results")
    if os.path.exists(results_path):
        shutil.rmtree(results_path)
    os.makedirs(results_path)

    sort_data_path = os.path.join(path, dataset + "_sorted")
    if os.path.exists(sort_data_path):
        shutil.rmtree(sort_data_path)
    os.makedirs(sort_data_path)

    ref_file_sorted = os.path.join(path, SORTEDREF)
    if os.path.isfile(ref_file_sorted):
        os.remove(ref_file_sorted)

    if tool == BEDTOOLS:
        c = "bedtools sort -i {} > {} ".format(os.path.join(path, REF), ref_file_sorted)
    elif tool == BEDOPS:
        c = "sort-bed {} > {}".format(os.path.join(path, REF), ref_file_sorted)
    call(c, shell=True)

    for f in list_files(os.path.join(path, dataset)):
        if tool == BEDTOOLS:
            command = "bedtools sort -i {} > {}".format(f, os.path.join(sort_data_path, os.path.basename(f)))
        elif tool == BEDOPS:
            command = "sort-bed {} > {}".format(f, os.path.join(sort_data_path, os.path.basename(f)))
        call(command, shell=True)
    return tool, sort_data_path, ref_file_sorted, results_path

def process(tool, sort_data_path, ref_file_sorted, results_path):
    for f in list_files(sort_data_path):
        if tool == BEDTOOLS:
            command = "bedtools intersect -a {} -b {} > {}".format(f, ref_file_sorted, os.path.join(results_path, os.path.basename(f)))
        elif tool == BEDOPS:
            command = "bedops --intersect {} {} > {}".format(f, ref_file_sorted, os.path.join(results_path, os.path.basename(f)))
        call(command, shell=True)


if __name__ == "__main__":
    try:
        tool = sys.argv[1]
    except IndexError:
        sys.exit("Tool name is missing; enter `--{}` or `--{}`".format(BEDTOOLS, BEDOPS))
    if tool == "--{}".format(BEDTOOLS):
        tool = BEDTOOLS
    elif tool == "--{}".format(BEDOPS):
        tool = BEDOPS
    else:
        sys.exit("Invalid tool name `{}`; enter `--{}` or `--{}`".format(tool, BEDTOOLS, BEDOPS))

    try:
        dataset_name = sys.argv[2]
    except IndexError:
        sys.exit('Missing a dataset label.')

    try:
        on_the_fly = True if sys.argv[3] == "--on-the-fly" else False
    except IndexError:
        on_the_fly = False

    copy(dataset_name)

    if on_the_fly:
        for r in range(0, REPEATS):
            start = time.time()
            process(*pre_process(tool, dataset_name))
            end = time.time()
            elapsed = end - start
            print("elapsed time:\t{}\tseconds".format(str(elapsed)))
    else:
        tool, sort_data_path, ref_file_sorted, results_path = pre_process(tool, dataset_name)
        print('Finished pre-processing files, now starting to process them.')
        for r in range(0, REPEATS):
            start = time.time()
            process(tool, sort_data_path, ref_file_sorted, results_path)
            end = time.time()
            elapsed = end - start
            print("elapsed time:\t{}\tseconds".format(str(elapsed)))
