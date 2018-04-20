"""
This script runs Giggle to first index a dataset, then queries the indexed data using a query dataset.

Run it as the following: 

    python giggle.py PATH_TO_DATA_TO_BE_INDEXED PATH_TO_QUERY_DATASETS


NOTE: 
If you're trying to index a large number of samples, you may need to run the 
following command to before running Giggle:

    ulimit -Sn 16384

"""

from __future__ import print_function
from subprocess import call
import os
import shutil, sys
import time


TRIES = 10
GIGGLE_SOURCE = "/Users/vahid/Code/giggle/source"


def list_files(scd):
    f = []
    for root, sub, files in os.walk(scd):
        for x in files:
            if x.endswith(".narrowPeak"):
                f += [os.path.join(root, x)]
    return f

def create_clean_folder(path):
    if os.path.exists(path):
        shutil.rmtree(path)
    os.makedirs(path)

def sort_and_bgzip_file(file, gz_path):
    filename = os.path.splitext(file)[0]
    extension = os.path.splitext(file)[1]
    gz_file = os.path.join(gz_path, os.path.basename(filename)) + ".bed.gz"
    sorted_file = filename + "_tmp_sorted" + extension
    cmd = "bedtools sort -i {} > {}".format(file, sorted_file)
    call(cmd, shell=True)
    cmd = "cut -f1,2,3 {} | {}/lib/htslib/bgzip -c > {}".format(sorted_file, GIGGLE_SOURCE, gz_file)
    call(cmd, shell=True)
    os.remove(sorted_file)
    return gz_file

def process(path_to_source_files, path_to_query_files):
    gz_data_path = os.path.join(path_to_source_files, "gz")
    create_clean_folder(gz_data_path)

    gz_q_data_path = os.path.join(path_to_query_files, "gz")
    create_clean_folder(gz_q_data_path)

    idx_data_path = os.path.join(path_to_source_files, "idx")
    create_clean_folder(idx_data_path)

    print('Preparing datasets ... ')
    for f in list_files(path_to_source_files):
        sort_and_bgzip_file(f, gz_data_path)
    print('done!')

    print('Indexing now ... ')
    idxcmd = "{}/bin/giggle index -s -f -i \"{}\" -o {}".format(GIGGLE_SOURCE, gz_data_path + "/*gz", idx_data_path)
    call(idxcmd, shell=True)
    print('done!')

    for f in list_files(path_to_query_files):
        gz_file = sort_and_bgzip_file(f, gz_q_data_path)
        print("Now quering: " + f)
        for i in range(0, TRIES):
            start = time.time()
            qcmd = "{}/bin/giggle search -s -i {} -q {} > {}".format(GIGGLE_SOURCE, idx_data_path, gz_file, "res.bed")
            call(qcmd, shell=True)
            end = time.time()
            elapsed = end - start
            print("elapsed time:\t{}\tseconds".format(str(elapsed)))

if __name__ == "__main__":
    try:
        path_to_source_files = sys.argv[1]
    except IndexError:
        sys.exit("Missing source files path.")

    try:
        path_to_query_files = sys.argv[2]
    except IndexError:
        sys.exit("Missing query files path.")

    process(path_to_source_files, path_to_query_files)