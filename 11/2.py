import sys
from functools import cache
from functools import wraps

lines = open(sys.argv[1]).readlines()
devices = {}
for line in lines:
    split = [x.strip() for x in line.split(" ")]
    input = split[0].rstrip(":")
    outputs = split[1:]
    devices[input] = outputs


# Source - https://stackoverflow.com/a
# Posted by qwr, modified by community. See post 'Timeline' for change history
# Retrieved 2025-12-21, License - CC BY-SA 4.0
def memoize_first(func):
    """Memoize like functools.cache, but only consider first argument.

    Adapted from https://wiki.python.org/moin/PythonDecoratorLibrary
    """
    cache = func.cache = {}

    @wraps(func)
    def memoizer(arg1, *args):
        if arg1 not in cache:
            cache[arg1] = func(arg1, *args)
        return cache[arg1]

    return memoizer


@cache
def traverse(prev, curr, fft, dac):
    if curr == "out":
        if fft and dac:
            return 1
        else:
            return 0

    if curr not in devices:
        return 0

    next = devices[curr]
    return sum(
        [traverse(curr, n, fft or curr == "fft", dac or curr == "dac") for n in next]
    )


print(traverse(None, "svr", False, False))
