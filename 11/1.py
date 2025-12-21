import sys

lines = open(sys.argv[1]).readlines()
devices = {}
for line in lines:
    split = [x.strip() for x in line.split(" ")]
    input = split[0].rstrip(":")
    outputs = split[1:]
    devices[input] = outputs


paths = set()


def traverse(curr, path):
    if curr == "out":
        paths.add(tuple(path))
        return

    next = devices[curr]
    for n in next:
        traverse(n, path + [curr])


traverse("you", [])
print(paths)
print(len(paths))
