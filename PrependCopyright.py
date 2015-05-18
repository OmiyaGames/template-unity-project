# Code copied from https://gist.github.com/pakdev/f5a21d108329ebeb8e72
# -*- coding: utf-8 -*-
import os
import re
import fnmatch
import argparse
from textwrap import dedent


parser = argparse.ArgumentParser(description='Add/update copyright on C# files')
parser.add_argument('root', nargs=1, help='Path to the root of the C# project')
args = parser.parse_args()

# Descend into the 'root' directory and find all *.cs files
files = []
for root, dirnames, filenames in os.walk(args.root[0]):
    for filename in fnmatch.filter(filenames, "*.cs"):
        files.append(os.path.join(root, filename))
print "Found {0} *.cs files".format(len(files))

for filepath in files:
    with open(filepath) as f:
        contents = f.read()

    # This regex will separate the contents of a *.cs file into two parts.
    # The first part is any text that appears before either 'using' or
    # 'namespace' - perhaps an old copyright. The second part *should* be
    # the actual code beginning with 'using' or 'namespace'.
    match = re.search(r"^.*?((using|namespace|/\*|#).+)$", contents, re.DOTALL)
    if match:
        # Make the file's now contain the user defined copyright (below)
        # followed by a blank line followed by the actual code.
        contents = dedent('''\
            // ****************************************************************************
            // <copyright file="{0}" company="Omiya Games">
            // The MIT License (MIT)
            // 
            // Copyright (c) 2014-2015 Omiya Games
            // 
            // Permission is hereby granted, free of charge, to any person obtaining a copy
            // of this software and associated documentation files (the "Software"), to deal
            // in the Software without restriction, including without limitation the rights
            // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
            // copies of the Software, and to permit persons to whom the Software is
            // furnished to do so, subject to the following conditions:
            // 
            // The above copyright notice and this permission notice shall be included in
            // all copies or substantial portions of the Software.
            // 
            // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
            // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
            // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
            // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
            // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
            // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
            // THE SOFTWARE.
            // </copyright>
            // <author>Taro Omiya</author>
            // <date>May 18, 2015</date>
            // ****************************************************************************
            ''').format(os.path.basename(filepath)) + match.group(1)
        with open(filepath, 'w') as f:
            f.write(contents)
        print "Wrote new: {0}".format(filepath)
