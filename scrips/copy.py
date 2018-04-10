"""

it assumes that you have:
How to use:

"""

from __future__ import print_function
from subprocess import call
import os
import shutil, sys
import time

FILES_FOLDER = "files"
DATASET_FILES_LIST_FOLDER = "list"

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


if __name__ == "__main__":
    try:
        dataset_name = sys.argv[1]
    except IndexError:
        sys.exit('Missing a dataset label.')

    copy(dataset_name)
